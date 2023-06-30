using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Data.Rooms;
using UnityEngine;
using UnityEngine.EventSystems;
using Socket;
using PlayerRoom.View;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera roomCamera;

    enum TAP_STATE { NONE, AVATAR, FURNITURE, CHAIR, BUBBLE_PRIVATE_CHAT };

    public delegate void TapAvatar(AvatarRoomController avatarRoomController);
    public static event TapAvatar OnTapAvatar;

    public delegate void TapFurniture(FurnitureRoomController furnitureRoomController);
    public static event TapFurniture OnTapFurniture;

    public delegate void TapBubblePrivateChat(AvatarRoomController avatarRoomController);
    public static event TapBubblePrivateChat OnTapBubblePrivateChat;

    private bool canCheck = false; //Prevent walk over UI (we make it true on mousedown and ask state on mouseup)
    private float tapTime;
    private TAP_STATE tapState;
    private const float longTapSeconds = 0.5f;
    private IGameData gameData;
    private Chair chairTapped;

    AvatarRoomController avatarRoomControllerTapped;
    FurnitureRoomController furnitureRoomControllerTapped;

    private void Awake()
    {
        gameData = Injection.Get<IGameData>();
        tapState = TAP_STATE.NONE;
    }

    public static bool IsPointerOverGameObject()
    {
        //check mouse
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        // check touch
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
            {
                return true;
            }
        }

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void Update()
    {
        ControlInputUpdate();
    }

    List<RaycastHit2D> GetHitObjectList(RaycastHit2D[] arrayHit2D)
    {
        List<RaycastHit2D> listHit2D = new List<RaycastHit2D>(arrayHit2D);

        listHit2D.Sort(delegate (RaycastHit2D p1, RaycastHit2D p2)
        {
            return p1.transform.position.y.CompareTo(p2.transform.position.y);
        });

        List<RaycastHit2D> objectList = new List<RaycastHit2D>();

        foreach (RaycastHit2D hit2D in listHit2D)
        {
            if (hit2D.collider.tag == Tags.Player || hit2D.collider.tag == Tags.Furniture || hit2D.collider.tag == Tags.Chair || hit2D.collider.tag == Tags.BubblePrivateChat)
            {
                objectList.Add(hit2D);
            }
        }

        return objectList;
    }

    void ControlInputUpdate()
    {
        if (CurrentRoom.Instance.IsInEditionMode)
        {
            return;
        }

        //Mouse Down
        if (InputFunctions.GetMouseButtonDown(0))
        {
            canCheck = !IsPointerOverGameObject();

            if (canCheck)
            {
                //Get hit item
                Vector3 worldTouch = roomCamera.ScreenToWorldPoint(InputFunctions.mousePosition);
                RaycastHit2D[] hitList = Physics2D.RaycastAll(new Vector2(worldTouch.x, worldTouch.y), Vector2.zero);
                if (hitList.Length > 0)
                {
                    List<RaycastHit2D> hitItemList = GetHitObjectList(hitList);

                    for (int i = hitItemList.Count - 1; i >= 0; i--)
                    {
                        RaycastHit2D hitItem = hitItemList[i];
                        Debug.Log(hitItemList[i].transform.name);
                        switch (hitItem.collider.tag)
                        {
                            case Tags.Player:
                                {
                                    avatarRoomControllerTapped = hitItem.collider.gameObject.GetComponent<AvatarRoomController>();

                                    if (avatarRoomControllerTapped != null)
                                    {
                                        tapState = TAP_STATE.AVATAR;
                                        tapTime = 0;
                                    }
                                    break;
                                }
                            case Tags.Furniture:
                                furnitureRoomControllerTapped = hitItem.transform.parent.transform.parent.GetComponent<FurnitureRoomController>();
                                if (furnitureRoomControllerTapped != null)
                                {
                                    if (furnitureRoomControllerTapped.IsChair())
                                    {
                                        chairTapped = furnitureRoomControllerTapped.GetSitPoint(worldTouch);
                                    }

                                    if (CurrentRoom.Instance.IsMyRoom())
                                    {
                                        tapState = TAP_STATE.FURNITURE;
                                        tapTime = 0;
                                    }
                                }
                                break;
                            case Tags.Chair:
                                tapTime = 0;
                                tapState = TAP_STATE.CHAIR;
                                chairTapped = hitItem.transform.GetComponent<Chair>();
                                break;
                            case Tags.BubblePrivateChat:
                                AvatarRoomController avatarRoomController = hitItem.transform.GetComponent<AvatarRoomController>();
                                if (avatarRoomController != null)
                                {
                                    canCheck = false;
                                    avatarRoomController.AvatarNotificationController.ShowWhitePrivateChat();
                                    if (OnTapBubblePrivateChat != null)
                                    {
                                        OnTapBubblePrivateChat(avatarRoomController);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        //Mouse still down
        if (InputFunctions.GetMouseButton(0) && (tapState == TAP_STATE.AVATAR || tapState == TAP_STATE.FURNITURE))
        {
            tapTime += Time.deltaTime;
            if (tapTime >= longTapSeconds)
            {
                //TAP AVATAR
                if (tapState == TAP_STATE.AVATAR)
                {
                    tapState = TAP_STATE.NONE;
                    canCheck = false;
                    if (OnTapAvatar != null)
                    {
                        OnTapAvatar(avatarRoomControllerTapped);
                    }
                    Debug.Log("CLICK AVATAR");
                }

                //TAP FURNITURE
                if (tapState == TAP_STATE.FURNITURE)
                {
                    tapState = TAP_STATE.NONE;
                    canCheck = false;
                    if (OnTapFurniture != null)
                    {
                        OnTapFurniture(furnitureRoomControllerTapped);
                    }
                    Debug.Log("CLICK FURNITURE");
                }
            }
        }

        if (InputFunctions.GetMouseButtonUp(0))
        {
            if (canCheck)
            {
                AvatarRoomController myAvatar = CurrentRoom.Instance.AvatarsManager.GetMyAvatar();
                RoomInformation roomInformation = CurrentRoom.Instance.RoomInformation;

                if (chairTapped != null)
                {
                    SitBehaviour(roomInformation, myAvatar);
                }
                else
                {
                    //Click Walk
                    Vector3 worldTouch = roomCamera.ScreenToWorldPoint(InputFunctions.mousePosition);

                    //Local movement
                    myAvatar.SetWalkToDestination(worldTouch.x, worldTouch.y);
                    //Send movement to backend

                    SimpleSocketManager.Instance.SendAvatarMove(roomInformation.RoomName, roomInformation.RoomIdInstance, worldTouch.x, worldTouch.y);

                }
            }

            canCheck = false;
            tapState = TAP_STATE.NONE;
            chairTapped = null;
            avatarRoomControllerTapped = null;
            furnitureRoomControllerTapped = null;
        }
    }

    public void SitBehaviour(RoomInformation roomInformation, AvatarRoomController myAvatar)
    {
        if (!chairTapped.IsOccupied())
        {
            myAvatar.SetGoSit(chairTapped);
            SimpleSocketManager.Instance.SendAvatarMove(roomInformation.RoomName, roomInformation.RoomIdInstance, chairTapped.transform.position.x, chairTapped.transform.position.y);
        }
        else
        {
            //Click Walk
            Vector3 worldTouch = roomCamera.ScreenToWorldPoint(InputFunctions.mousePosition);

            //Local movement
            myAvatar.SetWalkToDestination(worldTouch.x, worldTouch.y);
            //Send movement to backend

            SimpleSocketManager.Instance.SendAvatarMove(roomInformation.RoomName, roomInformation.RoomIdInstance, worldTouch.x, worldTouch.y);
        }
    }
}

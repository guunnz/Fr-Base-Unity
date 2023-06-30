using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Data.Users;
using UnityEngine;
using DG.Tweening;
using Managers.Avatar;
using Pathfinding;
using Data.Rooms;
using Socket;

public class AvatarRoomController : MonoBehaviour
{
    [SerializeField] private AvatarCustomizationController avatarCustomizationController;
    [SerializeField] private AvatarAnimationController avatarAnimationController;
    [SerializeField] AvatarNotificationController avatarNotificationController;
    [SerializeField] private AIPath aIPath;
    [SerializeField] private AIDestinationSetter aIDestinationSetter;
    [SerializeField] private AvatarSittingController avatarSittingController;
    public AvatarPetManager avatarPetManager;
    [SerializeField] private GameObject avatarContainer;

    public AvatarRoomData AvatarData { get; private set; }

    public AvatarAnimationController AnimationController => avatarAnimationController;

    public AvatarCustomizationController CustomizationController => avatarCustomizationController;
    public AvatarNotificationController AvatarNotificationController => avatarNotificationController;

    private GameObject destinationPointsContainer;
    private GameObject destinationPoint;
    private bool isLocalPlayer;

    private Seeker seeker;

    public void Init(AvatarRoomData avatarData, GameObject destinationPointsContainer, bool isLocalPlayer)
    {
        seeker = GetComponent<Seeker>();
        this.AvatarData = avatarData;
        this.destinationPointsContainer = destinationPointsContainer;
        this.isLocalPlayer = isLocalPlayer;
        //Set Avatar Skin
        avatarCustomizationController.SetAvatarCustomizationData(avatarData.AvatarCustomizationData.GetSerializeData());
        //Set Avatar Position
        SetOrientation(avatarData.Orientation);
        if (isLocalPlayer || avatarData.AvatarState == AvatarState.Sitting)
        {
            SetPosition(avatarData.Positionx, avatarData.Positiony);
            destinationPoint = new GameObject(avatarData.Username + "_destinationPoint");
            destinationPoint.transform.position = new Vector3(avatarData.Positionx, avatarData.Positiony, 0);
            destinationPoint.transform.SetParent(destinationPointsContainer.transform);
            aIDestinationSetter.target = destinationPoint.transform;
        }
        else
        {
            destinationPoint = new GameObject(avatarData.Username + "_destinationPoint");
            destinationPoint.transform.position = new Vector3(avatarData.Positionx, avatarData.Positiony, 0);
            destinationPoint.transform.SetParent(destinationPointsContainer.transform);

            StartCoroutine(SetNonLocalPosition());
        }
        //Debug.Log(aIPath.destination.x + " " +  aIPath.destination.y);
    }

    IEnumerator SetNonLocalPosition()
    {
        Path path = seeker.StartPath(Vector2.zero, destinationPoint.transform.position);
        while (!path.IsDone())
            yield return null;

        List<Vector3> pathList = path.vectorPath;
        Vector3 LastPos = pathList[pathList.Count - 1];
        aIDestinationSetter.target = destinationPoint.transform;
        SetPosition(LastPos.x, LastPos.y);
    }

    void SetPosition(float positionx, float positiony)
    {
        AvatarData.Positionx = positionx;
        AvatarData.Positiony = positiony;
        Vector3 currentPosition = transform.position;

        currentPosition.x = positionx;
        currentPosition.y = positiony;

        transform.position = currentPosition;
    }

    public void SetOrientation(int orientation)
    {
        if (orientation != 1 && orientation != -1)
        {
            return;
        }

        avatarNotificationController.SetOrientation(orientation);

        Vector3 scale = transform.localScale;

        scale.x = Mathf.Abs(scale.x) * orientation;

        transform.localScale = scale;
    }

    public bool IsLocalPlayer()
    {
        return isLocalPlayer;
    }

    public void SetWalkToDestination(float positionx, float positiony, bool walkToChair = false)
    {
        if (positionx == 0 && positiony == 0)
        {
            return;
        }
        if (!aIPath.enabled)
        {
            aIPath.enabled = true;
        }

        if (!walkToChair && AvatarData.AvatarState != AvatarState.Moving)
        {
            if (AvatarData.AvatarState == AvatarState.Sitting)
            {
                SetState(AvatarState.Moving);
            }
            else
            {
                SetState(AvatarState.Moving, useSocket: false);
            }
        }

        avatarAnimationController.SetWalkState();

        destinationPoint.transform.position = new Vector3(positionx, positiony, 0);

        //this.transform.DOKill(false);

        //this.transform.DOMove(new Vector2(positionx, positiony), 1).SetEase(Ease.Linear).OnComplete(()=> {
        //    avatarAnimationController.SetIdleState();
        //});

    }


    public void SetGoSit(Chair chair)
    {
        SetState(AvatarState.GoingToSit);
        avatarSittingController.GoSit(chair);
    }

    private void SetSit(Vector2 position)
    {
        Debug.Log("SetSit");
        aIPath.enabled = false;
        avatarAnimationController.SetSitState();
        this.transform.position = position;
        //Debug.LogError(CurrentRoom.Instance.GetChair(position));
        avatarSittingController.SetChair(CurrentRoom.Instance.GetChair(position));
        avatarSittingController.OccupyChair();
    }

    public string GetState()
    {
        return AvatarData.AvatarState;
    }

    public void SetState(string status, float x = 0, float y = 0, int orientation = 1, bool useSocket = true)
    {
        if (useSocket)
        {
            RoomInformation roomInformation = CurrentRoom.Instance.RoomInformation;
            SimpleSocketManager.Instance.SendAvatarStatus(roomInformation.RoomName, roomInformation.RoomIdInstance, x, y, orientation, status);
        }
        if (status == AvatarState.Sitting)
        {
            SetSit(new Vector2(x, y));
            //this.SetOrientation(orientation);
        }
        else if (GetState() == AvatarState.Sitting)
        {
            avatarSittingController.DeocupyChair();
        }

        AvatarData.SetState(status);
    }

    private void Update()
    {
        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        Vector2 vel = aIPath.desiredVelocity;
        if (vel.magnitude > 0.01 && AvatarData.AvatarState != AvatarState.Sitting)
        {
            avatarAnimationController.SetWalkState();
            var sign = Mathf.Sign(vel.x);
            this.transform.localScale = new Vector3(sign, 1, 1);
            avatarNotificationController.SetOrientation(sign);
        }
        else if (AvatarData.AvatarState != AvatarState.Sitting && AvatarData.AvatarState != AvatarState.GoingToSit)
        {
            if (AvatarData.AvatarState != AvatarState.Idling)
            {
                //SetState(AvatarState.Idling);
            }
            avatarAnimationController.SetIdleState();
        }
    }

    public void DestroyAvatar()
    {
        Destroy(destinationPoint);
        Destroy(this.gameObject);
    }

    public void Hide()
    {
        avatarContainer.SetActive(false);
        if (avatarPetManager.GetCurrentPetObject() != null)
            avatarPetManager.GetCurrentPetObject().SetActive(false);
    }

    public void Show()
    {
        avatarContainer.SetActive(true);
        if (avatarPetManager.GetCurrentPetObject() != null)
            avatarPetManager.GetCurrentPetObject().SetActive(true);
    }
}

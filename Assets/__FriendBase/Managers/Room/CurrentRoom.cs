using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture.Injector.Core;
using Data;
using Data.Catalog;
using AddressablesSystem;
using Data.Rooms;
using Data.Bag;
using Socket;
using PlayerRoom.View;
using System.Linq;
using Managers.InRoomGems;
using UnityEngine.SceneManagement;

public class CurrentRoom : GenericSingleton<CurrentRoom>
{
    [SerializeField] private GameObject roomContainer;
    [SerializeField] private GameObject avatarContainer;
    [SerializeField] private GameObject furnituresContainer;
    [SerializeField] private GameObject destinationPointsContainer;
    [SerializeField] private GameObject petContainer;
    [SerializeField] private RoomUIReferences roomUIReferences;
    [SerializeField] private AvatarRoomController avatarPrefab;
    [SerializeField] private FurnitureRoomController furniturePrefab;
    [SerializeField] private InRoomGemsController inRoomGemsController;
    private List<Chair> chairList = new List<Chair>();
    public int UserId { get; private set; }
    public AvatarsManager AvatarsManager { get; private set; }

    public FurnituresRoomManager FurnituresRoomManager { get; private set; }
    public RoomSnapshotAvatarManager RoomSnapshotAvatarManager { get; private set; }

    public bool IsReady { get; private set; }
    public RoomInformation RoomInformation { get; private set; }

    public RoomManager CurrentRoomManager => currentRoomManager;

    private IGameData gameData;
    private ILoader loader;

    private RoomManager currentRoomManager;
    private ExitRoomManager exitRoomManager;

    public bool IsInEditionMode { get; set; }
    public int EventType { get; private set; }

    Coroutine scanPathfindingCoroutine;

    public ChatManager chatManager { get; private set; }

    public Chair GetChair(Vector2 destination)
    {
        chairList = FindObjectsOfType<Chair>().ToList();
        float min = 100000;
        Chair closestChair = chairList.FirstOrDefault();
        if (closestChair == null)
            return null;
        foreach (Chair chair in chairList)
        {
            float distance = Vector2.Distance(chair.GetSitpoint(), destination);
            if (distance < min)
            {
                closestChair = chair;
                min = distance;
            }
        }

        return closestChair;
    }
    
     public bool IsGuest(string idFirebase)
     {
         bool isGuest = AvatarsManager.GetAvatarById(idFirebase).AvatarData.IsGuest;
         return isGuest;
     }

    public void SetReady(bool flag)
    {
        IsReady = flag;
        gameData = Injection.Get<IGameData>();
        if (flag && gameData.GetUserInformation().UserStatus.IsSuspended())
        {
            if (!IsMyRoom())
            {
                GoToMyHouse();
                return;
            }
            GetRoomUIReferences().MsgPanelBanned.OpenWithSuspendedDescription(gameData.GetUserInformation().UserStatus.GetTimeSuspensionLeft() ,null);
        }
    }

    public void InitRoom(int userId, RoomInformation roomInformation, int eventType)
    {
        SimpleSocketManager.Instance.OnCloseConnection += OnCloseConnection;
        SimpleSocketManager.Instance.OnUserBanned += OnUserBanned;
        SimpleSocketManager.Instance.OnUserSuspended += OnUserSuspended;

        chatManager = new ChatManager();

        gameData = Injection.Get<IGameData>();
        loader = Injection.Get<ILoader>();

        this.UserId = userId;
        this.AvatarsManager = new AvatarsManager(avatarPrefab, avatarContainer, destinationPointsContainer);
        this.FurnituresRoomManager = new FurnituresRoomManager(furniturePrefab, furnituresContainer);
        this.RoomSnapshotAvatarManager = new RoomSnapshotAvatarManager();

        this.RoomInformation = roomInformation;
        this.EventType = eventType;

        roomUIReferences.Initialize(IsPublicRoom(), IsMyRoom(), EventType);

        exitRoomManager = new ExitRoomManager(this, roomInformation, roomUIReferences.Loader);
        exitRoomManager.OnExitRoom += OnExitRoom;

        AvatarsManager.OnUserKicked += OnUserKicked;

        CreateRoomPrefab();

        FurnituresRoomManager.OnUpdatePathfinding += OnUpdatePathfinding;

        chairList = FindObjectsOfType<Chair>().ToList();

        inRoomGemsController.Init();

        IsInEditionMode = false;
    }

    public void TempFurniture(int action)
    {
        FurnitureRoomData roomData = FurnituresRoomManager.GetFurnitureByIndex(0)?.FurnitureRoomData;

        GenericCatalogItem item = gameData.GetCatalogByItemType(ItemType.TABLE).GetItemByIndex(0);
        GenericBagItem bagItem = gameData.GetBagByItemType(ItemType.TABLE).GetItemByIndex(0);

        switch (action)
        {
            case 0:
                SimpleSocketManager.Instance.SendFurnitureAdd(RoomInformation.RoomName, RoomInformation.RoomIdInstance, bagItem.IdInstance, item.IdItemWebClient, 0, -1.5f, 1);
                break;
            case 1:
                if (roomData!=null)
                {
                    SimpleSocketManager.Instance.SendFurnitureMove(RoomInformation.RoomName, RoomInformation.RoomIdInstance, roomData.IdInstance, roomData.Position.x + 1, -1.5f, 1);
                }
                break;
            case 2:
                if (roomData != null)
                {
                    SimpleSocketManager.Instance.SendFurnitureRemove(RoomInformation.RoomName, RoomInformation.RoomIdInstance, roomData.IdInstance);
                }
                break;
        }
    }

    void OnUpdatePathfinding(FurnitureRoomController furnitureRoomController, FurnituresRoomManager.PATHFINDING_REASON pathfindingReason)
    {
        RescanPathfinding();
    }

    public void RescanPathfinding()
    {
        //Prevent to have two items Scanning at the same time
        if (scanPathfindingCoroutine!=null)
        {
            StopCoroutine(scanPathfindingCoroutine);
        }
        scanPathfindingCoroutine = StartCoroutine(RescanPathfindingCoroutine());
    }

    IEnumerator RescanPathfindingCoroutine()
    {
        CurrentRoomManager.ActivateColliders();
        yield return new WaitForEndOfFrame();
        AstarPath.active.Scan();
        CurrentRoomManager.DeactivateColliders();
    }

    void CreateRoomPrefab()
    {
        GameObject roomPrefab = loader.GetModel(RoomInformation.NamePrefab + "_prefab");
        roomPrefab.transform.position = Vector3.zero;
        roomPrefab.transform.SetParent(roomContainer.transform, true);

        currentRoomManager = roomPrefab.GetComponent<RoomManager>();
        currentRoomManager.SetPathfinding();
    }

    public void SetCameras()
    {
        Transform player = AvatarsManager.GetMyAvatar().transform;

        currentRoomManager.SetCameras(player);
    }

    public bool IsMyRoom()
    {
        return UserId == gameData.GetUserInformation().UserId;
    }

    public bool IsPublicRoom()
    {
        return RoomInformation.RoomType == RoomType.PUBLIC;
    }

    public bool IsPrivateRoom()
    {
        return RoomInformation.RoomType == RoomType.PRIVATE;
    }

    public RoomUIReferences GetRoomUIReferences()
    {
        if (roomUIReferences == null)
        {
            gameData.GetRoomInformation();
        }
        return roomUIReferences;
    }

    public void UpdateEventType(int eventType)
    {
        this.EventType = eventType;
        roomUIReferences.UpdateEventType(this.EventType);
    }

    private void OnUserBanned()
    {
        DestroyRoom();
        SceneManager.UnloadSceneAsync(GameScenes.RoomScene);
    }

    private void OnUserSuspended()
    {
        if (!IsMyRoom())
        {
            GoToMyHouse();
        }
        else
        {
            GetRoomUIReferences().CloseRoomListPanel();
            GetRoomUIReferences().MsgPanelBanned.OpenWithSuspendedDescription(gameData.GetUserInformation().UserStatus.GetTimeSuspensionLeft(), null);
        }
    }

    void OnCloseConnection()
    {
        DestroyRoom();
    }

    public void DestroyRoom()
    {
        SimpleSocketManager.Instance.OnCloseConnection -= OnCloseConnection;
        SimpleSocketManager.Instance.OnUserBanned -= OnUserBanned;
        SimpleSocketManager.Instance.OnUserSuspended -= OnUserSuspended;
        AvatarsManager.OnUserKicked -= OnUserKicked;
        exitRoomManager.OnExitRoom -= OnExitRoom;
        AvatarsManager.Destroy();
        FurnituresRoomManager.Destroy();
        FurnituresRoomManager.OnUpdatePathfinding -= OnUpdatePathfinding;
        exitRoomManager.Destroy();
        chatManager.Destroy();
    }

    void OnUserKicked()
    {
        ExitError();
    }

    void OnExitRoom()
    {
        DestroyRoom();
    }

    public void LinkProvider()
    {
        exitRoomManager.GoToLinkProviderScene();
    }

    public void GoToAvatarCustomization()
    {
        exitRoomManager.GoToAvatarCustomization();
    }

    public void GoToMinigames()
    {
        exitRoomManager.GoToMinigames();
    }

    public void GoToNewRoom(RoomInformation roomInformation)
    {
        exitRoomManager.GoToOtheRoom(roomInformation);
    }

    public void GoToMyHouse()
    {
        exitRoomManager.GoToOtheRoom(gameData.GetMyHouseInformation());
    }

    public void ExitError()
    {
        exitRoomManager.ExitError();
    }

    public void GoToBannedScene()
    {
        exitRoomManager.ExitBannedScene();
    }

    public void ReloadSameRoom()
    {
        exitRoomManager.GoToOtheRoom(RoomInformation);
    }

    public Transform GetDestinationTransform()
    {
        if (destinationPointsContainer == null)
            return null;
        return destinationPointsContainer.transform;
    } 
    
    public Transform GetPetContainer()
    {
        if (petContainer == null)
            return null;
        return petContainer.transform;
    }
}
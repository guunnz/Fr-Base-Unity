using System.Collections;
using System.Collections.Generic;
using AddressablesSystem;
using Architecture.Injector.Core;
using Data;
using Data.Catalog;
using Data.Users;
using Socket;
using UnityEngine;
using Data.Rooms;
using Architecture.ViewManager;
using UnityEngine.SceneManagement;
using AvatarCustomization;
using UnityEngine.UI;
using DG.Tweening;
using Pathfinding;

public class RoomJoinManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup loaderPanel;

    private IGameData gameData;
    private ILoader loader;
    private IAnalyticsSender analyticsSender;
    RoomInformation roomInformation;

    public delegate void RoomReady();
    public static event RoomReady OnRoomReady;

    void Start()
    {
        gameData = Injection.Get<IGameData>();
        loader = Injection.Get<ILoader>();
        analyticsSender = Injection.Get<IAnalyticsSender>();
        CurrentRoom.Instance.SetReady(false);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.JOIN_ROOM, OnJoinRoom);
        ILoading loading = Injection.Get<ILoading>();

        /*
        if (loading.isloading())
        {
            loading.Unload();
        }
        */

        StartCoroutine(JoinRoom());
    }


    IEnumerator JoinRoom()
    {
        //roomInfo = gameData.GetCatalogByItemType(Data.Catalog.ItemType.ROOM).GetItem(2);
        roomInformation = gameData.GetRoomInformation();

        //PRELOAD ITEMS TO USE IN THE ROOM
        string[] namesToPreload = { roomInformation.NamePrefab + "_prefab" };

        analyticsSender.SendAnalytics(AnalyticsEvent.JoinRoom, roomInformation.NamePrefab);
        foreach (string nameToPreload in namesToPreload)
        {
            loader.LoadItem(new LoaderItemModel(nameToPreload));
        }


        //WAIT UNTIL ALL ITEMS HAS BEEN LOADED
        while (!AreAllItemsLoaded(namesToPreload))
        {
            yield return null;
        }

        //Send JOIN to the server

        yield return null;
        SimpleSocketManager.Instance.JoinChatRoom(roomInformation.RoomName, roomInformation.RoomIdInstance, 3f, -1f);
        yield return null;
    }

    bool AreAllItemsLoaded(string[] namesToCheck)
    {
        foreach (string nameToPreload in namesToCheck)
        {
            LoaderAbstractItem item = loader.GetItem(nameToPreload);
            if (item == null || item.State != LoaderItemState.SUCCEED)
            {
                return false;
            }
        }
        return true;
    }

    void OnJoinRoom(AbstractIncomingSocketEvent incomingEvent)
    {
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.JOIN_ROOM, OnJoinRoom);

        IncomingEventJoinRoom incomingEventJoinRoom = incomingEvent as IncomingEventJoinRoom;
        if (incomingEventJoinRoom != null && incomingEventJoinRoom.State == SocketEventResult.OPERATION_SUCCEED)
        {
            int userId = gameData.GetUserInformation().UserId;
            SimpleSocketManager.Instance.JoinPrivateChat(userId);

            //Initialize Current Room
            CurrentRoom.Instance.InitRoom(incomingEventJoinRoom.ownerUserId, roomInformation, incomingEventJoinRoom.eventType);

            //Create Users
            OnJoinRoomUsers(incomingEventJoinRoom.ListAvatarData);
            //Create Furnitures
            OnJoinRoomFurnitures(incomingEventJoinRoom.ListFurnitureData);

            CurrentRoom.Instance.SetCameras();
            StartCoroutine(LoaderFadeOut());
        }
        else
        {
            StartCoroutine(GoToMyHouseCoroutine());
            StartCoroutine(LoaderFadeOut());
        }
    }

    IEnumerator GoToMyHouseCoroutine()
    {
        Injection.Get<IGameData>().SetRoomInformation(gameData.GetMyHouseInformation());
        SceneManager.LoadScene(GameScenes.GoToRoomScene, LoadSceneMode.Additive);
        yield return new WaitForEndOfFrame();
        SceneManager.UnloadSceneAsync(GameScenes.RoomScene);
    }

    IEnumerator LoaderFadeOut()
    {
        yield return new WaitForEndOfFrame();
        loaderPanel.DOFade(0, 1).SetEase(Ease.InSine);
        yield return new WaitForSeconds(1);
        Injection.Get<ILoading>().Unload();
        CurrentRoom.Instance.SetReady(true);
        if (OnRoomReady!=null)
        {
            OnRoomReady();
        }
    }

    void OnJoinRoomUsers(List<AvatarRoomData> listAvatarData)
    {
        foreach (AvatarRoomData avatarRoomData in listAvatarData)
        {
            CurrentRoom.Instance.AvatarsManager.AddAvatar(avatarRoomData);
        }
    }

    void OnJoinRoomFurnitures(List<FurnitureRoomData> ListFurnitureData)
    {
        foreach (FurnitureRoomData furnitureData in ListFurnitureData)
        {
            CurrentRoom.Instance.FurnituresRoomManager.AddFurniture(furnitureData);
        }
    }

    private void OnDestroy()
    {
    }
}

/*
 * Load room with addressables 
 * Data structures 
 * Managers
 * Place avatar, move avatar
 * 
 * Refactor Socket Manager
 * System for Suscribe on events on Socket Manager
 * 
 * Integrate socket manager to join/leaveRoom /user enter/user leave
 * 
 * 
 * Integrate pathfinding
 * Implement Camera system
 * 
 * Change system to new Backend
 * Refactor RoomList modal
 * FriendList/Chat small modifications
 * 
 * Chair system
 * Music
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * DELETE
 * UIStoreFurnituresManager
 * MenusSwitcher
 * namespace WorldAreasCanvas
 * 
 * 
 * 
 * -Gems container is not updating
 * -Select image on selected object
 * -Select Image un selected furniture on store
 * -Update Furniture From room
 * -When complete editing -> Open Store
 * 
 * -Bug Gus when buying furniture
 * -Show gems while buying
 * -Add back button
 * 
 * -Detect if we can place furniture because of pathfinding
 * -BUG on SetCameras when registering User
 * 
 * 
 * 
 */
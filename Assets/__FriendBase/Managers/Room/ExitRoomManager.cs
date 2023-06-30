using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using AvatarCustomization;
using Data;
using Data.Rooms;
using Socket;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitRoomManager 
{
    public enum TYPE_EXIT_ROOM { GOTO_AVATAR_CUSTOMIZATION, GOTO_PRIVATE_ROOM, GOTO_PUBLIC_ROOM, GOTO_MINIGAMES, LINK_GUEST_USER }

    public delegate void ExitRoom();
    public event ExitRoom OnExitRoom;

    private RoomInformation currentRoomInformation;
    private CurrentRoom currentRoom;
    private TYPE_EXIT_ROOM typeExitRoom;
    private RoomInformation roomInformationToGo;
    private GameObject loader;

    public ExitRoomManager(CurrentRoom currentRoom, RoomInformation roomInformation, GameObject loader)
    {
        this.currentRoom = currentRoom;
        this.currentRoomInformation = roomInformation;
        this.loader = loader;

        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.LEAVE_ROOM, OnLeaveRoom);
    }

    public void Destroy()
    {
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.LEAVE_ROOM, OnLeaveRoom);
    }

    public void GoToLinkProviderScene()
    {
        string roomName = currentRoomInformation.RoomName;
        string roomId = currentRoomInformation.RoomIdInstance;

        typeExitRoom = TYPE_EXIT_ROOM.LINK_GUEST_USER;

        SimpleSocketManager.Instance.LeaveChatRoom(roomName, roomId);
    }

    public void GoToAvatarCustomization()
    {
        string roomName = currentRoomInformation.RoomName;
        string roomId = currentRoomInformation.RoomIdInstance;

        typeExitRoom = TYPE_EXIT_ROOM.GOTO_AVATAR_CUSTOMIZATION;

        SimpleSocketManager.Instance.LeaveChatRoom(roomName, roomId);
    }
    public void GoToMinigames()
    {
        string roomName = currentRoomInformation.RoomName;
        string roomId = currentRoomInformation.RoomIdInstance;

        typeExitRoom = TYPE_EXIT_ROOM.GOTO_MINIGAMES;

        SimpleSocketManager.Instance.LeaveChatRoom(roomName, roomId);
    }

    public void GoToOtheRoom(RoomInformation roomInformationToGo)
    {
        string roomName = currentRoomInformation.RoomName;
        string roomId = currentRoomInformation.RoomIdInstance;

        typeExitRoom = TYPE_EXIT_ROOM.GOTO_PRIVATE_ROOM;

        this.roomInformationToGo = roomInformationToGo;

        SimpleSocketManager.Instance.LeaveChatRoom(roomName, roomId);
    }

    void OnLeaveRoom(AbstractIncomingSocketEvent incomingEvent)
    {
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.LEAVE_ROOM, OnLeaveRoom);

        IncomingEventLeaveRoom incomingEventLeaveRoom = incomingEvent as IncomingEventLeaveRoom;
        if (incomingEventLeaveRoom != null && incomingEventLeaveRoom.State == SocketEventResult.OPERATION_SUCCEED)
        {
            if (OnExitRoom!=null)
            {
                OnExitRoom();
            }
            switch (typeExitRoom)
            {
                case TYPE_EXIT_ROOM.GOTO_AVATAR_CUSTOMIZATION:
                    Injection.Get<ILoading>().Load();
                    currentRoom.StartCoroutine(GoToAvatarCustomizationCoroutine());
                    break;
                case TYPE_EXIT_ROOM.GOTO_PRIVATE_ROOM:
                case TYPE_EXIT_ROOM.GOTO_PUBLIC_ROOM:
                    Injection.Get<ILoading>().Load();
                    currentRoom.StartCoroutine(GoToOtherRoomCoroutine());
                    break;
                case TYPE_EXIT_ROOM.GOTO_MINIGAMES:
                    Injection.Get<ILoading>().Load();
                    currentRoom.StartCoroutine(GoToMinigamesCoroutine());
                    break;
                case TYPE_EXIT_ROOM.LINK_GUEST_USER:
                    Injection.Get<ILoading>().Load();
                    currentRoom.StartCoroutine(GoToLinkProviderCoroutine());
                    break;
            }
        }
    }

    public void ExitBannedScene()
    {
        currentRoom.StartCoroutine(GoToBannedScene());
        if (OnExitRoom != null)
        {
            OnExitRoom();
        }
    }

    IEnumerator GoToBannedScene()
    {
        SceneManager.LoadScene(GameScenes.BannedScene, LoadSceneMode.Additive);

        yield return new WaitForEndOfFrame();
        SceneManager.UnloadSceneAsync(GameScenes.RoomScene);
    }

    public void ExitError()
    {
        currentRoom.StartCoroutine(GoToAvatarCustomizationCoroutine());
        if (OnExitRoom != null)
        {
            OnExitRoom();
        }
    }

    IEnumerator GoToMinigamesCoroutine()
    {
        //Injection.Get<IViewManager>().Show<AvatarCustomizationPanel>();
        SceneManager.LoadScene(GameScenes.Minigames, LoadSceneMode.Additive);

        yield return new WaitForEndOfFrame();
        SceneManager.UnloadSceneAsync(GameScenes.RoomScene);
    }

    IEnumerator GoToAvatarCustomizationCoroutine()
    {
        //Injection.Get<IViewManager>().Show<AvatarCustomizationPanel>();
        SceneManager.LoadScene(GameScenes.AvatarCustomization, LoadSceneMode.Additive);

        yield return new WaitForEndOfFrame();
        SceneManager.UnloadSceneAsync(GameScenes.RoomScene);
    }

    IEnumerator GoToOtherRoomCoroutine()
    {
        Injection.Get<IGameData>().SetRoomInformation(this.roomInformationToGo);
        SceneManager.LoadScene(GameScenes.GoToRoomScene, LoadSceneMode.Additive);
        yield return new WaitForEndOfFrame();
        SceneManager.UnloadSceneAsync(GameScenes.RoomScene);
    }

    IEnumerator GoToLinkProviderCoroutine()
    {
        NauthFlowManager.SetStateAuthFlow(NauthFlowManager.STATE_AUTH_FLOW.LINK_PROVIDER);
        
        SceneManager.LoadScene(GameScenes.NewAuthFlow, LoadSceneMode.Additive);
        yield return new WaitForEndOfFrame();
        SceneManager.UnloadSceneAsync(GameScenes.RoomScene);
    }
}
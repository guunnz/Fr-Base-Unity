using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Socket;
using UnityEngine;
using DebugConsole;

public class AvatarsManager
{
    private List<AvatarRoomController> listAvatars;
    private GameObject avatarsContainer;
    private GameObject destinationPointsContainer;
    private AvatarRoomController avatarPrefab;

    private IGameData gameData;
    private IDebugConsole debugConsole;

    public delegate void UserKicked();
    public event UserKicked OnUserKicked;

    public AvatarsManager(AvatarRoomController avatarPrefab, GameObject avatarsContainer, GameObject destinationPointsContainer)
    {
        this.avatarPrefab = avatarPrefab;
        this.avatarsContainer = avatarsContainer;
        this.destinationPointsContainer = destinationPointsContainer;
        listAvatars = new List<AvatarRoomController>();
        gameData = Injection.Get<IGameData>();
        debugConsole = Injection.Get<IDebugConsole>();

        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.AVATAR_MOVE, OnAvatarMove);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.USER_LEAVE, OnUserLeaveRoom);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.USER_ENTER, OnUserEnterRoom);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.AVATAR_STATE, OnUserStateChange);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.PET_INFO, OnPetInfo);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.PRIVATE_CHAT_MESSAGE, OnPrivateChatReceive);
    }

    public List<AvatarRoomController> GetListAvatars()
    {
        return listAvatars;
    }

    public AvatarRoomController AddAvatar(AvatarRoomData avatarData)
    {
        try
        {
            AvatarRoomController avatar = GetAvatarById(avatarData.FirebaseId);

            if (gameData.GetUserInformation().GetBlockedPlayers().Contains(int.Parse(avatarData.UserId)))
            {
                return null;
            }

            if (avatar != null)
            {
                debugConsole.ErrorLog("AvatarsManager:AddAvatar", "User already created", "userId:" + avatarData.FirebaseId);
                return null;
            }

            bool isLocalPlayer = avatarData.FirebaseId == gameData.GetUserInformation().FirebaseId;

            AvatarRoomController avatarRoomController = UnityEngine.Object.Instantiate(avatarPrefab, Vector3.zero, avatarPrefab.transform.rotation);
            avatarRoomController.name = "Avatar_" + avatarData.Username;
            avatarRoomController.transform.SetParent(avatarsContainer.transform, true);
            avatarRoomController.Init(avatarData, destinationPointsContainer, isLocalPlayer);
            Debug.Log(avatarRoomController.gameObject.activeSelf);
            avatarRoomController.SetState(avatarData.AvatarState, avatarData.Positionx, avatarData.Positiony, useSocket: false);
            listAvatars.Add(avatarRoomController);
            avatarRoomController.avatarPetManager.SetPet(avatarData.CurrentPetId, avatarData.PetIdInGame, false, PetPrefabName: avatarData.PetPrefabName);
            return avatarRoomController;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        return null;
    }

    private void OnPrivateChatReceive(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventPrivateChatMessage incomingEventPrivateChat = incomingEvent as IncomingEventPrivateChatMessage;

        if (incomingEventPrivateChat != null && incomingEventPrivateChat.State == SocketEventResult.OPERATION_SUCCEED)
        {
            //Discard my user 
            if (!incomingEventPrivateChat.ChatMessageData.firebaseUid.Equals(gameData.GetUserInformation().FirebaseId))
            {
                AvatarRoomController avatar = GetAvatarById(incomingEventPrivateChat.ChatMessageData.firebaseUid);

                bool flagIsChatOpen = CurrentRoom.Instance.GetRoomUIReferences().Chat.IsChatOpen(incomingEventPrivateChat.ChatMessageData.firebaseUid);

                if (avatar != null && !flagIsChatOpen)
                {
                    avatar.AvatarNotificationController.ShowGreenPrivateChat();
                }
            }
        }
    }

    private void OnAvatarMove(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventMoveAvatar incomingEventMoveAvatar = incomingEvent as IncomingEventMoveAvatar;

        if (incomingEventMoveAvatar != null && incomingEventMoveAvatar.State == SocketEventResult.OPERATION_SUCCEED)
        {

            //Discard my user as we move it locally
            if (!incomingEventMoveAvatar.FirebaseId.Equals(gameData.GetUserInformation().FirebaseId))
            {
                AvatarRoomController avatar = GetAvatarById(incomingEventMoveAvatar.FirebaseId);
                if (avatar != null)
                {
                    avatar.SetWalkToDestination(incomingEventMoveAvatar.Destinationx, incomingEventMoveAvatar.Destinationy);
                }
            }
        }
    }

    public bool RemoveAvatar(string idFirebase)
    {
        int amountAvatars = listAvatars.Count;
        for (int i = amountAvatars - 1; i >= 0; i--)
        {
            if (listAvatars[i].AvatarData.FirebaseId.Equals(idFirebase))
            {
                AvatarRoomController currentAvatar = listAvatars[i];
                currentAvatar.DestroyAvatar();
                listAvatars.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public AvatarRoomController GetMyAvatar()
    {
        string idFirebase = gameData.GetUserInformation().FirebaseId;
        return GetAvatarById(idFirebase);
    }

    public AvatarRoomController GetAvatarById(string idFirebase)
    {
        foreach (AvatarRoomController avatar in listAvatars)
        {
            if (avatar.AvatarData.FirebaseId.Equals(idFirebase))
            {
                return avatar;
            }
        }
        return null;
    }

    public AvatarRoomController GetAvatarByUserId(string idUser)
    {
        foreach (AvatarRoomController avatar in listAvatars)
        {
            if (avatar.AvatarData.UserId.Equals(idUser))
            {
                return avatar;
            }
        }
        return null;
    }


    public void Destroy()
    {
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.AVATAR_MOVE, OnAvatarMove);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.USER_ENTER, OnUserEnterRoom);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.USER_LEAVE, OnUserLeaveRoom);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.AVATAR_STATE, OnUserStateChange);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.PET_INFO, OnPetInfo);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.PRIVATE_CHAT_MESSAGE, OnPrivateChatReceive);
        foreach (AvatarRoomController avatar in listAvatars)
        {
            avatar.DestroyAvatar();
        }
    }

    private void OnUserEnterRoom(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventUserEnter incomingEventUserEnter = incomingEvent as IncomingEventUserEnter;
        if (incomingEventUserEnter != null && incomingEventUserEnter.State == SocketEventResult.OPERATION_SUCCEED)
        {
            AddAvatar(incomingEventUserEnter.AvatarData);
        }
    }

    private void OnUserStateChange(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventChangePlayerState incomingEventChangePlayerState = incomingEvent as IncomingEventChangePlayerState;
        if (incomingEventChangePlayerState != null && incomingEventChangePlayerState.State == SocketEventResult.OPERATION_SUCCEED)
        {

            if (!incomingEventChangePlayerState.FirebaseId.Equals(gameData.GetUserInformation().FirebaseId))
            {
                AvatarRoomController avatar = GetAvatarById(incomingEventChangePlayerState.FirebaseId);
                if (avatar != null)
                {
                    avatar.SetState(incomingEventChangePlayerState.AvatarState, incomingEventChangePlayerState.PositionX, incomingEventChangePlayerState.PositionY, incomingEventChangePlayerState.Orientation, false);
                }
            }
        }
    }

    private void OnPetInfo(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventNewPetInfo incomingEventPetInfo = incomingEvent as IncomingEventNewPetInfo;
        if (incomingEventPetInfo != null && incomingEventPetInfo.State == SocketEventResult.OPERATION_SUCCEED)
        {
            if (incomingEventPetInfo.UserId != gameData.GetUserInformation().UserId.ToString())
            {
                AvatarRoomController avatar = GetAvatarByUserId(incomingEventPetInfo.UserId);
                if (avatar != null)
                {
                    avatar.avatarPetManager.SetPet(incomingEventPetInfo.PetIdInGame, incomingEventPetInfo.PetId, false, PetPrefabName: incomingEventPetInfo.PetPrefabName);
                }
            }
        }
    }

    void OnUserLeaveRoom(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventUserLeave incomingEventUserLeave = incomingEvent as IncomingEventUserLeave;
        if (incomingEventUserLeave != null && incomingEventUserLeave.State == SocketEventResult.OPERATION_SUCCEED)
        {
            if (!incomingEventUserLeave.FirebaseId.Equals(gameData.GetUserInformation().FirebaseId))
            {
                RemoveAvatar(incomingEventUserLeave.FirebaseId);
            }
            else
            {
                //My Avatar => I was Kicked from the room
                if (OnUserKicked != null)
                {
                    OnUserKicked();
                }
            }
        }
    }
}

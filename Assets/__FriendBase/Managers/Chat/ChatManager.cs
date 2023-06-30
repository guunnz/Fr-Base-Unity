using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using ChatView.Core.Domain;
using Data;
using Socket;
using UnityEngine;

public class ChatManager
{
    public enum ChatType
    {
        Public,
        Private
    };

    private List<UserChatData> publicChatData;
    private Dictionary<string, PrivateChatData> privateChatData;

    public delegate void NewPublicChat(UserChatData userChatData);

    public event NewPublicChat OnNewPublicChat;

    public delegate void NewPrivateChat(string idUser, string channelUserId, UserChatData userChatData);

    public event NewPrivateChat OnNewPrivateChat;

    public delegate void AddUserToPrivateChat(AvatarRoomData AvatarData);

    public event AddUserToPrivateChat OnAddUserToPrivateChat;

    public delegate void AddManualUserToPrivateChat(AvatarRoomData AvatarData);

    public event AddManualUserToPrivateChat OnAddManualUserToPrivateChat;
    
    public delegate void RemoveUserFromPrivateChat(string idUser);

    public event RemoveUserFromPrivateChat OnRemoveUserFromPrivateChat;

    public delegate void ShowPublicChat();

    public event ShowPublicChat OnShowPublicChat;

    public delegate void HidePublicChat();

    public event HidePublicChat OnHidePublicChat;

    public delegate void SetPublicChatIcon();

    public event SetPublicChatIcon OnSetPublicChatIcon;

    public delegate void ShowPrivateChat(string idUser);

    public event ShowPrivateChat OnShowPrivateChat;

    public delegate void HidePrivateChat();

    public event HidePrivateChat OnHidePrivateChat;

    public delegate void ExitRoom();

    public event ExitRoom OnExitRoom;

    public ChatType ChatSelected { get; private set; }
    public bool IsChatOpen { get; private set; }
    public string PrivateUserIdSelected { get; private set; }

    private IGameData gameData;
    private string myUserId;



    public string GetCurrentPrivateChatSelected()
    {
        return PrivateUserIdSelected;
    }

    public ChatManager()
    {
        publicChatData = new List<UserChatData>();
        privateChatData = new Dictionary<string, PrivateChatData>();

        ChatSelected = ChatType.Public;
        IsChatOpen = false;

        gameData = Injection.Get<IGameData>();

        myUserId = gameData.GetUserInformation().UserId.ToString();

        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.CHAT_MESSAGE, OnNewMessage);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.PRIVATE_CHAT_MESSAGE, OnNewPrivateMessage);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.USER_LEAVE, OnUserLeaveMessage);
    }

    public void OpenPublicChat()
    {
        ChatSelected = ChatType.Public;
        IsChatOpen = true;
        
        if (OnShowPublicChat != null)
        {
            OnShowPublicChat();
        }
    }

    public void ClosePublicChat()
    {
        if (OnHidePublicChat != null)
        {
            OnHidePublicChat();
        }

        ChatSelected = ChatType.Public;
        IsChatOpen = false;
    }

    public void OpenPrivateChat(string idUser)
    {
        ChatSelected = ChatType.Private;
        IsChatOpen = true;
        PrivateUserIdSelected = idUser;

        if (privateChatData.ContainsKey(idUser))
        {
            privateChatData[idUser].HasUnreadMessages = false;
        }

        if (OnShowPrivateChat != null)
        {
            OnShowPrivateChat(idUser);
        }
    }

    public void ClosePrivateChat()
    {
        if (OnHidePrivateChat != null)
        {
            OnHidePrivateChat();
        }

        ChatSelected = ChatType.Private;
        IsChatOpen = false;
    }

    public void SetPublicChatIconActive()
    {
        if (OnSetPublicChatIcon != null)
        {
            OnSetPublicChatIcon();
        }

        ChatSelected = ChatType.Public;
    }

    private void OnUserLeaveMessage(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventUserLeave incomingEventUserLeave = incomingEvent as IncomingEventUserLeave;
        if (incomingEventUserLeave == null || incomingEventUserLeave.State != SocketEventResult.OPERATION_SUCCEED)
        {
            return;
        }

        AvatarRoomController avatarRoomController =
            CurrentRoom.Instance.AvatarsManager.GetAvatarById(incomingEventUserLeave.FirebaseId);
        if (avatarRoomController == null)
        {
            return;
        }

        string idUser = avatarRoomController.AvatarData.UserId;
        if (privateChatData.ContainsKey(idUser))
        {
            privateChatData.Remove(idUser);
            if (OnRemoveUserFromPrivateChat != null)
            {
                OnRemoveUserFromPrivateChat(idUser);
            }
        }
    }

    private void OnNewMessage(AbstractIncomingSocketEvent incomingEvent)
    {
        //Receive Public Chat
        IncomingEventChatMessage incomingEventChatMessage = incomingEvent as IncomingEventChatMessage;
        if (incomingEventChatMessage == null || incomingEventChatMessage.State != SocketEventResult.OPERATION_SUCCEED)
        {
            return;
        }

        AvatarRoomController avatarRoomController =
            CurrentRoom.Instance.AvatarsManager.GetAvatarById(incomingEventChatMessage.ChatMessageData.firebaseUid);
        if (avatarRoomController == null)
        {
            return;
        }

        ChatData chatData = incomingEventChatMessage.ChatMessageData;
        bool isMe = avatarRoomController.AvatarData.UserId.Equals(myUserId);
        UserChatData userChatData = new UserChatData(avatarRoomController.AvatarData, chatData.message, isMe);
        publicChatData.Add(userChatData);

        if (OnNewPublicChat != null)
        {
            OnNewPublicChat(userChatData);
        }
    }

    public List<UserChatData> GetPublicChatData()
    {
        return publicChatData;
    }

    public void StartPrivateChatFromCard(string userId)
    {
        AvatarRoomController avatarRoomController = CurrentRoom.Instance.AvatarsManager.GetAvatarByUserId(userId);
        if (avatarRoomController == null)
        {
            return;
        }

        //Get the channel User Id (Not me)
        string channelUserId = userId;
        if (!privateChatData.ContainsKey(channelUserId))
        {
            privateChatData.Add(channelUserId,
                new PrivateChatData(avatarRoomController.AvatarData, new List<UserChatData> { }));
        }
        if (OnAddManualUserToPrivateChat != null)
        {
            OnAddManualUserToPrivateChat(avatarRoomController.AvatarData);
        }
    }

    private void OnNewPrivateMessage(AbstractIncomingSocketEvent incomingEvent)
    {
        //Receive Private Chat
        IncomingEventPrivateChatMessage incomingEventChatMessage = incomingEvent as IncomingEventPrivateChatMessage;
        if (incomingEventChatMessage == null || incomingEventChatMessage.State != SocketEventResult.OPERATION_SUCCEED)
        {
            return;
        }

        AvatarRoomController avatarRoomController =
            CurrentRoom.Instance.AvatarsManager.GetAvatarById(incomingEventChatMessage.ChatMessageData.firebaseUid);

        // PrivateUserIdSelected = avatarRoomController.AvatarData.UserId;
        
        if (avatarRoomController == null)
        {
            return;
        }

        ChatData chatData = incomingEventChatMessage.ChatMessageData;
        bool isMe = myUserId.Equals(incomingEventChatMessage.SenderUserId);
        UserChatData userChatData = new UserChatData(avatarRoomController.AvatarData, chatData.message, isMe,true);
        string idUser = avatarRoomController.AvatarData.UserId;


        //Get the channel User Id (Not me)
        string channelUserId = incomingEventChatMessage.SenderUserId;
        if (isMe)
        {
            channelUserId = incomingEventChatMessage.ReceiverUserId;
        }

        if (!privateChatData.ContainsKey(channelUserId))
        {
            privateChatData.Add(channelUserId,
                new PrivateChatData(avatarRoomController.AvatarData, new List<UserChatData> { userChatData }));
            if (OnAddUserToPrivateChat != null)
            {
                OnAddUserToPrivateChat(avatarRoomController.AvatarData);
            }
        }
        else
        {
            privateChatData[channelUserId].AddChat(userChatData);
        }

        //Check Unread Message
        if (ChatSelected == ChatType.Public || !IsChatOpen || !channelUserId.Equals(PrivateUserIdSelected))
        {
            privateChatData[channelUserId].HasUnreadMessages = true;
        }

        if (OnNewPrivateChat != null)
        {
            OnNewPrivateChat(idUser, channelUserId, userChatData);
        }
    }

    public PrivateChatData GetPrivateChatData(string userId)
    {
        if (privateChatData.ContainsKey(userId))
        {
            return privateChatData[userId];
        }

        return null;
    }

    public List<AvatarRoomData> GetPrivateChatUsers()
    {
        List<AvatarRoomData> listUsers = new List<AvatarRoomData>();

        foreach (KeyValuePair<string, PrivateChatData> privateChat in privateChatData)
        {
            listUsers.Add(privateChat.Value.AvatarData);
        }

        return listUsers;
    }

    public void Destroy()
    {
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.CHAT_MESSAGE, OnNewMessage);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.PRIVATE_CHAT_MESSAGE, OnNewPrivateMessage);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.USER_LEAVE, OnUserLeaveMessage);
        if (OnExitRoom != null)
        {
            OnExitRoom();
        }
    }
}
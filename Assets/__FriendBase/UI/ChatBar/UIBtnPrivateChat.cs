using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Data.Users;
using UnityEngine;
using UnityEngine.UI;

public class UIBtnPrivateChat : MonoBehaviour
{
    public enum PrivateState { EMPTY, CLOSE, OPEN, ACTIVE_AVATAR };

    [SerializeField] private Transform containerCards;
    [SerializeField] private Sprite iconClose;
    [SerializeField] private Sprite iconClosePressed;
    [SerializeField] private Sprite iconOpen;
    [SerializeField] private Sprite iconOpenPressed;
    [SerializeField] private Sprite iconDisable;
    [SerializeField] private Button buttonOpenPrivateChat;
    [SerializeField] private UIPrivateChatAvatar cardActiveAvatar;

    private Image iconImg;
    private UIPrivateChatAvatarPool privateChatPool;
    private IGameData gameData;
    private PrivateState privateState;
    private AvatarRoomData avatarDataSelected;
    private ChatManager chatManager;

    void Start()
    {
        gameData = Injection.Get<IGameData>();
        privateChatPool = GetComponent<UIPrivateChatAvatarPool>();
        iconImg = buttonOpenPrivateChat.GetComponent<Image>();

        privateState = PrivateState.EMPTY;
        RefreshState();
        
        RoomJoinManager.OnRoomReady += OnRoomReady;
    }

    void OnRoomReady()
    {
        chatManager = CurrentRoom.Instance.chatManager;
        chatManager.OnExitRoom += OnExitRoom;
        chatManager.OnAddUserToPrivateChat += OnAddUserToPrivateChat;
        chatManager.OnShowPublicChat += OnShowPublicChat;
        chatManager.OnRemoveUserFromPrivateChat += OnRemoveUserFromPrivateChat;
        chatManager.OnNewPrivateChat += OnNewPrivateChat;
        chatManager.OnAddManualUserToPrivateChat += OnAddManualUserToPrivateChat;
    }

    void OnExitRoom()
    {
        RoomJoinManager.OnRoomReady -= OnRoomReady;
        chatManager.OnExitRoom -= OnExitRoom;
        chatManager.OnAddUserToPrivateChat -= OnAddUserToPrivateChat;
        chatManager.OnShowPublicChat -= OnShowPublicChat;
        chatManager.OnRemoveUserFromPrivateChat -= OnRemoveUserFromPrivateChat;
        chatManager.OnNewPrivateChat -= OnNewPrivateChat;
        chatManager.OnAddManualUserToPrivateChat -= OnAddManualUserToPrivateChat;
    }

    void OnNewPrivateChat(string idUser, string channelUserId, UserChatData userChatData)
    {
        switch (privateState)
        {
            case PrivateState.EMPTY:
            case PrivateState.CLOSE:
                break;
            case PrivateState.OPEN:
                int childs = containerCards.transform.childCount;
                for (int i = childs - 1; i >= 0; i--)
                {
                    containerCards.transform.GetChild(i).GetComponent<UIPrivateChatAvatar>().CheckUnreadMessages(chatManager);
                }
                break;
            case PrivateState.ACTIVE_AVATAR:
                if (cardActiveAvatar != null && cardActiveAvatar.AvatarRoomData.UserId.Equals(idUser))
                {
                    cardActiveAvatar.CheckUnreadMessages(chatManager);
                }
                break;
        }
    }

    private void RefreshState()
    {
        SpriteState sprState = new SpriteState();
        sprState.disabledSprite = iconDisable;
        switch (privateState)
        {
            case PrivateState.EMPTY:
                avatarDataSelected = null;
                buttonOpenPrivateChat.gameObject.SetActive(true);
                buttonOpenPrivateChat.interactable = false;
                cardActiveAvatar.gameObject.SetActive(false);
                CleanAvatars();
                break;
            case PrivateState.CLOSE:
                avatarDataSelected = null;
                buttonOpenPrivateChat.gameObject.SetActive(true);
                buttonOpenPrivateChat.interactable = true;
                cardActiveAvatar.gameObject.SetActive(false);
                CleanAvatars();
                //Change sprites
                iconImg.sprite = iconClose;
                sprState.pressedSprite = iconClosePressed;
                buttonOpenPrivateChat.spriteState = sprState;
                break;
            case PrivateState.OPEN:
                buttonOpenPrivateChat.gameObject.SetActive(true);
                buttonOpenPrivateChat.interactable = true;
                cardActiveAvatar.gameObject.SetActive(false);
                //Change sprites
                iconImg.sprite = iconOpen;
                sprState.pressedSprite = iconOpenPressed;
                buttonOpenPrivateChat.spriteState = sprState;
                break;
            case PrivateState.ACTIVE_AVATAR:
                CleanAvatars();
                buttonOpenPrivateChat.gameObject.SetActive(false);
                cardActiveAvatar.gameObject.SetActive(true);
                cardActiveAvatar.SetUpCard(avatarDataSelected, CallbackPressSelectedAvatar, chatManager);
                break;
        }
    }

    void CallbackPressSelectedAvatar(UIPrivateChatAvatar cardAvatar)
    {
        OnClickOpenPrivateChat();
    }

    void OnRemoveUserFromPrivateChat(string idUser)
    {
        switch (privateState)
        {
            case PrivateState.EMPTY:
                break;
            case PrivateState.CLOSE:
                if (chatManager.GetPrivateChatUsers().Count == 0)
                {
                    privateState = PrivateState.EMPTY;
                }
                break;
            case PrivateState.OPEN:
                if (chatManager.GetPrivateChatUsers().Count == 0)
                {
                    privateState = PrivateState.EMPTY;
                    chatManager.SetPublicChatIconActive();
                }
                else
                {
                    //if (avatarDataSelected.UserId.Equals(idUser))
                    //{
                    //    avatarDataSelected = null;
                    //    chatManager.SetPublicChatIconActive();
                    //}

                    UIPrivateChatAvatar card = GetCardByIdUser(idUser);
                    if (card!=null)
                    {
                        privateChatPool.ReturnToPool(card);
                        ReasignIndexes();
                    }
                }
                break;
            case PrivateState.ACTIVE_AVATAR:
                if (chatManager.GetPrivateChatUsers().Count == 0)
                {
                    privateState = PrivateState.EMPTY;
                    chatManager.SetPublicChatIconActive();
                }
                else if (avatarDataSelected.UserId.Equals(idUser))
                {
                    privateState = PrivateState.CLOSE;
                    if (chatManager.ChatSelected == ChatManager.ChatType.Private)
                    {
                        chatManager.SetPublicChatIconActive();
                    }
                }
                break;
        }
        RefreshState();
    }

    UIPrivateChatAvatar GetCardByIdUser(string idUser)
    {
        int childs = containerCards.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            UIPrivateChatAvatar card = containerCards.transform.GetChild(i).GetComponent<UIPrivateChatAvatar>();
            if (card.AvatarRoomData.UserId.Equals(idUser))
            {
                return card;
            }
        }
        return null;
    }

    void ReasignIndexes()
    {
        int childs = containerCards.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            containerCards.transform.GetChild(i).GetComponent<UIPrivateChatAvatar>().ReasignIndex(i);
        }
    }

    void OnAddUserToPrivateChat(AvatarRoomData avatarData)
    {
        //Receive Callback when we have a new user on Private Chat
        if (privateState == PrivateState.EMPTY)
        {
            privateState = PrivateState.CLOSE;
            RefreshState();
        }
        else if (privateState == PrivateState.OPEN)
        {
            AddPrivateChatCard(avatarData);
        }
    }

    void OnShowPublicChat()
    {
        //Receive Callback when public chat is Open
        ClosePrivateChat();
    }

    void ClosePrivateChat()
    {
        switch (privateState)
        {
            case PrivateState.EMPTY:
            case PrivateState.CLOSE:
                break;
            case PrivateState.OPEN:
                privateState = PrivateState.CLOSE;
                break;
            case PrivateState.ACTIVE_AVATAR:
                break;
        }
        RefreshState();
    }

    public void OnClickOpenPrivateChat()
    {
        switch (privateState)
        {
            case PrivateState.EMPTY:
                break;
            case PrivateState.CLOSE:
                privateState = PrivateState.OPEN;
                CreateAvatars();
                break;
            case PrivateState.OPEN:
                if (avatarDataSelected == null)
                {
                    chatManager.ClosePrivateChat();
                    privateState = PrivateState.CLOSE;
                    CleanAvatars();
                }
                else
                {
                    avatarDataSelected = null;
                    chatManager.ClosePrivateChat();
                    privateState = PrivateState.CLOSE;
                    CleanAvatars();

                    //privateState = PrivateState.ACTIVE_AVATAR;
                }
                break;
            case PrivateState.ACTIVE_AVATAR:
                if (chatManager.ChatSelected == ChatManager.ChatType.Public)
                {
                    chatManager.OpenPrivateChat(avatarDataSelected.UserId);
                }
                else
                {
                    privateState = PrivateState.OPEN;
                    CreateAvatars();
                }
                break;
        }
        RefreshState();
    }

    private void CleanAvatars()
    {
        int childs = containerCards.transform.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            privateChatPool.ReturnToPool(containerCards.transform.GetChild(i).GetComponent<UIPrivateChatAvatar>());
        }
        containerCards.gameObject.SetActive(false);
    }

    private void CreateAvatars()
    {
        CleanAvatars();
        containerCards.gameObject.SetActive(true);
        List<AvatarRoomData> listAvatarData = chatManager.GetPrivateChatUsers();
        foreach(AvatarRoomData avatarRoomData in listAvatarData)
        {
            AddPrivateChatCard(avatarRoomData);
        }
    }

    public void AddPrivateChatCard(AvatarRoomData avatarRoomData)
    {
        UIPrivateChatAvatar privateChatCard = privateChatPool.Get();
        privateChatCard.transform.SetParent(containerCards);
        privateChatCard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        privateChatCard.transform.localScale = Vector3.one * 0.8f;
        int index = privateChatCard.transform.GetSiblingIndex();
        privateChatCard.SetUpCard(index, avatarRoomData, CallbackCardPressed, chatManager);
    }

    void CallbackCardPressed(UIPrivateChatAvatar card)
    {
        avatarDataSelected = card.AvatarRoomData;
        privateState = PrivateState.ACTIVE_AVATAR;
        chatManager.OpenPrivateChat(avatarDataSelected.UserId);
        RefreshState();
    }

    void OnAddManualUserToPrivateChat(AvatarRoomData avatarData)
    {
        avatarDataSelected = avatarData;
        privateState = PrivateState.ACTIVE_AVATAR;
        chatManager.OpenPrivateChat(avatarDataSelected.UserId);
        RefreshState();
    }
}

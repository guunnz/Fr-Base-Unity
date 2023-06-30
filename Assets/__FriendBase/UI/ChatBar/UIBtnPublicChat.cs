using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBtnPublicChat : MonoBehaviour
{
    [SerializeField] private Sprite iconClose;
    [SerializeField] private Sprite iconClosePressed;
    [SerializeField] private Sprite iconOpen;
    [SerializeField] private Sprite iconOpenPressed;

    private Image iconImg;
    private Button buttonOpenChat;
    private ChatManager chatManager;

    void Start()
    {
        iconImg = GetComponent<Image>();
        buttonOpenChat = GetComponent<Button>();
        RoomJoinManager.OnRoomReady += OnRoomReady;
    }

    void OnRoomReady()
    {
        chatManager = CurrentRoom.Instance.chatManager;
        chatManager.OnExitRoom += OnExitRoom;
        chatManager.OnShowPrivateChat += OnShowPrivateChat;
    }

    void OnExitRoom()
    {
        RoomJoinManager.OnRoomReady -= OnRoomReady;
        chatManager.OnShowPrivateChat -= OnShowPrivateChat;
    }

    void OnShowPrivateChat(string idUser)
    {
        //Close public icon if it is open
        RefreshIcon();
    }

    public void OnClickButtonPublicChat()
    {
        if (chatManager.ChatSelected == ChatManager.ChatType.Private)
        {
            chatManager.OpenPublicChat();
        }
        else if (chatManager.ChatSelected == ChatManager.ChatType.Public)
        {
            if (chatManager.IsChatOpen)
            {
                chatManager.ClosePublicChat();
            }
            else
            {
                chatManager.OpenPublicChat();
            }
        }

        RefreshIcon();
    }

    void RefreshIcon()
    {
        SpriteState sprState = new SpriteState();
        if (chatManager.ChatSelected == ChatManager.ChatType.Private || !chatManager.IsChatOpen)
        {
            iconImg.sprite = iconOpen;
            sprState.pressedSprite = iconOpenPressed;
            buttonOpenChat.spriteState = sprState;
        }
        else
        {
            iconImg.sprite = iconClose;
            sprState.pressedSprite = iconClosePressed;
            buttonOpenChat.spriteState = sprState;
        }
    }
}

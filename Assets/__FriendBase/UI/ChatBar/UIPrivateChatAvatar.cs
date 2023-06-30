using System.Collections;
using System.Collections.Generic;
using Data.Users;
using Snapshots;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UIPrivateChatAvatar : MonoBehaviour
{
    [SerializeField] private GameObject greenImgNotification;
    [SerializeField] private Image imgAvatar;
    [SerializeField] private SnapshotAvatar snapshotAvatar;

    public int Index { get; private set; }
    public AvatarRoomData AvatarRoomData { get; private set; }
    private Action<UIPrivateChatAvatar> callbackFunction;
    private ChatManager chatManager;

    void Start()
    {
    }

    public void SetUpCard(AvatarRoomData avatarRoomData, Action<UIPrivateChatAvatar> callbackFunction, ChatManager chatManager)
    {
        this.gameObject.SetActive(true);
        AvatarRoomData = avatarRoomData;
        this.callbackFunction = callbackFunction;

        SetSnapshot();
        CheckUnreadMessages(chatManager);
    }

    public void SetUpCard(int index, AvatarRoomData avatarRoomData, Action<UIPrivateChatAvatar> callbackFunction, ChatManager chatManager)
    {
        Index = index;
        this.gameObject.SetActive(true);
        AvatarRoomData = avatarRoomData;
        this.callbackFunction = callbackFunction;
        this.chatManager = chatManager;

        SetSnapshot();
        CheckUnreadMessages(chatManager);

        this.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 70 * (1+index)), 0.3f);
    }

    public void CheckUnreadMessages(ChatManager chatManager)
    {
        PrivateChatData chatData = chatManager.GetPrivateChatData(AvatarRoomData.UserId);
        if (chatData == null)
        {
            return;
        }
        greenImgNotification.SetActive(chatData.HasUnreadMessages);
    }

    public void ReasignIndex(int newIndex)
    {
        if (newIndex != Index)
        {
            Index = newIndex;
            this.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 70 * (1 + Index)), 0.3f);
        }
    }

    public void SetSnapshot()
    {
        Sprite sprite = CurrentRoom.Instance.RoomSnapshotAvatarManager.GetSnapshot(AvatarRoomData.UserId);
        if (sprite==null)
        {
            snapshotAvatar.CreateSnaphot(CallbackSnapshot, AvatarRoomData.AvatarCustomizationData);
        }
        else
        {
            imgAvatar.sprite = sprite;
        }
    }

    public void CallbackSnapshot(bool flag, int idSnapshot, Sprite sprite)
    {
        if (flag)
        {
            CurrentRoom.Instance.RoomSnapshotAvatarManager.SetSnapshot(AvatarRoomData.UserId, sprite);
        }
    }

    public void OnClickCard()
    {
        callbackFunction(this);
    }
}

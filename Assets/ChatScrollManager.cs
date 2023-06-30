using System;
using Architecture.Injector.Core;
using Data;
using FriendsView.Core.Domain;
using LocalizationSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChatView.Core.Domain;
using Functional.Maybe;
using ResponsiveUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[System.Serializable]
public class ChatUserColors
{
    public string username;
    public Color color;
}

public class ChatScrollManager : MonoBehaviour
{
    [SerializeField] IGameData gameData;
    [SerializeField] GameObject chatRowPrefab;

    [SerializeField] List<ChatRow> chatListRowCards = new List<ChatRow>();
    [SerializeField] List<ChatUserColors> chatColors = new List<ChatUserColors>();
    [SerializeField] List<UserChatData> defaultChat = new List<UserChatData>();
    private ChatManager ChatManager;

    private Color[] colorList = new Color[]
    {
        new Color(83f / 255f, 152f / 255f, 0, 1), new Color(83f / 255f, 152f / 255f, 0, 1),
        new Color(248f / 255f, 105f / 255f, 58f / 255f, 1), new Color(122f / 255f, 22f / 255f, 2f / 255f, 1),
        new Color(180f / 255f, 164f / 255f, 19f / 255f, 1)
    };

    [SerializeField] Transform chatRowContainer;
    [SerializeField] Transform minYMask;
    [SerializeField] Transform maxYMask;

    private int chatCardsAmount = 15;

    public GameObject Container;

    private int DistanceBetweenCards = 73;

    private int ChatRowSize = 63;

    private string lastTrimmed;

    private int maskMargins = 692;

    private ILanguage language;

    [SerializeField] ScrollRect scroll;

    [SerializeField] List<DoSnapshotList> snapshotLists;

    IAvatarEndpoints AvatarEndpoints;

    private List<FriendData> FriendList = new List<FriendData>();

    private float spacing = 2f;

    float maxYOffset = -232f;

    private bool anchoredPositionSet = false;

    RectTransform containerRect;

    private void LateUpdate()
    {
        AdjustSpacing();
    }

    void HideChat()
    {
        Container.SetActive(false);
    }

    void ShowChat()
    {
        Container.SetActive(true);
    }

    public IEnumerator AddChat(UserChatData data, bool isPrivateChat = false, string channelUserId = "")
    {
        if (!data.IsMe)
        {
            if (chatColors.Any(x => x.username == data.AvatarData.Username))
            {
                data.Color = chatColors.FirstOrDefault(x => x.username == data.AvatarData.Username).color;
            }
            else
            {
                data.Color = colorList[Random.Range(0, colorList.Length)];
                ChatUserColors newColor = new ChatUserColors()
                {
                    username = data.AvatarData.Username, color = data.Color
                };

                chatColors.Add(newColor);
            }
        }
        else if (isPrivateChat)
        {
            if (channelUserId != ChatManager.GetCurrentPrivateChatSelected())
            {
                yield break;
            }
        }

        if (isPrivateChat == (ChatManager.ChatSelected == ChatManager.ChatType.Private))
        {
            List<UserChatData> currentChat = GetCurrentChat();
            List<ChatRow> imageList = chatListRowCards.OrderBy(x => x.GetIndex()).ToList();

            bool isAtBottom = imageList.Any(x => x.GetIndex() >= currentChat.Count - 3);

            bool DoneIsAtBottom = false;

            containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x,
                currentChat.Sum(x => x.ChatBubbleHeight) +
                currentChat[currentChat.Count - 1].ChatBubbleHeight +
                ((spacing) * currentChat.Count - 1));


            if (isAtBottom)
            {
                int userChatIndex = currentChat.Count - 1;

                for (int i = chatListRowCards.Count - 1; i >= 0; i--)
                {
                    if (userChatIndex < 0)
                    {
                        chatListRowCards[i].SetIndex(i + 1, null);
                    }
                    else
                    {
                        chatListRowCards[i].SetIndex(i + 1, currentChat[userChatIndex]);
                    }

                    userChatIndex--;
                }
            }
            
        }

        yield return null;
    }

    private void AdjustSpacing()
    {
        maxYMask.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            maxYMask.GetComponent<RectTransform>().anchoredPosition.x,
            maxYOffset + (chatListRowCards.Sum(x => x.GetRect().sizeDelta.y)));

        List<ChatRow> imageList = chatListRowCards.OrderBy(x => x.GetIndex()).ToList();
        if (imageList.Count == 0)
        {
            return;
        }

        int lastIndex = imageList.Count - 1;

        // Get the position of the first one (last in the list)
        RectTransform rectTransformFirst = imageList[lastIndex].GetRect();
        float firstPosY = rectTransformFirst.anchoredPosition.y;

        if (firstPosY < 0f)
        {
            firstPosY = 0f;
            rectTransformFirst.anchoredPosition = new Vector2(rectTransformFirst.anchoredPosition.x, firstPosY);
        }

        float currentYPosition = firstPosY;

        for (int i = lastIndex - 1; i >= 0; i--)
        {
            RectTransform rectTransformCurrent = imageList[i].GetRect();
            RectTransform rectTransformNext = imageList[i + 1].GetRect();

            float heightOfNextImage = rectTransformNext.rect.height;

            currentYPosition += heightOfNextImage + spacing;
            rectTransformCurrent.anchoredPosition =
                new Vector2(rectTransformCurrent.anchoredPosition.x, currentYPosition);
        }
    }

    // private void CreateChatSnapshot()
    // {
    //     FriendList = gameData.GetDummyFriendList();
    //
    //     int listToFill = 1;
    //     int snapshotListAmount = snapshotLists.Count;
    //     List<List<FriendData>> list = new List<List<FriendData>>();
    //
    //     for (int x = 0; x < snapshotLists.Count; x++)
    //     {
    //         list.Add(new List<FriendData>());
    //     }
    //
    //     for (int i = 0; i < FriendList.Count; i++)
    //     {
    //         list[listToFill].Add(FriendList[i]);
    //
    //         listToFill++;
    //
    //         if (listToFill == snapshotListAmount)
    //             listToFill = 1;
    //     }
    //
    //     for (int s = 0; s < snapshotLists.Count; s++)
    //     {
    //         StartCoroutine(snapshotLists[s].CreateSnaphotFriendList(list[s]));
    //     }
    // }

    IEnumerator Start()
    {
        HideChat();
        yield return null;

        while (ChatManager == null)
        {
            ChatManager = CurrentRoom.Instance.chatManager;
            yield return null;
        }

        language = Injection.Get<ILanguage>();
        AvatarEndpoints = Injection.Get<IAvatarEndpoints>();
        gameData = Injection.Get<IGameData>();
        containerRect = chatRowContainer.GetComponent<RectTransform>();
        // CreateChatSnapshot();
        ChatManager.OnNewPublicChat += AddChatPublic;
        ChatManager.OnNewPrivateChat += AddChatPrivate;
        ChatManager.OnShowPrivateChat += ShowPrivateChat;
        ChatManager.OnShowPublicChat += ShowPublicChat;
        ChatManager.OnHidePrivateChat += HideChat;
        ChatManager.OnHidePublicChat += HideChat;
        yield return null;
        yield return null;
        
    }

    private void OnDestroy()
    {
        ChatManager.OnNewPublicChat -= AddChatPublic;
        ChatManager.OnNewPrivateChat -= AddChatPrivate;
        ChatManager.OnShowPrivateChat -= ShowPrivateChat;
        ChatManager.OnShowPublicChat -= ShowPublicChat;
    }

    public List<UserChatData> GetCurrentChat()
    {
        List<UserChatData> FilteredChatList = ChatManager.ChatSelected == ChatManager.ChatType.Private
            ? ChatManager.GetPrivateChatData(ChatManager.GetCurrentPrivateChatSelected()).ChatData
            : ChatManager.GetPublicChatData();

        return FilteredChatList;
    }


    void ArrangeChat()
    {
        List<UserChatData> FilteredChatList = GetCurrentChat();

        List<UserChatData> lastMessages = new List<UserChatData>();

        int startingIndex = FilteredChatList.Count - chatCardsAmount;

        if (startingIndex < 0)
        {
            startingIndex = 0;
        }

        for (int i = startingIndex; i < FilteredChatList.Count; i++)
        {
            lastMessages.Add(FilteredChatList[i]);
        }

        for (int i = 0; i < chatListRowCards.Count; i++)
        {
            chatListRowCards[i].SetClear(i);
        }

        int userChatIndex = lastMessages.Count - 1;

        for (int i = chatListRowCards.Count - 1; i >= 0; i--)
        {
            if (userChatIndex < 0)
            {
                chatListRowCards[i].SetIndex(i + 1, null);
            }
            else
            {
                chatListRowCards[i].SetIndex(i + 1, lastMessages[userChatIndex]);
            }

            userChatIndex--;
        }

        scroll.verticalNormalizedPosition = 0;

        List<ChatRow> imageList = chatListRowCards.OrderBy(x => x.GetIndex()).ToList();
        if (imageList.Count == 0)
        {
            return;
        }

        int lastIndex = imageList.Count - 1;

        float heightForContainer = FilteredChatList.Sum(x => x.ChatBubbleHeight) +
                                   ((spacing) * FilteredChatList.Count - 1);

        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, heightForContainer);

        // Get the position of the first one (last in the list)
        RectTransform rectTransformFirst = imageList[lastIndex].GetRect();
        rectTransformFirst.anchoredPosition = new Vector2(rectTransformFirst.anchoredPosition.x, 0);
    }


    void ShowPublicChat()
    {
        ShowChat();
        ArrangeChat();
        OnDragPool();
    }

    void ShowPrivateChat(string idUser)
    {
        ShowChat();
        ArrangeChat();
        OnDragPool();
    }

    void AddChatPublic(UserChatData chatData)
    {
        StartCoroutine(this.AddChat(chatData));
        OnDragPool();
    }

    void AddChatPrivate(string idUser, string channelUserId, UserChatData chatData)
    {
        StartCoroutine(this.AddChat(chatData, true, channelUserId));
        OnDragPool();
    }

    public void OnDragPool()
    {
        List<UserChatData> FilteredChatList = GetCurrentChat();

        chatListRowCards.ForEach(x =>
        {
            if (x.transform.position.y < minYMask.position.y)
            {
                ChatRow firstRow = chatListRowCards.OrderBy(item => item.GetIndex()).First();
                ChatRow lastRow = chatListRowCards.OrderByDescending(item => item.GetIndex()).First();

                if (firstRow.GetIndex() > chatCardsAmount)
                {
                    lastRow.SetIndex(firstRow.GetIndex() - 1, FilteredChatList[firstRow.GetIndex() - 1]);
                    lastRow.GetRect().anchoredPosition = new Vector2(firstRow.GetRect().anchoredPosition.x,
                        firstRow.GetRect().anchoredPosition.y + (lastRow.chatMessageData.ChatBubbleHeight + spacing));

                    //setIndexInfo
                }
            }
            else if (x.transform.position.y > maxYMask.position.y)
            {
                ChatRow firstRow = chatListRowCards.OrderBy(item => item.GetIndex()).First();
                ChatRow lastRow = chatListRowCards.OrderByDescending(item => item.GetIndex()).First();
                if (lastRow.GetIndex() < FilteredChatList.Count - 1)
                {
                    firstRow.SetIndex(lastRow.GetIndex() + 1, FilteredChatList[lastRow.GetIndex() + 1]);
                    firstRow.GetRect().anchoredPosition = new Vector2(lastRow.GetRect().anchoredPosition.x,
                        lastRow.GetRect().anchoredPosition.y - (firstRow.chatMessageData.ChatBubbleHeight + spacing));

                    //setIndexInfo
                }
            }
        });
    }
}
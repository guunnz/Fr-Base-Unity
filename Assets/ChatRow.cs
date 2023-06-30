using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatRow : MonoBehaviour
{
    private RectTransform rect;
    public int Index = 0;
    [SerializeField] Image Head;
    [SerializeField] TextMeshProUGUI PlayerName;
    [SerializeField] TextMeshProUGUI ChatMessage;
    RectTransform ChatMessageRect;
    public Image backgroundBubble;
    internal UserChatData chatMessageData; // Changed to UserChatData

    private int smallSize = 37;
    private int mediumSize = 43;
    private int largeSize = 55;

    private int ownerSmallSize = 20;
    private int ownerMediumSize = 30;
    private int ownerLargeSize = 42;

    public void SetClear(int index)
    {
        this.Index = index;
        backgroundBubble.enabled = false;
        Head.enabled = false;
        PlayerName.enabled = false;
        Head.sprite = null;
        ChatMessage.text = "";
    }

    public void SetIndex(int Index, UserChatData data)
    {
        this.Index = Index;
        if (data == null)
            return;
        if (string.IsNullOrEmpty(data.AvatarData.Username))
        {
            return;
        }

        if (!backgroundBubble.enabled)
            backgroundBubble.enabled = true;
        if (data.IsMe)
        {
            SetIndexOwner(Index, data);
            return;
        }

        Head.enabled = true;
        PlayerName.enabled = true;
        PlayerName.color = data.Color;
        backgroundBubble.color = new Color(1, 1, 1, 0.9f);
        ChatMessageRect.anchoredPosition = new Vector2(ChatMessageRect.anchoredPosition.x, -25f);
        Head.sprite = null;

        chatMessageData = data;
        PlayerName.text = data.AvatarData.Username;
        ChatMessage.text = data.ChatText;
        Head.sprite = CurrentRoom.Instance.RoomSnapshotAvatarManager.GetSnapshot(data.AvatarData.UserId);
        Head.color = Color.white;
        if (data.ChatBubbleHeight == 0)
            data.ChatBubbleHeight =
                GetNeededHeight(false, data);
        rect.sizeDelta =
            new Vector2(rect.sizeDelta.x,
                data.ChatBubbleHeight);
    }

    public void SetIndex()
    {
        Head.sprite = null;
    }

    public RectTransform GetRect()
    {
        return rect;
    }

    public int GetIndex()
    {
        return Index;
    }

    public void SetIndexOwner(int Index, UserChatData data) // Changed to UserChatData
    {
        Head.enabled = false;
        PlayerName.enabled = false;
        if (data.PrivateChat)
        {
            backgroundBubble.color = new Color((175f / 255f), (224f / 255f), (232f / 255f), 0.9f);
        }
        else
        {
            backgroundBubble.color = new Color((224f / 255f), (243f / 255f), (200f / 255f), 0.9f);
        }

        ChatMessageRect.anchoredPosition = new Vector2(ChatMessageRect.anchoredPosition.x, -10f);
        this.Index = Index;
        chatMessageData = data;
        ChatMessage.text = data.ChatText;

        if (data.ChatBubbleHeight == 0)
            data.ChatBubbleHeight = GetNeededHeight(true, data);

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, data.ChatBubbleHeight);
    }

    float GetNeededHeight(bool isOwner, UserChatData data)
    {
        float totalWidth = 0f;
        string text = ChatMessage.text;

        string textBuilt = "";

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            TMP_Character character = null;
            textBuilt += text[i];
            // Use TryAddCharacters to make sure the character exists in the font asset
            if (!ChatMessage.font.TryAddCharacters(c.ToString()))
            {
                // Find the character in the character table
                foreach (var charItem in ChatMessage.font.characterTable)
                {
                    if (charItem.unicode == c)
                    {
                        character = charItem;
                        break;
                    }
                }

                if (character != null)
                {
                    totalWidth += character.glyph.metrics.horizontalAdvance;
                }

                if (totalWidth > 2000f)
                {
                    ChatMessage.text = textBuilt + "...";
                    break;
                }
            }
        }

        data.ChatText = textBuilt;
        if (totalWidth < 770f)
        {
            return !isOwner ? smallSize : ownerSmallSize;
        }
        else if (totalWidth < 1435f)
        {
            return !isOwner ? mediumSize : ownerMediumSize;
        }
        else
        {
            return !isOwner ? largeSize : ownerLargeSize;
        }
    }


    private void Awake()
    {
        ChatMessageRect = ChatMessage.GetComponent<RectTransform>();
        backgroundBubble = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }
}
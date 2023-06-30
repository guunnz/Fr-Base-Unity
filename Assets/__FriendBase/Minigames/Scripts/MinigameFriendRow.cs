using FriendsView.Core.Domain;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinigameFriendRow : MonoBehaviour
{
    private FriendData friend;
    private FriendInviteMultiplayer InviteManager;
    private RectTransform rect;
    private int Index = 0;
    [SerializeField] Image Head;
    [SerializeField] TextMeshProUGUI PlayerName;

    public void SetIndex(int Index)
    {
        Head.sprite = null;
        this.friend = InviteManager.GetTrimmedFriendlist()[Index];

        if (InviteManager.GetFriendToInvite() == this.friend)
        {
            SetChosen();
        }
        else
        {
            SetUnChosen();
        }
        this.Index = Index;

        PlayerName.text = friend.username;
        CreateHead();
    }

    public void SetIndex()
    {
        Head.sprite = null;
        this.friend = InviteManager.GetTrimmedFriendlist()[Index];

        if (InviteManager.GetFriendToInvite() == this.friend)
        {
            SetChosen();
        }
        else
        {
            SetUnChosen();
        }

        PlayerName.text = friend.username;
        CreateHead();
    }

    private void CreateHead()
    {
        Head.sprite = this.friend.GetHeadSprite();
        Head.color = Color.white;

    }

    private void Update()
    {
        if (this.friend.GetHeadSprite() != null && Head.sprite == null)
        {
            CreateHead();
        }
    }

    public RectTransform GetRect()
    {
        return rect;
    }

    public int GetIndex()
    {
        return Index;
    }

    internal void SetInviteManager(FriendInviteMultiplayer inviteManager)
    {
        InviteManager = inviteManager;
    }

    private void SetChosen()
    {
        GetComponent<Image>().color = new Color(0.6235294f, 0.8784314f, 0.3176471f, 0.4f);
    }

    private void SetUnChosen()
    {
        GetComponent<Image>().color = Color.white;
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }


    public void SetInvite()
    {
        InviteManager.SetFriendToInvite(friend);
    }
}

using Architecture.Injector.Core;
using Data;
using FriendsView.Core.Domain;
using LocalizationSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendInviteMultiplayer : MonoBehaviour
{
    [SerializeField] Game game;
    [SerializeField] IGameData gameData;
    [SerializeField] GameObject friendRowPrefab;

    [SerializeField] List<MinigameFriendRow> FriendListRowCards = new List<MinigameFriendRow>();
    private List<FriendData> FriendList = new List<FriendData>();
    private List<FriendData> TrimmedFriendList = new List<FriendData>();

    [SerializeField] GridLayoutGroup layoutGroup;
    [SerializeField] Transform friendRowContainer;
    [SerializeField] Transform minYMask;
    [SerializeField] Transform maxYMask;
    [SerializeField] TextMeshProUGUI waitingForPlayerText;
    [SerializeField] RacingMultiplayerManager RMM;

    private int FriendsAmount = 50;
    private int FriendsCardsAmount = 7;

    private int DistanceBetweenCards = 38;

    private FriendData friendToInvite;

    private string lastTrimmed;

    private ILanguage language;

    [SerializeField] ScrollRect scroll;

    [SerializeField] TMPro.TMP_InputField inputFieldTrimmer;
    [SerializeField] List<DoSnapshotList> snapshotLists;

    private bool friendPortraitsSet;
    IAvatarEndpoints AvatarEndpoints;

    public List<FriendData> GetTrimmedFriendlist()
    {
        return TrimmedFriendList;
    }

    private void CreateFriendlistSnapshot()
    {
        FriendList = gameData.GetDummyFriendList();
        //FriendList = gameData.GetFriendList();

        int listToFill = 1;
        int snapshotListAmount = snapshotLists.Count;
        List<List<FriendData>> list = new List<List<FriendData>>();

        for (int x = 0; x < snapshotLists.Count; x++)
        {
            list.Add(new List<FriendData>());
        }

        for (int i = 0; i < FriendList.Count; i++)
        {
            list[listToFill].Add(FriendList[i]);

            listToFill++;

            if (listToFill == snapshotListAmount)
                listToFill = 1;
        }

        for (int s = 0; s < snapshotLists.Count; s++)
        {
            StartCoroutine(snapshotLists[s].CreateSnaphotFriendList(list[s]));
        }
    }

    IEnumerator Start()
    {
        language = Injection.Get<ILanguage>();
        AvatarEndpoints = Injection.Get<IAvatarEndpoints>();
        gameData = Injection.Get<IGameData>();

        CreateFriendlistSnapshot();
        SpawnFriends(FriendList);

        yield return null;
        ListLoaded();
        yield return null;
        OnDragPool();
    }
    private void ManageTrimming()
    {
        string searchingTest = inputFieldTrimmer.text.ToLower();


        if (searchingTest == lastTrimmed)
            return;

        if (string.IsNullOrEmpty(searchingTest) || searchingTest == "" || searchingTest == null)
        {
            TrimmedFriendList = FriendList;
        }
        else
        {
            TrimmedFriendList = FriendList.Where(x => x.username.ToLower().Contains(searchingTest) || x.realName.ToLower().Contains(searchingTest)).ToList();
        }

        RectTransform containerRect = friendRowContainer.GetComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, TrimmedFriendList.Count * DistanceBetweenCards);


        if (TrimmedFriendList.Count < FriendsCardsAmount)
        {
            FriendListRowCards.ForEach(x => x.gameObject.SetActive(false));
            for (int i = 0; i < TrimmedFriendList.Count; i++)
            {
                FriendListRowCards[i].gameObject.SetActive(true);
                FriendListRowCards[i].SetIndex(i);
            }
        }
        else
        {
            for (int i = 0; i < FriendListRowCards.Count; i++)
            {
                FriendListRowCards[i].gameObject.SetActive(true);
                FriendListRowCards[i].SetIndex(i);
                FriendListRowCards[i].GetRect().anchoredPosition = new Vector2(FriendListRowCards[i].GetRect().anchoredPosition.x, -20 + (i * -38f));
            }
        }

        scroll.verticalNormalizedPosition = 1f;

        //OnDragPool();
        lastTrimmed = searchingTest;
    }

    private void ListLoaded()
    {
        layoutGroup.enabled = false;
    }

    public void OnDragPool()
    {
        FriendListRowCards.ForEach(x =>
        {
            if (x.transform.position.y < minYMask.position.y)
            {
                MinigameFriendRow firstRow = FriendListRowCards.OrderBy(item => item.GetIndex()).First();
                if (firstRow.GetIndex() > 0)
                {
                    x.SetIndex(firstRow.GetIndex() - 1);
                    x.GetRect().anchoredPosition = new Vector2(x.GetRect().anchoredPosition.x, firstRow.GetRect().anchoredPosition.y + 38f);
                    //setIndexInfo
                }
            }
            else if (x.transform.position.y > maxYMask.position.y)
            {
                MinigameFriendRow lastRow = FriendListRowCards.OrderByDescending(item => item.GetIndex()).First();
                if (lastRow.GetIndex() < FriendsAmount - 1)
                {
                    x.SetIndex(lastRow.GetIndex() + 1);
                    x.GetRect().anchoredPosition = new Vector2(x.GetRect().anchoredPosition.x, lastRow.GetRect().anchoredPosition.y - 38f);
                    //setIndexInfo
                }
            }
        });
    }

    private void OnDisable()
    {
        inputFieldTrimmer.onEndEdit.RemoveAllListeners();
    }

    private void SpawnFriends(List<FriendData> friendList)
    {
        this.FriendList = friendList;
        inputFieldTrimmer.onValueChanged.AddListener(delegate { ManageTrimming(); });
        TrimmedFriendList = FriendList;
        FriendsAmount = FriendList.Count;

        if (FriendsAmount < FriendsCardsAmount)
        {
            FriendsCardsAmount = FriendsAmount;
        }

        for (int i = 0; i < FriendsCardsAmount; i++)
        {
            MinigameFriendRow friendRow = Instantiate(friendRowPrefab, friendRowContainer).GetComponent<MinigameFriendRow>();
            friendRow.SetInviteManager(this);
            FriendListRowCards.Add(friendRow);

            friendRow.SetIndex(i);
        }

        RectTransform containerRect = friendRowContainer.GetComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, FriendsAmount * DistanceBetweenCards);
    }
    public FriendData GetFriendToInvite()
    {
        return friendToInvite;
    }

    public void InviteFriend()
    {
        AvatarEndpoints.GameFriendInviteAsync(gameData.GetUserInformation().UserId, friendToInvite.userID, this.game);

        waitingForPlayerText.text = language.GetTextByKey(LangKeys.RACING_WAITING_FOR_USERNAME).Replace("[username]", friendToInvite.username);

        RMM.StartCoroutine(RMM.SearchMatchCoroutine(true, friendToInvite.username));
    }

    void DoIndexBehaviourOnRows()
    {
        FriendListRowCards.ForEach(x => x.SetIndex());
    }

    public void SetFriendToInvite(FriendData friend)
    {
        friendToInvite = friend;
        DoIndexBehaviourOnRows();
    }
}
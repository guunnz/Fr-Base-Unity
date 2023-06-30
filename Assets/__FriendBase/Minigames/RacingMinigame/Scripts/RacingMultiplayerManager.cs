using Architecture.Injector.Core;
using Data;
using Socket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Data.Users;
using System.Linq;
using static CarSelectionManager;
using System.Text.RegularExpressions;
using LocalizationSystem;

public class RacingCustomActions
{
    public const string ChooseCar = "ChooseCar";
    public const string QuitMatch = "QuitMatch";
    public const string RollbackInfo = "RollbackInfo";
    public const string TrackSeed = "TrackSeed";
    public const string ForceEnd = "ForceEnd";
    public const string Ready = "Ready";
}

[System.Serializable]
public class RacingMultiplayerMenuItems
{
    public GameObject MenuObject;
    public RacingMultiplayerManager.RacingMultiplayerMenu MenuType;
}

[System.Serializable]
public class RacingPlayer
{
    public string Username;
    public string UserId;
    public CarSkinEnum CarSkin;
    public bool isLocalPlayer = false;
    public Sprite playerSprite;
    public int playerNumber;
    public bool WantsToReplay = false;
}

public class RacingMultiplayerManager : MinigameMultiplayerManager
{
    public enum RacingMultiplayerMenu
    {
        MainMenu = 0,
        ChooseMatchmaking = 1,
        FindingPlayer = 2,
        WaitingForPlayer = 3,
        UsernameJoined = 4,
        WeCantFindMatch = 5,
        SelectCars = 6,
        SelectControls = 7,
        CantFindFriend = 8,
        NoRematch = 9,
        LeftMatch = 10,
        Replay = 11,
    }

    public enum RacingActions
    {
        None = 0,
        TurnRight = 1,
        TurnLeft = 2,
        Accelerate = 3,
        Stop = 4,
        RaceEnd = 5,
    }

    [SerializeField] TextMeshProUGUI waitingForPlayerText;

    [SerializeField] TextMeshProUGUI WeCantFindAnyMatchText;

    [SerializeField] TextMeshProUGUI RematchNotAvailable;

    [SerializeField] TextMeshProUGUI FriendNotAvailableText;

    [SerializeField] TextMeshProUGUI LeftTheMatchText;

    [SerializeField] GameObject waitingForPlayerContainer;
    [SerializeField] float timerMatchMaking = 30;

    public TextMeshProUGUI UsernameOpponentText;

    public TextMeshProUGUI JoinedText;

    public Snapshots.SnapshotAvatar OpponentAvatarSnapshot;

    public List<RacingMultiplayerMenuItems> menuList;

    public List<RacingPlayer> racingPlayers;

    public GameObject SelectControls;

    public GameObject PlayerFound;

    public RacingMinigame RM;

    private ILanguage language;

    private string lastFriendInvited;

    private Coroutine SearchMatch;

    public override void FromMultiplayerInvite(AbstractIncomingSocketEvent incomingEvent)
    {
        MatchFound(incomingEvent);
    }

    void Start()
    {
        language = Injection.Get<ILanguage>();
        gameData = Injection.Get<IGameData>();
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.MATCH_EVENT, MatchFound);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.RACING_CUSTOM_MESSAGE, CustomEventMessage);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.PLAY_AGAIN, ReplayMatch);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.MINIGAME_INVITE_STATUS_UPDATE, ManageReject);
    }

    public void ManageReject(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventMinigameStatus incomingEventResponse = incomingEvent as IncomingEventMinigameStatus;

        if (incomingEventResponse.status == "rejected")
        {
            SimpleSocketManager.Instance.LeaveLobby(gameData.GetUserInformation().UserId.ToString());
            if (SearchMatch != null)
                StopCoroutine(SearchMatch);
            ResetAllMenues();
            menuList.Single(x => x.MenuType == RacingMultiplayerMenu.CantFindFriend).MenuObject.SetActive(true);
            FriendNotAvailableText.text = language.GetTextByKey(LangKeys.RACING_NOT_AVAILABLE_USERNAME).Replace("[username]", lastFriendInvited);
        }
    }

    public void WaitingForPlayerRematch()
    {
        waitingForPlayerContainer.SetActive(true);
        waitingForPlayerText.text = language.GetTextByKey(LangKeys.RACING_WAITING_FOR_USERNAME).Replace("[username]", racingPlayers.Single(x => !x.isLocalPlayer).Username);
        SearchMatch = StartCoroutine(SearchMatchCoroutine(false, racingPlayers.Single(x => !x.isLocalPlayer).Username, true));
    }

    public IEnumerator LeftMatch()
    {
        ResetAllMenues();

        menuList.Single(x => x.MenuType == RacingMultiplayerMenu.LeftMatch).MenuObject.SetActive(true);
        LeftTheMatchText.text = language.GetTextByKey(LangKeys.RACING_LEFT_MATCH).Replace("[username]", racingPlayers.Single(x => !x.isLocalPlayer).Username);
        if (SearchMatch != null)
            StopCoroutine(SearchMatch);

        yield return new WaitForSeconds(1);

        ResetAllMenues();
        menuList.Single(x => x.MenuType == RacingMultiplayerMenu.MainMenu).MenuObject.SetActive(true);


    }

    public IEnumerator SearchMatchCoroutine(bool isFriend = false, string name = "", bool isRematch = false)
    {
        yield return new WaitForSeconds(timerMatchMaking);
        SimpleSocketManager.Instance.LeaveLobby(gameData.GetUserInformation().UserId.ToString());
        ResetAllMenues();
        if (isRematch)
        {
            menuList.Single(x => x.MenuType == RacingMultiplayerMenu.NoRematch).MenuObject.SetActive(true);
            RematchNotAvailable.text = language.GetTextByKey(LangKeys.RACING_NOT_AVAILABLE_ANYMORE_USERNAME).Replace("[username]", name);
        }
        else
        {
            if (!isFriend)
            {
                menuList.Single(x => x.MenuType == RacingMultiplayerMenu.WeCantFindMatch).MenuObject.SetActive(true);
                WeCantFindAnyMatchText.text = language.GetTextByKey(LangKeys.RACING_WE_CANT_FIND_MATCH);
            }
            else
            {
                lastFriendInvited = name;
                menuList.Single(x => x.MenuType == RacingMultiplayerMenu.CantFindFriend).MenuObject.SetActive(true);
                FriendNotAvailableText.text = language.GetTextByKey(LangKeys.RACING_NOT_AVAILABLE_USERNAME).Replace("[username]", name);
            }
        }
    }

    public void ResetPlayers()
    {
        racingPlayers.Clear();
        matchId = null;
    }

    private void OnDestroy()
    {
        SimpleSocketManager.Instance.LeaveLobby(gameData.GetUserInformation().UserId.ToString());
        SimpleSocketManager.Instance.LeaveMatch(gameData.GetUserInformation().UserId.ToString());
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.MATCH_EVENT, MatchFound);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.RACING_CUSTOM_MESSAGE, CustomEventMessage);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.PLAY_AGAIN, ReplayMatch);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.MINIGAME_INVITE_STATUS_UPDATE, ManageReject);
    }

    public bool IsPlayerOne(string userId)
    {
        return racingPlayers.Any(x => x.playerNumber == 1 && x.UserId == userId);
    }

    public void MatchFound(AbstractIncomingSocketEvent incomingEvent)
    {
        MinigameInvitation.instance.canBeInvited = false;
        if (SearchMatch != null)
            StopCoroutine(SearchMatch);
        IncomingEventMatchFound incomingEventMatch = incomingEvent as IncomingEventMatchFound;
        if (matchId == incomingEventMatch.matchId)
            return;
        UserInformation myInfo = gameData.GetUserInformation();
        SimpleSocketManager.Instance.LeaveLobby(gameData.GetUserInformation().UserId.ToString());
        SimpleSocketManager.Instance.JoinMatch(incomingEventMatch.matchId);
        matchId = incomingEventMatch.matchId;
        UsernameOpponentText.text = "";

        //Clear potential old player list
        racingPlayers.Clear();

        //Set Racing Players
        racingPlayers.Add(new RacingPlayer() { UserId = incomingEventMatch.player1Id, Username = incomingEventMatch.player1Username, isLocalPlayer = incomingEventMatch.player1Username == myInfo.UserName, playerNumber = 1 });
        racingPlayers.Add(new RacingPlayer() { UserId = incomingEventMatch.player2Id, Username = incomingEventMatch.player2Username, isLocalPlayer = incomingEventMatch.player2Username == myInfo.UserName, playerNumber = 2 });

        AvatarCustomizationData opponentAvatarData = new AvatarCustomizationData();

        ResetAllMenues();

        menuList.Single(x => x.MenuType == RacingMultiplayerMenu.UsernameJoined).MenuObject.SetActive(true);

        opponentAvatarData.SetDataFromUserSkin(incomingEventMatch.player2Username == myInfo.UserName ? incomingEventMatch.avatar1Json : incomingEventMatch.avatar2Json, false);

        OpponentAvatarSnapshot.CreateSnaphot(ShowUsernameConnected, opponentAvatarData);

        RM.SetMatchId(matchId);
    }

    public void ReplayMatch(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventPlayAgain incomingEventPlayAgain = incomingEvent as IncomingEventPlayAgain;

        racingPlayers.Single(x => x.UserId == incomingEventPlayAgain.userId).WantsToReplay = true;

        if (racingPlayers.All(x => x.WantsToReplay))
        {
            if (SearchMatch != null)
                StopCoroutine(SearchMatch);
            ResetAllMenues();
            SimpleSocketManager.Instance.LeaveMatch(matchId);
            SimpleSocketManager.Instance.JoinMatch(incomingEventPlayAgain.matchId);
            matchId = incomingEventPlayAgain.matchId;
            RM.SetMatchId(matchId);
            EnableSelectControls();
            racingPlayers.ForEach(x => x.WantsToReplay = false);
        }
    }

    public void ShowUsernameConnected(bool success, int spriteId, Sprite sprite)
    {
        if (SearchMatch != null)
            StopCoroutine(SearchMatch);
        JoinedText.text = language.GetTextByKey(LangKeys.RACING_JOINED_USERNAME).Replace("[username]", ""); //TODO: TRANSLATE
        RacingPlayer Opponent = racingPlayers.Single(x => !x.isLocalPlayer);
        UsernameOpponentText.text = Opponent.Username;
        Opponent.playerSprite = sprite;
        Invoke("EnableSelectControls", 2f);
    }

    private void EnableSelectControls()
    {
        if (SearchMatch != null)
            StopCoroutine(SearchMatch);
        PlayerFound.SetActive(false);
        SelectControls.SetActive(true);
    }

    public void ResetAllMenues()
    {
        menuList.ForEach(x => x.MenuObject.SetActive(false));
    }

    public void JoinLobby()
    {
        menuList.Single(x => x.MenuType == RacingMultiplayerMenu.FindingPlayer).MenuObject.SetActive(true);
        gameData = Injection.Get<IGameData>();
        SimpleSocketManager.Instance.JoinLobby(gameData.GetUserInformation().UserId.ToString());
        SearchMatch = StartCoroutine(SearchMatchCoroutine(false));
    }

    public void CustomEventMessage(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventRacingCustomMessage incomingEventMatch = incomingEvent as IncomingEventRacingCustomMessage;

        if (incomingEventMatch.firebaseUid == gameData.GetUserInformation().FirebaseId)
        {
            return;
        }

        switch (incomingEventMatch.actionType)
        {
            case RacingCustomActions.ChooseCar:
                SetCarSkin(int.Parse(incomingEventMatch.eventMessage), false);
                break;
            case RacingCustomActions.QuitMatch:
                StartCoroutine(LeftMatch());
                break;
            default:
                break;
        }
    }

    //Local player or (generally) the opponent
    public void SetCarSkin(int skin, bool isLocalPlayer)
    {
        RacingPlayer Player = racingPlayers.Single(x => x.isLocalPlayer == isLocalPlayer);

        Player.CarSkin = (CarSkinEnum)skin;

        if (isLocalPlayer)
        {
            SimpleSocketManager.Instance.SendCustomActionRacing(matchId, RacingCustomActions.ChooseCar, skin.ToString());
        }
    }

    //Always Local Player
    public void SetCarSkin(int skin)
    {
        RacingPlayer Player = racingPlayers.Single(x => x.isLocalPlayer == true);

        Player.CarSkin = (CarSkinEnum)skin;

        SimpleSocketManager.Instance.SendCustomActionRacing(matchId, RacingCustomActions.ChooseCar, skin.ToString());
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SimpleSocketManager.Instance.SendActionRacing(matchId, RacingActions.Accelerate);
        }
    }
}
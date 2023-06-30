using Architecture.Injector.Core;
using Data;
using LocalizationSystem;
using Socket;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RacingMinigame : AbstractMinigame
{
    [SerializeField] GameObject gameplayPrefab;
    private GameObject gameplayInstance;
    [SerializeField] GameObject container;
    [SerializeField] GameObject replayMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject background;
    public RacingMultiplayerManager MultiplayerManager;





    [SerializeField] TextMeshProUGUI coinsEarnedText;
    [SerializeField] TextMeshProUGUI bonusCoinsText;
    [SerializeField] TextMeshProUGUI YouWinText;
    [SerializeField] TextMeshProUGUI OnePlayerText;
    [SerializeField] TextMeshProUGUI MultiplayerText;
    [SerializeField] TextMeshProUGUI SelectControlsText;
    [SerializeField] TextMeshProUGUI InviteFriendText;
    [SerializeField] TextMeshProUGUI FindSomeoneText;
    [SerializeField] TextMeshProUGUI SearchbarFriend;
    [SerializeField] TextMeshProUGUI SearchFriendOnline;
    [SerializeField] TextMeshProUGUI InviteOtherFriend;
    [SerializeField] TextMeshProUGUI PlayAgainstBobText;
    [SerializeField] TextMeshProUGUI TiltActivatedTxt;
    [SerializeField] TextMeshProUGUI OnlyButtonsTxt;
    [SerializeField] TextMeshProUGUI IfYouSelectTilt;
    [SerializeField] TextMeshProUGUI ChooseText;
    [SerializeField] TextMeshProUGUI BonusText;
    [SerializeField] TextMeshProUGUI PlayingCoinsTitle;
    [SerializeField] TextMeshProUGUI ReplayText;
    [SerializeField] TextMeshProUGUI GoToMenu;
    [SerializeField] TextMeshProUGUI InviteAFriendText;
    [SerializeField] TextMeshProUGUI FindSomeoneText2;
    [SerializeField] TextMeshProUGUI HowDoYouWantToPlay;
    [SerializeField] TextMeshProUGUI GoBack;
    [SerializeField] TextMeshProUGUI FindingSomeoneOnlineText;
    [SerializeField] TextMeshProUGUI KeepWaitingText;
    [SerializeField] TextMeshProUGUI PlayAgainstBobText2;
    [SerializeField] TextMeshProUGUI WeCantFindAnyMatchText2;
    [SerializeField] TextMeshProUGUI InviteOtherFriend2;
    [SerializeField] TextMeshProUGUI PlayAgainstBobText3;
    [SerializeField] TextMeshProUGUI PlayAgainstBobText4;
    [SerializeField] TextMeshProUGUI GoMenuText;
    [SerializeField] TextMeshProUGUI FriendNotAvailableText;
    [SerializeField] TextMeshProUGUI Choose;
    [SerializeField] TextMeshProUGUI GoBack2;






    private bool OpponentWon;
    [SerializeField] int CoinsPerWin = 6;
    [SerializeField] int CoinsPerLose = 3;
    [SerializeField] AudioSource _AudioSource;
    [SerializeField] List<RacingSound> RacingSounds;


    private string MatchId;

    public void playAudio(eRacingSound sound, bool loop = true)
    {
        _AudioSource.loop = loop;
        _AudioSource.clip = RacingSounds.Single(x => x.racingSound == sound).AudioClip;
        _AudioSource.Play();
    }

    public string GetMatchId()
    {
        return MatchId;
    }

    public bool IsMultiplayerGame()
    {
        return MultiplayerManager.racingPlayers.Count > 0;
    }

    public void SetMatchId(string MatchId)
    {
        this.MatchId = MatchId;
    }

    public RacingMinigame(Game idGame)
    {
        this.idGame = idGame;
    }

    public void LoadTest()
    {
        SceneManager.LoadScene("TestCars");
    }

    private void Start()
    {
        SetLanguageKeys();
        PlayerPrefs.SetInt("Bonus", 0);
        gameData = Injection.Get<IGameData>();
    }

    public void SetLanguageKeys()
    {
        language = Injection.Get<ILanguage>();

        language.SetText(YouWinText, language.GetTextByKey(LangKeys.RACING_Awesome));
        language.SetText(OnePlayerText, language.GetTextByKey(LangKeys.RACING_One_Player));
        language.SetText(MultiplayerText, language.GetTextByKey(LangKeys.MINIGAME_MULTIPLAYER));
        language.SetText(SelectControlsText, language.GetTextByKey(LangKeys.RACING_Select_Controls));
        language.SetText(InviteFriendText, language.GetTextByKey(LangKeys.RACING_INVITE_FRIEND));
        language.SetText(FindSomeoneText, language.GetTextByKey(LangKeys.RACING_FIND_SOMEONE));
        language.SetText(SearchbarFriend, language.GetTextByKey(LangKeys.RACING_SEARCHING_BAR));
        language.SetText(SearchFriendOnline, language.GetTextByKey(LangKeys.RACING_SEARCHING_BAR));
        language.SetText(InviteOtherFriend, language.GetTextByKey(LangKeys.RACING_INVITE_OTHER_FRIEND));
        language.SetText(PlayAgainstBobText, language.GetTextByKey(LangKeys.RACING_PLAY_AGAINST_BOB));
        language.SetText(TiltActivatedTxt, language.GetTextByKey(LangKeys.MINIGAME_TILT));
        language.SetText(OnlyButtonsTxt, language.GetTextByKey(LangKeys.MINIGAME_BUTTONS));
        language.SetText(IfYouSelectTilt, language.GetTextByKey(LangKeys.RACING_TILT_TUTORIAL));
        language.SetText(ChooseText, language.GetTextByKey(LangKeys.RACING_Choose));
        language.SetText(BonusText, language.GetTextByKey(LangKeys.MINIGAME_BONUS));
        language.SetText(PlayingCoinsTitle, language.GetTextByKey(LangKeys.MINIGAME_PLAYING));
        language.SetText(ReplayText, language.GetTextByKey(LangKeys.RACING_REMATCH));
        language.SetText(GoToMenu, language.GetTextByKey(LangKeys.MINIGAME_GO_TO_MAIN));
        language.SetText(InviteAFriendText, language.GetTextByKey(LangKeys.RACING_INVITE_FRIEND));
        language.SetText(FindSomeoneText2, language.GetTextByKey(LangKeys.RACING_FIND_SOMEONE));
        language.SetText(HowDoYouWantToPlay, language.GetTextByKey(LangKeys.RACING_HOW_TO_PLAY));
        language.SetText(GoBack, language.GetTextByKey(LangKeys.RACING_GO_BACK));
        language.SetText(FindingSomeoneOnlineText, language.GetTextByKey(LangKeys.RACING_FINDING_SOMEONE_ONLINE));
        language.SetText(KeepWaitingText, language.GetTextByKey(LangKeys.RACING_KEEP_WAITING));
        language.SetText(PlayAgainstBobText2, language.GetTextByKey(LangKeys.RACING_PLAY_AGAINST_BOB));
        language.SetText(WeCantFindAnyMatchText2, language.GetTextByKey(LangKeys.RACING_WE_CANT_FIND_MATCH));
        language.SetText(InviteOtherFriend2, language.GetTextByKey(LangKeys.RACING_INVITE_OTHER_FRIEND));
        language.SetText(PlayAgainstBobText3, language.GetTextByKey(LangKeys.RACING_PLAY_AGAINST_BOB));
        language.SetText(Choose, language.GetTextByKey(LangKeys.RACING_Choose));
        language.SetText(GoBack2, language.GetTextByKey(LangKeys.RACING_GO_BACK));
        language.SetText(PlayAgainstBobText4, language.GetTextByKey(LangKeys.RACING_PLAY_AGAINST_BOB));
        language.SetText(GoMenuText, language.GetTextByKey(LangKeys.MAIN_MENU));
    }

    public override void StartGame()
    {
        container.SetActive(false);
        ToggleVolume(true);
        GameObject gameplay = Instantiate(gameplayPrefab, this.transform);
        RacingMinigameGameplayManager GameplayManager = gameplay.GetComponent<RacingMinigameGameplayManager>();
        GameplayManager.racingMiniGame = this;
        gameplayInstance = gameplay;
    }

    public void SetTilt(bool tilt)
    {
        Input.gyro.enabled = tilt;
        PlayerPrefs.SetInt("Tilt", tilt ? 1 : 0);
    }

    public override void UserLeave()
    {

    }

    public override void OpponentWin()
    {
        OpponentWon = true;
    }

    public void PlayerEnd(int extraCoins = 0)
    {
        int coinsToPlayer = 0;

        if (!OpponentWon)
        {
            StartCoroutine(AudioResultsScreen(true));
            coinsToPlayer = CoinsPerWin;
            YouWinText.text = language.GetTextByKey(LangKeys.MINIGAME_YOU_WIN);
        }
        else
        {
            StartCoroutine(AudioResultsScreen(false));
            YouWinText.text = YouWinText.text = language.GetTextByKey(LangKeys.MINIGAME_YOU_LOSE);
            coinsToPlayer = CoinsPerLose;
        }

        coinsEarnedText.text = coinsToPlayer.ToString();
        bonusCoinsText.text = extraCoins.ToString();
        replayMenu.SetActive(true);
        background.SetActive(true);
        container.SetActive(true);
        Destroy(gameplayInstance);

        MinigameInvitation.instance.canBeInvited = true;
        if (gameData == null)
            gameData = Injection.Get<IGameData>();
        gameData.GetUserInformation().AddGold(coinsToPlayer + extraCoins);

        OpponentWon = false;
    }

    public IEnumerator AudioResultsScreen(bool playerWon)
    {
        ToggleVolume(false);
        if (playerWon)
        {
            playAudio(eRacingSound.FA_Win_Stinger_1_1, false);
        }
        else
        {
            playAudio(eRacingSound.FA_Lose_Stinger_1_1, false);
        }
        yield return new WaitForSeconds(4);
        if (playerWon)
        {
            playAudio(eRacingSound.FA_Win_Jingle_Loop);
        }
        else
        {
            playAudio(eRacingSound.FA_Lose_Jingle_Loop);
        }
    }

    public void ToggleVolume(bool mute)
    {
        if (mute)
        {
            _AudioSource.volume = 0;
        }
        else
        {
            _AudioSource.volume = 1;
        }
    }

    private void OnDestroy()
    {
        MinigameInvitation.instance.canBeInvited = true;
    }

    public void Replay()
    {
        playAudio(eRacingSound.FA_Loading_Screen_Jingle_Loop);
        if (IsMultiplayerGame())
        {
            replayMenu.SetActive(false);
            MultiplayerManager.WaitingForPlayerRematch();
            SimpleSocketManager.Instance.RacingPlayAgain(MatchId);
        }
        else
        {
            StartGame();
        }
    }

    public void GoMenu()
    {
        MinigameInvitation.instance.canBeInvited = true;
        if (IsMultiplayerGame())
        {
            SimpleSocketManager.Instance.SendCustomActionRacing(MatchId, RacingCustomActions.QuitMatch, "");
            SimpleSocketManager.Instance.LeaveMatch(MatchId);
            MatchId = null;
            MultiplayerManager.ResetPlayers();
        }
        playAudio(eRacingSound.FA_Loading_Screen_Jingle_Loop);
        MultiplayerManager.ResetAllMenues();
        replayMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}

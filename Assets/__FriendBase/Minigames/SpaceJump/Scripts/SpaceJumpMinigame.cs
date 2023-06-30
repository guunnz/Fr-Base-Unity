using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Snapshots;
using Audio.Music;
using Architecture.Injector.Core;
using Data;
using LocalizationSystem;

public class SpaceJumpMinigame : AbstractMinigame
{
    [SerializeField] GameObject gameplayPrefab;
    [SerializeField] GameObject replayMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject background;
    [SerializeField] TextMeshProUGUI coinsEarnedText;
    [SerializeField] TextMeshProUGUI bonusCoinsText;

    //IMusicPlayer musicPlayer;

    [SerializeField] SpriteRenderer PlayerHeadMainMenu;


    [Header("Texts")]
    [SerializeField] TextMeshProUGUI OnePlayerText;
    [SerializeField] TextMeshProUGUI MultiplayerText;
    [SerializeField] TextMeshProUGUI ComingSoonText;
    [SerializeField] TextMeshProUGUI GetToTheSpaceshipFirstText;
    [SerializeField] TextMeshProUGUI SelectControlsText;
    [SerializeField] TextMeshProUGUI TiltText;
    [SerializeField] TextMeshProUGUI ButtonsText;
    [SerializeField] TextMeshProUGUI YouWinText;
    [SerializeField] TextMeshProUGUI BonusText;
    [SerializeField] TextMeshProUGUI PlayAgainText;
    [SerializeField] TextMeshProUGUI GoToMainText;
    [SerializeField] TextMeshProUGUI PlayingText;
    [SerializeField] int CoinsPerWin = 6;
    [SerializeField] int CoinsPerLose = 3;


    private GameObject gameplayInstance;

    private bool AIWon;


    public SpaceJumpMinigame(Game idGame)
    {
        this.idGame = idGame;
    }

    private void Start()
    {
        SetLanguageKeys();
        PlayerPrefs.SetInt("Bonus", 0);
        PlayerHeadMainMenu.sprite = MinigamePlayerAvatar.Singleton.GetFace();
        gameData = Injection.Get<IGameData>();
    }

    public void SetLanguageKeys()
    {
        language = Injection.Get<ILanguage>();
        language.SetTextByKey(OnePlayerText, LangKeys.MINIGAME_ONE_PLAYER);
        language.SetTextByKey(MultiplayerText, LangKeys.MINIGAME_MULTIPLAYER);
        language.SetTextByKey(ComingSoonText, LangKeys.MINIGAME_COMING_SOON);
        language.SetTextByKey(GetToTheSpaceshipFirstText, LangKeys.MINIGAME_GET_TO_THE_SPACESHIP_FIRST);
        language.SetTextByKey(SelectControlsText, LangKeys.MINIGAME_SELECT_CONTROLS);
        language.SetTextByKey(TiltText, LangKeys.MINIGAME_TILT);
        language.SetTextByKey(ButtonsText, LangKeys.MINIGAME_BUTTONS);
        language.SetTextByKey(BonusText, LangKeys.MINIGAME_BONUS);
        language.SetTextByKey(PlayAgainText, LangKeys.MINIGAME_PLAY_AGAIN);
        language.SetTextByKey(GoToMainText, LangKeys.MINIGAME_GO_TO_MAIN);
        language.SetTextByKey(PlayingText, LangKeys.MINIGAME_PLAYING);
    }

    public override void StartGame()
    {
        SpaceJumpGameplayManager gameplayManager = Instantiate(gameplayPrefab, this.transform).GetComponent<SpaceJumpGameplayManager>();
        AIWon = false;
        gameplayManager.spaceJumpMinigame = this;

        gameplayInstance = gameplayManager.gameObject;
    }

    public void SetTilt(bool tilt)
    {
        Input.gyro.enabled = tilt;
        PlayerPrefs.SetInt("Tilt", tilt ? 1 : 0);
    }

    public override void UserLeave()
    {

    }

    public override void UserEnds()
    {
        int coinsToPlayer = 0;

        int bonusCoins = PlayerPrefs.GetInt("Bonus");
        PlayerPrefs.SetInt("Bonus", 0);
        if (!AIWon)
        {
            coinsToPlayer = CoinsPerWin;
            YouWinText.text = language.GetTextByKey(LangKeys.MINIGAME_YOU_WIN);
        }
        else
        {
            YouWinText.text = YouWinText.text = language.GetTextByKey(LangKeys.MINIGAME_YOU_LOSE);
            coinsToPlayer = CoinsPerLose;
        }

        coinsEarnedText.text = coinsToPlayer.ToString();
        bonusCoinsText.text = bonusCoins.ToString();
        gameData.GetUserInformation().AddGold(coinsToPlayer + bonusCoins);
        replayMenu.SetActive(true);
        background.SetActive(true);
        Destroy(gameplayInstance);
    }

    public override void OpponentWin()
    {
        AIWon = true;
    }

    public void Replay()
    {
        background.SetActive(false);
        replayMenu.SetActive(false);
        StartGame();
    }

    public void GoMenu()
    {
        replayMenu.SetActive(false);
        Destroy(gameplayInstance);
        mainMenu.SetActive(true);
    }
}
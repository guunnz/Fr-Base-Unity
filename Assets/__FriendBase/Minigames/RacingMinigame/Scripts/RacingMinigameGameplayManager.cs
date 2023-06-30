using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Socket;
using Data;
using Architecture.Injector.Core;
using PathCreation;
using UnityEngine.UI;
using Audio.Music;
using LocalizationSystem;

public class RacingMinigameGameplayManager : MonoBehaviour
{

    [HideInInspector] public RacingMinigame racingMiniGame;

    [SerializeField] TextMeshProUGUI StartCountText;
    [SerializeField] TextMeshProUGUI LoadingText;
    [SerializeField] TextMeshProUGUI PressToAccelerate;

    [SerializeField] Image TopBarPlacement;
    [SerializeField] GameObject TopBarCanvas;
    [SerializeField] GameObject LoadingScreen;

    [SerializeField] List<CarSkin> CarSkins;

    [SerializeField] RacingMinigameTrackManager TrackManager;
    [SerializeField] RacingMusicPlayer MusicPlayer;

    public bool IsMultiplayerMatch;

    [SerializeField] CarController mainPlayer;
    [SerializeField] CarController opponent;

    [SerializeField] Transform goal;

    private float raceTotalDistance;

    private IGameData gameData;
    private ILanguage language;

    private bool LocalPlayerReady;
    private bool OpponentReady;
    [SerializeField] AudioSource _AudioSource;
    [SerializeField] List<RacingSound> RacingSounds;

    internal bool raceStarted;

    public void playAudio(eRacingSound sound)
    {
        _AudioSource.clip = RacingSounds.Single(x => x.racingSound == sound).AudioClip;
        _AudioSource.Play();
    }

    public List<CarSkin> GetCarSkins()
    {
        return CarSkins;
    }

    public float GetRaceTotalDistance()
    {
        return raceTotalDistance;
    }

    public void SetRaceTotalDistance(VertexPath path)
    {
        raceTotalDistance = path.GetClosestDistanceAlongPath(TrackManager.GetGoal().position);
    }

    private void SetRacingFillBar()
    {
        float maxDistanceTravelled = mainPlayer.GetDistanceTravelled()/* > opponent.GetDistanceTravelled() ? mainPlayer.GetDistanceTravelled() : opponent.GetDistanceTravelled()*/;

        TopBarPlacement.fillAmount = (1 / raceTotalDistance) * maxDistanceTravelled;
    }

    private void Update()
    {
        SetRacingFillBar();
    }

    private void Awake()
    {
        TopBarCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        gameData = Injection.Get<IGameData>();
        language = Injection.Get<ILanguage>();

        IsMultiplayerMatch = racingMiniGame.IsMultiplayerGame();
        LoadingText.text = language.GetTextByKey(LangKeys.Loading) + "...";
        PressToAccelerate.text = language.GetTextByKey(LangKeys.RACING_PRESS_ACCELERATE);
        if (!IsMultiplayerMatch)
        {
            TrackManager.CreateRandomizedPath();
            StartCoroutine(InitializeGame());
        }
        else
        {
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.RACING_MATCH_FINISH, MultiplayerMatchEnded);
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.RACING_CUSTOM_MESSAGE, ManageCustomMessages);
            LocalPlayerReady = true;
            SimpleSocketManager.Instance.SendCustomActionRacing(racingMiniGame.GetMatchId(), RacingCustomActions.Ready, RacingCustomActions.Ready);
            StartCoroutine(WaitBothReady());
        }
    }

    private void OnDestroy()
    {
        if (IsMultiplayerMatch)
        {
            SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.RACING_MATCH_FINISH, MultiplayerMatchEnded);
            SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.RACING_CUSTOM_MESSAGE, ManageCustomMessages);
        }
    }

    private void ManageCustomMessages(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventRacingCustomMessage incomingCustomEvent = incomingEvent as IncomingEventRacingCustomMessage;

        switch (incomingCustomEvent.actionType)
        {
            case RacingCustomActions.ChooseCar:
                return;
            case RacingCustomActions.Ready:
                if (incomingCustomEvent.firebaseUid != gameData.GetUserInformation().FirebaseId)
                {
                    OpponentReady = true;
                }
                break;
            case RacingCustomActions.TrackSeed:
                if (incomingCustomEvent.firebaseUid != gameData.GetUserInformation().FirebaseId)
                {
                    TrackManager.CreatePathFromSeed(incomingCustomEvent.eventMessage);
                }
                StartCoroutine(InitializeGame());
                break;
        }
    }

    private IEnumerator WaitBothReady()
    {
        while (!LocalPlayerReady || !OpponentReady)
        {
            yield return null;
        }

        SimpleSocketManager.Instance.SendCustomActionRacing(racingMiniGame.GetMatchId(), RacingCustomActions.Ready, "");

        if (racingMiniGame.MultiplayerManager.IsPlayerOne(gameData.GetUserInformation().UserId.ToString()))
        {
            SimpleSocketManager.Instance.SendCustomActionRacing(racingMiniGame.GetMatchId(), RacingCustomActions.TrackSeed, TrackManager.CreateRandomizedPath());
        }
    }


    public void MultiplayerMatchEnded(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventRacingMatchFinished incomingEventMatch = incomingEvent as IncomingEventRacingMatchFinished;
        //Check who won etc

        if (incomingEventMatch.winnerId != gameData.GetUserInformation().UserId.ToString())
        {
            OpponentWon(mainPlayer.CoinsGrabbed);
        }
        else
        {
            PlayerWon(mainPlayer.CoinsGrabbed);
        }
    }

    private IEnumerator InitializeGame()
    {
        MinigameInvitation.instance.canBeInvited = false;
        MusicPlayer.PlayIntro();
        yield return new WaitForSecondsRealtime(1f);
        LoadingScreen.SetActive(false);
        SetRaceTotalDistance(TrackManager.GetMidPath());

        List<CarController> cars = new List<CarController>(); //temporary
        cars.Add(mainPlayer);
        cars.Add(opponent);

        cars.ForEach(x => { x.myPathFollower.distanceTravelled = 5; x.SetCarColor(); x.SetPlacementImageAmountToMovePerMeter(1000 / GetRaceTotalDistance()); x.StartCamAnimation(); });
        yield return new WaitForSecondsRealtime(3.7f);
        StartCountText.gameObject.SetActive(true);
        playAudio(eRacingSound.FA_Count_Down_Beep);
        StartCountText.text = "3";
        yield return new WaitForSecondsRealtime(1f);
        playAudio(eRacingSound.FA_Count_Down_Beep);

        StartCountText.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        playAudio(eRacingSound.FA_Count_Down_Beep);

        StartCountText.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        StartCountText.text = "GO!";
        cars.ForEach(x => { x.DoCarAnimation(CarController.CarAnimations.Ride); });

        playAudio(eRacingSound.FA_Count_Down_End_2);
        TopBarCanvas.gameObject.SetActive(true);
        cars.ForEach(x => x.raceStarted = true);
        raceStarted = true;
        yield return new WaitForSecondsRealtime(1f);
        StartCountText.gameObject.SetActive(false);
    }


    public void PlayerWon(int extraCoins = 0)
    {
        racingMiniGame.PlayerEnd(extraCoins);
    }

    public void OpponentWon(int multiplayerExtraCoins = 0)
    {
        racingMiniGame.OpponentWin();
        if (IsMultiplayerMatch)
        {
            racingMiniGame.PlayerEnd(multiplayerExtraCoins);
        }
    }


    public void InitializeAI()
    {
    }
}

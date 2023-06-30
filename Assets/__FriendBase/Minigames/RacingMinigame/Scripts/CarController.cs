using PathCreation;
using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Socket;
using System.Linq;
using System;
using System.Globalization;
using static CarSelectionManager;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public enum RacingPath
{
    Left = -1,
    Mid = 0,
    Right = 2,
}

public enum eRacingSound
{
    FA_Loading_Screen_Jingle_Loop,
    FA_Select_Button_1_1,
    FA_Confirm_Button_1_3,
    FA_Count_Down_Beep,
    FA_Count_Down_End_2,
    FA_Collect_Coin_1_1,
    FA_Power_Up_1_1,
    FA_Speed_Up,
    FA_Wrong_Way_1_2,
    FA_Funny_Impact_1_2,
    FA_Win_Stinger_1_1,
    FA_Lose_Stinger_1_1,
    FA_Win_Jingle_Loop,
    FA_Lose_Jingle_Loop,
    FA_Elite_Stadium_FULL_Loop,
    FA_Elite_Stadium_Intro,
    FA_Elite_Stadium_Fast_Loop,
}

[System.Serializable]
public class RacingSound
{
    public eRacingSound racingSound;
    public AudioClip AudioClip;
}

public class CarController : MonoBehaviour
{
    public enum CarAnimations
    {
        Idle = 0,
        TurnRight = 1,
        TurnLeft = 2,
        Spin = 3,
        Hit = 4,
        Win = 5,
        Ride = 6,
    }
    private MinigameInputManager InputMinigame;
    public RacingMinigameGameplayManager RM;
    private bool usingGyroscope;
    private bool supportsGyroscope;
    private bool soundOff; //PLEASE REMOVE AFTER PROPER SOUND IMPLEMENTATION PLEASE
    internal PathCreator pathCreatorMid;
    internal PathCreator pathCreatorRight;
    internal PathCreator pathCreatorLeft;
    public PathFollower myPathFollower;
    public bool immune;
    public Transform CarGraphicsTransform;
    public RacingPath currentPathSelected;
    private float inputDelayAux = 0.3f;
    private float inputDelay = 0.3f;
    [SerializeField] public float Speed = 5f;
    private float Acceleration = 0f;
    private float DeaccelerationSpeed = 5f;
    [SerializeField] float MaxSpeed = 20f;
    [SerializeField] float AccelerationSpeed = 2f;

    [SerializeField] SkinnedMeshRenderer CarSkinnedMesh;
    [SerializeField] SkinnedMeshRenderer DriverSkinnedMesh;

    [SerializeField] CinemachineVirtualCamera StartupAnimationCamera;

    [SerializeField] Image PlacementSkin;
    [SerializeField] Image PlacementSkinParent;
    [SerializeField] Animator _Animator;
    RectTransform PlacementSkinParentRect;

    internal bool canDeaccelerate = true;
    internal bool raceStarted = false;
    private float StartMaxSpeed;
    public bool IsOpponent = false;

    internal bool CanMoveLeft = true;
    internal bool CanMoveRight = true;

    private float timeToGoalInFixedFrames;

    private bool playerFinished;

    private string PlayerUserId;

    private bool bAccelerate;

    private float placementImageAmountToMovePerMeter;

    internal int CoinsGrabbed;

    public TextMeshProUGUI coinsText;

    [SerializeField] Transform Kart;

    [SerializeField] List<RacingSound> SoundList;

    [SerializeField] AudioSource _AudioSource;


    public void playAudio(eRacingSound sound)
    {
        _AudioSource.clip = SoundList.Single(x => x.racingSound == sound).AudioClip;
        _AudioSource.Play();
    }
    public void StartCamAnimation()
    {
        if (!IsOpponent)
            StartupAnimationCamera.enabled = false;
    }



    public void DoCarAnimation(CarAnimations anim)
    {
        switch (anim)
        {
            case CarAnimations.Ride:
            case CarAnimations.Idle:
            case CarAnimations.Win:
                _Animator.SetBool(Enum.GetName(typeof(CarAnimations), CarAnimations.Win), false);
                _Animator.SetBool(Enum.GetName(typeof(CarAnimations), CarAnimations.Idle), false);
                _Animator.SetBool(Enum.GetName(typeof(CarAnimations), CarAnimations.Ride), false);
                _Animator.SetBool(Enum.GetName(typeof(CarAnimations), anim), true);
                break;
            default:
                _Animator.SetTrigger(Enum.GetName(typeof(CarAnimations), anim));
                if (anim == CarAnimations.Spin)
                {
                    StartCoroutine(DoKartSpin());
                }
                break;
        }
    }

    IEnumerator DoKartSpin()
    {
        playAudio(eRacingSound.FA_Wrong_Way_1_2);
        Kart.transform.DOLocalRotate(new Vector3(0, 375, 0), 0.5f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.5f);
        Kart.transform.DOLocalRotate(new Vector3(0, -30, 0), 0.05f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.05f);
        Kart.transform.DOLocalRotate(new Vector3(0, 15, 0), 0.05f, RotateMode.FastBeyond360).SetRelative(true).SetEase(Ease.Linear);
    }

    public void DoKartSpeedUp()
    {
        playAudio(eRacingSound.FA_Speed_Up);
    }

    public float GetDistanceTravelled()
    {
        return myPathFollower.distanceTravelled;
    }

    public bool PlayerFinishedRace()
    {
        return playerFinished;
    }
    public void SetPlacementImageAmountToMovePerMeter(float amount)
    {
        placementImageAmountToMovePerMeter = amount;
    }
    public void SetPaths(Road road)
    {
        pathCreatorMid = road.pathCreatorMid;
        pathCreatorRight = road.pathCreatorRight;
        pathCreatorLeft = road.pathCreatorLeft;
        myPathFollower.pathCreator = currentPathSelected == RacingPath.Left ? road.pathCreatorLeft : currentPathSelected == RacingPath.Right ? road.pathCreatorRight : road.pathCreatorMid;
    }

    private void Awake()
    {
        supportsGyroscope = SystemInfo.supportsGyroscope;
        StartMaxSpeed = MaxSpeed;
    }

    private void Start()
    {
        soundOff = PlayerPrefs.GetInt(Settings.PlayerPrefsValues.SoundOff) == 1;
        InputMinigame = MinigameInputManager.Singleton;
        usingGyroscope = PlayerPrefs.GetInt("Tilt") == 1 ? true : false;
        this.PlacementSkinParentRect = this.PlacementSkinParent.GetComponent<RectTransform>();
        if (RM.IsMultiplayerMatch)
        {
            DoMultiplayerInitialization();
        }
    }

    private void DoMultiplayerInitialization()
    {
        //Only suscribe opponent to socket events


        if (IsOpponent)
        {
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.RACING_CUSTOM_MESSAGE, DoCustomAction);
            PlayerUserId = RM.racingMiniGame.MultiplayerManager.racingPlayers.Single(x => !x.isLocalPlayer).UserId;
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.RACING_ACCELERATE, AccelerateFromSocket);
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.RACING_TURN_RIGHT, TurnRightFromSocket);
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.RACING_TURN_LEFT, TurnLeftFromSocket);
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.RACING_STOP, StopFromSocket);
        }
        else
        {
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.RACING_CUSTOM_MESSAGE, ManageForceEnd);
            StartCoroutine(MultiplayerRollback());
            PlayerUserId = RM.racingMiniGame.MultiplayerManager.racingPlayers.Single(x => x.isLocalPlayer).UserId;
        }
        SetMultiplayerPlayerStartingPosition();
    }

    private void OnDestroy()
    {
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.RACING_CUSTOM_MESSAGE, ManageForceEnd);
        if (RM.IsMultiplayerMatch && IsOpponent)
        {
            SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.RACING_CUSTOM_MESSAGE, DoCustomAction);
            SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.RACING_ACCELERATE, AccelerateFromSocket);
            SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.RACING_TURN_RIGHT, TurnRightFromSocket);
            SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.RACING_TURN_LEFT, TurnLeftFromSocket);
            SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.RACING_STOP, StopFromSocket);
        }

        if (!playerFinished && RM.IsMultiplayerMatch && !IsOpponent)
        {
            SimpleSocketManager.Instance.SendActionRacing(RM.racingMiniGame.GetMatchId().ToString(), RacingMultiplayerManager.RacingActions.RaceEnd, "13000000");
        }
    }

    //private void OnApplicationPause()
    //{
    //    if (!playerFinished && RM.IsMultiplayerMatch && !IsOpponent)
    //    {
    //        SimpleSocketManager.Instance.SendActionRacing(RM.racingMiniGame.GetMatchId().ToString(), RacingMultiplayerManager.RacingActions.RaceEnd, "13000000");
    //        RM.OpponentWon();
    //    }
    //}

    //-----------------------------------------------------------------------------
    //-----------------------------------------------------------------------------
    //----------   M U L T I P L A Y E R I M P L E M E N T A T I O N   ------------
    //-----------------------------------------------------------------------------
    //-----------------------------------------------------------------------------

    private void AccelerateFromSocket(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventRacingAccelerate incomingEventAccelerate = incomingEvent as IncomingEventRacingAccelerate;

        if (incomingEventAccelerate.userId == PlayerUserId)
        {
            bAccelerate = true;
        }
    }

    private void TurnRightFromSocket(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventRacingTurnRight incomingEventTurnRight = incomingEvent as IncomingEventRacingTurnRight;
        if (incomingEventTurnRight.userId == PlayerUserId)
        {
            Move(1);
        }
    }

    private void TurnLeftFromSocket(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventRacingTurnLeft incomingEventTurnLeft = incomingEvent as IncomingEventRacingTurnLeft;
        if (incomingEventTurnLeft.userId == PlayerUserId)
        {
            Move(-1);
        }
    }

    private void StopFromSocket(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventRacingStop incomingEventStop = incomingEvent as IncomingEventRacingStop;
        if (incomingEventStop.userId == PlayerUserId)
        {
            bAccelerate = false;
        }
    }

    private void DoCustomAction(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventRacingCustomMessage incomingEventCustom = incomingEvent as IncomingEventRacingCustomMessage;

        if (incomingEventCustom.actionType == RacingCustomActions.RollbackInfo)
        {
            string[] rollbackData = incomingEventCustom.eventMessage.Split('|');
            string userId = rollbackData[0];

            if (PlayerUserId != userId || !IsOpponent)
            {
                return;
            }
            float distance;
            float speed;
            string carril = rollbackData[3];
            float.TryParse(rollbackData[1].Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out distance);
            float.TryParse(rollbackData[2].Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out speed);
            Enum.TryParse(carril, out RacingPath pathSocket);


            if (pathSocket != currentPathSelected)
            {
                currentPathSelected = pathSocket;
                myPathFollower.OnPathChanged();
                UpdatePath();
            }
            myPathFollower.speed = speed;
            myPathFollower.distanceTravelled = distance;
        }
    }

    private void ManageForceEnd(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventRacingCustomMessage incomingEventCustom = incomingEvent as IncomingEventRacingCustomMessage;
        if (incomingEventCustom.actionType == RacingCustomActions.ForceEnd)
        {
            if (!playerFinished && !IsOpponent)
            {
                Invoke("ForceEnd", 2f);
            }
        }
    }

    public void ForceEnd()
    {
        if (!playerFinished)
            SimpleSocketManager.Instance.SendActionRacing(RM.racingMiniGame.GetMatchId().ToString(), RacingMultiplayerManager.RacingActions.RaceEnd, "10000000");
    }

    public IEnumerator ForceWin()
    {
        yield return new WaitForSecondsRealtime(2f);
        RM.PlayerWon(CoinsGrabbed);
    }

    public IEnumerator MultiplayerRollback()
    {
        while (!playerFinished)
        {
            SimpleSocketManager.Instance.SendCustomActionRacing(RM.racingMiniGame.GetMatchId(), RacingCustomActions.RollbackInfo, PlayerUserId + "|" + myPathFollower.distanceTravelled + "|" + myPathFollower.speed + "|" + currentPathSelected);
            yield return new WaitForSeconds(1f);
        }
    }

    public void SetCarColor()
    {
        if (RM.IsMultiplayerMatch)
        {
            RacingPlayer player = RM.racingMiniGame.MultiplayerManager.racingPlayers.Single(x => x.isLocalPlayer != IsOpponent);
            CarSkin carSkin = RM.GetCarSkins().Single(x => x.Car == player.CarSkin);
            this.CarSkinnedMesh.material = carSkin.carMaterial;
            this.DriverSkinnedMesh.material = carSkin.driverMaterial;
            this.PlacementSkinParent.color = carSkin.carColor;
            if (player.playerSprite != null)
                this.PlacementSkin.sprite = player.playerSprite;
        }
        else
        {
            CarSkinEnum skin = (CarSkinEnum)PlayerPrefs.GetInt("PlayerSkin");
            if (IsOpponent)
            {
                List<CarSkin> possibleSkins = RM.GetCarSkins().Where(x => x.Car != skin).ToList();

                CarSkin carSkin = possibleSkins[UnityEngine.Random.Range(0, possibleSkins.Count)];
                this.CarSkinnedMesh.material = carSkin.carMaterial;
                this.DriverSkinnedMesh.material = carSkin.driverMaterial;

                this.PlacementSkinParent.color = carSkin.carColor;
            }
            else
            {
                CarSkin carSkin = RM.GetCarSkins().Single(x => x.Car == skin);
                this.CarSkinnedMesh.material = carSkin.carMaterial;
                this.DriverSkinnedMesh.material = carSkin.driverMaterial;
                this.PlacementSkinParent.color = carSkin.carColor;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!raceStarted)
        {
            return;
        }

        if (!playerFinished)
        {
            timeToGoalInFixedFrames++;
        }

        float unitsToMove = myPathFollower.speed <= 1 ? 0.04f : myPathFollower.speed / 30f;

        if (unitsToMove > 0.12f)
        {
            unitsToMove = 0.12f;
        }

        CarGraphicsTransform.position = Vector3.Lerp(CarGraphicsTransform.position, new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), unitsToMove);
        CarGraphicsTransform.transform.rotation = Quaternion.Lerp(CarGraphicsTransform.transform.rotation, this.transform.rotation, myPathFollower.speed <= 1 ? 0.04f : myPathFollower.speed / 30f);
    }

    private void SetMultiplayerPlayerStartingPosition()
    {
        RacingPlayer player = RM.racingMiniGame.MultiplayerManager.racingPlayers.Single(x => x.UserId == PlayerUserId);

        switch (player.playerNumber)
        {
            case 1:
                currentPathSelected = RacingPath.Left;
                break;
            case 2:
                currentPathSelected = RacingPath.Right;
                break;
            default:
                break;
        }
        myPathFollower.OnPathChanged();
        Debug.Log(currentPathSelected);
        UpdatePath();
    }

    private void Update()
    {
        if (!raceStarted)
        {
            CarGraphicsTransform.position = transform.position;
            CarGraphicsTransform.rotation = transform.rotation;
            return;
        }

        MovePlacementImage();

        if (!IsOpponent)
        {
            PlayerMovement();
        }
        else if (!RM.IsMultiplayerMatch)
        {
            AIMovement();
        }
        else
        {
            if (bAccelerate)
            {
                Accelerate();
            }
            else
            {
                Deaccelerate();
            }
        }
    }

    private void MovePlacementImage()
    {
        // 1000 is the max amount the image should move and represents the end of the race, we divide that to the total distance and we get how much should the image move per distance travelled
        this.PlacementSkinParent.GetComponent<RectTransform>().anchoredPosition = new Vector3(myPathFollower.distanceTravelled * placementImageAmountToMovePerMeter, 0, 0);
    }

    private void AIMovement()
    {
        Accelerate();

        if (UnityEngine.Random.Range(0, 100) == 1)
        {
            Move(UnityEngine.Random.Range(CanMoveLeft ? -1 : 1, CanMoveRight ? 1 : -1));
        }
    }

    private void PlayerMovement()
    {
        inputDelay -= Time.deltaTime;

        if (InputMinigame.GetPrimaryButton())
        {
            Accelerate();
        }
        else
        {
            Deaccelerate();
        }


        if (supportsGyroscope && usingGyroscope && inputDelay < 0)
        {
            float x = Input.acceleration.x;
            if (x != 0) //If gyroscope value is 0 check buttons
            {
                Move(x);
            }
            else //Get Buttons Value
            {
                Move(InputMinigame.GetHorizontal());
            }
            return;
        }
        else if (inputDelay < 0)
        {
            Move(InputMinigame.GetHorizontal()); //If doesnt have gyroscope
        }

#if UNITY_EDITOR
        {
            Move(Input.GetAxisRaw("Horizontal")); //Testing in editor
        }
#endif
    }

    private void Deaccelerate()
    {
        if (!canDeaccelerate)
            return;

        if (bAccelerate)
        {
            bAccelerate = false;
            if (RM.IsMultiplayerMatch)
            {
                SimpleSocketManager.Instance.SendActionRacing(RM.racingMiniGame.GetMatchId(), RacingMultiplayerManager.RacingActions.Stop);
            }
        }

        if (myPathFollower.speed <= 0)
        {
            myPathFollower.speed = 0;
            return;
        }

        myPathFollower.speed -= Time.deltaTime * DeaccelerationSpeed;
        Acceleration -= Time.deltaTime;

        if (Acceleration < 0)
            Acceleration = 0;
    }

    private void Accelerate()
    {
        if (myPathFollower.speed > MaxSpeed)
        {
            myPathFollower.speed = MaxSpeed;
            return;
        }

        if (!bAccelerate)
        {
            bAccelerate = true;
            if (RM.IsMultiplayerMatch)
            {
                SimpleSocketManager.Instance.SendActionRacing(RM.racingMiniGame.GetMatchId(), RacingMultiplayerManager.RacingActions.Accelerate);
            }
        }

        Acceleration += Time.deltaTime * AccelerationSpeed;

        if (Acceleration > 1)
            Acceleration = 1;

        myPathFollower.speed += (Time.deltaTime * Speed) * Acceleration;
    }

    private void Move(float x)
    {
        if (x == 0)
            return;

        if (supportsGyroscope && Math.Abs(x) < 0.3f)
        {
            return;
        }

        if (supportsGyroscope)
        {
            inputDelay = inputDelayAux + 0.15f;
        }
        else
        {
            inputDelay = inputDelayAux;
        }
        myPathFollower.lastPath = currentPathSelected;

        switch (currentPathSelected)
        {
            case RacingPath.Left:
                if (x < 0 || !CanMoveLeft)
                {
                    return;
                }
                else
                {
                    if (!CanMoveRight)
                    {
                        return;
                    }
                    if (!IsOpponent && RM.IsMultiplayerMatch)
                    {
                        SimpleSocketManager.Instance.SendActionRacing(RM.racingMiniGame.GetMatchId(), RacingMultiplayerManager.RacingActions.TurnRight);
                    }
                    currentPathSelected = RacingPath.Mid;
                }
                DoCarAnimation(CarAnimations.TurnRight);
                break;
            case RacingPath.Mid:
                if (x < 0)
                {
                    if (!CanMoveLeft)
                    {
                        return;
                    }
                    if (!IsOpponent && RM.IsMultiplayerMatch)
                    {
                        SimpleSocketManager.Instance.SendActionRacing(RM.racingMiniGame.GetMatchId(), RacingMultiplayerManager.RacingActions.TurnLeft);
                    }
                    DoCarAnimation(CarAnimations.TurnLeft);
                    currentPathSelected = RacingPath.Left;
                }
                else
                {
                    if (!CanMoveRight)
                    {
                        return;
                    }
                    if (!IsOpponent && RM.IsMultiplayerMatch)
                    {
                        SimpleSocketManager.Instance.SendActionRacing(RM.racingMiniGame.GetMatchId(), RacingMultiplayerManager.RacingActions.TurnRight);
                    }
                    DoCarAnimation(CarAnimations.TurnRight);
                    currentPathSelected = RacingPath.Right;
                }
                break;
            case RacingPath.Right:
                if (x < 0)
                {
                    if (!CanMoveLeft)
                    {
                        return;
                    }
                    if (!IsOpponent && RM.IsMultiplayerMatch)
                    {
                        SimpleSocketManager.Instance.SendActionRacing(RM.racingMiniGame.GetMatchId(), RacingMultiplayerManager.RacingActions.TurnLeft);
                    }

                    currentPathSelected = RacingPath.Mid;
                }
                else
                {
                    return;
                }
                DoCarAnimation(CarAnimations.TurnLeft);
                break;
        }
        myPathFollower.OnPathChanged();
        UpdatePath();
    }

    private void UpdatePath()
    {
        switch (currentPathSelected)
        {
            case RacingPath.Mid:
                myPathFollower.pathCreator = pathCreatorMid;
                break;
            case RacingPath.Left:
                myPathFollower.pathCreator = pathCreatorLeft;
                break;
            case RacingPath.Right:
                myPathFollower.pathCreator = pathCreatorRight;
                break;
        }
    }

    public void UseGyroscope(bool usingGyroscope)
    {
        this.usingGyroscope = usingGyroscope;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.PowerUp))
        {
            RacingPowerUp powerUp = other.GetComponent<RacingPowerUp>();
            powerUp.carThatPickedUp = this;
            powerUp.EnablePowerup();
        }
        else if (other.CompareTag(Tags.Goal) && raceStarted)
        {
            DoCarAnimation(CarAnimations.Win);
            playerFinished = true;
            if (RM.IsMultiplayerMatch)
            {
                if (!IsOpponent)
                {
                    StartCoroutine(ForceWin());
                    SimpleSocketManager.Instance.SendActionRacing(RM.racingMiniGame.GetMatchId().ToString(), RacingMultiplayerManager.RacingActions.RaceEnd, timeToGoalInFixedFrames.ToString());
                    SimpleSocketManager.Instance.SendCustomActionRacing(RM.racingMiniGame.GetMatchId(), RacingCustomActions.ForceEnd, "end");
                }
            }
            else
            {
                if (IsOpponent)
                {
                    RM.OpponentWon();
                }
                else
                {
                    RM.PlayerWon(CoinsGrabbed);
                }
            }
        }
        if (other.CompareTag(Tags.Coin))
        {
            Destroy(other.gameObject);

            playAudio(eRacingSound.FA_Collect_Coin_1_1);

            if (IsOpponent)
                return;

            CoinsGrabbed++;
            if (CoinsGrabbed < 10)
            {
                coinsText.text = "0" + CoinsGrabbed.ToString();
            }
            else
            {
                coinsText.text = CoinsGrabbed.ToString();
            }
        }
    }

    internal void SetSpeed(float speed, bool setMaxSpeed = false)
    {
        if (!setMaxSpeed)
        {
            this.Speed = speed;
            myPathFollower.speed = speed;
        }
        else
        {
            this.MaxSpeed = speed;
        }
    }

    internal float GetStartMaxSpeed()
    {
        return StartMaxSpeed;
    }
}
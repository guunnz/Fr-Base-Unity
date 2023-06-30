using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Architecture.Injector.Core;
using Audio.SFX;
using CanvasInput.Core.Services;
using CanvasInput.Infrastructure;
using Functional.Maybe;
using JetBrains.Annotations;
using Pathfinding;
using PlayerRoom.Core.Services;
using PlayerRoom.View;
using Socket;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

namespace PlayerMovement
{
    public interface IPlayerWorldData
    {
        public Vector2 Position { get; }
        public Vector2 Destination { get; }
        IObservable<RemotePlayer> OnTapAvatar { get; }
    }

    //public class PlayerMovementManager : RoomViewComponent, IPlayerWorldData
    //{
    //    public Camera cam;

    //    public Transform destination;

    //    // public AstarPath path;
    //    public GameObject playerParent;
    //    public Transform playerFlip;

    //    public AIPath aiPath;
    //    [SerializeField] float startupStepsAmount = 0.1f;
    //    [SerializeField] float chairSittingLerpUnitsAmount = 0.01f;

    //    AvatarAnimationController animationsCache;

    //    static PlayerMovementManager current;

    //    public static PlayerMovementManager Current =>
    //        current ? current : current = FindObjectOfType<PlayerMovementManager>();

    //    public AvatarAnimationController Animations
    //    {
    //        get
    //        {
    //            if (!animationsCache)
    //            {
    //                animationsCache = playerParent.GetComponentInChildren<AvatarAnimationController>();
    //            }

    //            return animationsCache;
    //        }
    //    }

    //    readonly Collider2D[] cols = new Collider2D[256];

    //    readonly CompositeDisposable disposables = new CompositeDisposable();
    //    IRoomMask walkArea;

    //    ICanvasInputSystem input;
    //    PointerEventData pointerEventData;
    //    readonly List<RaycastResult> results = new List<RaycastResult>();


    //    Chair destinationChair;
    //    bool sit;

    //    Seeker seeker;
    //    Seeker Seeker => seeker ??= playerParent.GetComponent<Seeker>();

    //    readonly ISubject<RemotePlayer> onTapAvatar = new Subject<RemotePlayer>();
    //    public IObservable<RemotePlayer> OnTapAvatar => onTapAvatar;

    //    private bool movingStart = false;

    //    const float longTapSeconds = 0.5f;
    //    private float tapTime;
    //    private bool checkTap = true;
    //    private RemotePlayer tappedAvatar;

    //    void OnEnable()
    //    {
    //        ResetDestination();
    //        BindTap();
    //    }

    //    void Awake()
    //    {
    //        Injection.Register<IPlayerWorldData>(this).AddTo(gameObject);
    //    }

    //    void Start()
    //    {
    //        UpdateFootSteps().ToObservable().Subscribe();
    //    }

    //    void BindTap()
    //    {
    //        CreateInputManager();
    //        //input.Tap().Subscribe(OnTapScreen).AddTo(disposables);
    //    }

    //    void CreateInputManager()
    //    {
    //        if (input != null) return;
    //        if (Input.touchSupported)
    //        {
    //            input = new CanvasTouchInput();
    //        }
    //        else
    //        {
    //            input = new CanvasClickInput();
    //        }

    //        input.Enable();
    //    }


    //    void Update()
    //    {
    //        UpdateAnimations();
    //        ControlInputUpdate();
    //    }

    //    public List<string> footStepSounds;
    //    public float timeBetweenSteps;

    //    ISfxPlayer sfxPlayer;

    //    IEnumerator UpdateFootSteps()
    //    {
    //        Injection.SafeGet(ref sfxPlayer);
    //        // wait 2 frames
    //        yield return null;
    //        yield return null;

    //        //yield break;
    //        if (footStepSounds.Count == 0)
    //        {
    //            Debug.LogError("no footstep sound");
    //            yield break;
    //        }

    //        Debug.LogWarning("step begin");
    //        while (gameObject)
    //        {
    //            yield return null;
    //            if (!walk || !enabled || !gameObject.activeSelf || !gameObject.activeInHierarchy)
    //            {
    //                yield return null;
    //                continue;
    //            }

    //            var sfxKey = RandomFootStepKey;
    //            Debug.LogWarning("step -> " + sfxKey);
    //            sfxPlayer.Play(sfxKey);
    //            var wait = new WaitForSeconds(timeBetweenSteps);
    //            yield return wait;
    //        }

    //        Debug.LogWarning("step end");
    //    }

    //    string RandomFootStepKey => footStepSounds[Random.Range(0, footStepSounds.Count)];

    //    bool walk;

    //    void UpdateAnimations()
    //    {
    //        walk = false;
    //        //animations
    //        if (!Animations) return;

    //        if (destinationChair)
    //        {
    //            UpdateDestinationChair();
    //        }


    //        Vector2 vel = aiPath.desiredVelocity;
    //        if (vel.magnitude > 0.01)
    //        {
    //            if (!IsSit)
    //            {
    //                Animations.SetWalkState();
    //                walk = true;
    //                aiPath.canMove = true;
    //                if (Mathf.Abs(vel.x) > 0.2f)
    //                {
    //                    playerFlip.localScale = new Vector3(Mathf.Sign(vel.x), 1, 1);
    //                }
    //            }
    //            else if (aiPath.canMove)
    //            {
    //                destinationChair.SetChairOccupied(true);
    //                aiPath.canMove = false;
    //                Animations.SetSeatState();
    //                playerFlip.localScale = new Vector3(destinationChair.facingRight ? 1 : -1, 1, 1);
    //            }
    //        }
    //        else
    //        {
    //            if (IsSit)
    //            {
    //                Animations.SetSeatState();
    //                playerFlip.localScale = new Vector3(destinationChair.facingRight ? 1 : -1, 1, 1);
    //            }
    //            else
    //            {
    //                Animations.SetIdleState();
    //            }
    //        }
    //    }

    //    bool IsSit => destinationChair && sit;

    //    Vector3 GetChairSitPoint()
    //    {
    //        var buttOffset = Animations.ButtOffset;
    //        if (destinationChair.facingRight)
    //        {
    //            buttOffset.x = -Mathf.Abs(buttOffset.x);
    //        }
    //        else
    //        {
    //            buttOffset.x = +Mathf.Abs(buttOffset.x);
    //        }

    //        return destinationChair.SitPoint - buttOffset;
    //    }

    //    void UpdateDestinationChair()
    //    {
    //        var dest = GetChairSitPoint();
    //        Debug.DrawLine(dest + Vector3.right, dest - Vector3.left);
    //        Debug.DrawLine(dest + Vector3.down, dest - Vector3.up);
    //        destination.position = dest;

    //        if (destinationChair.IsOccupied())
    //        {
    //            return;
    //        }
    //        if (Vector2.Distance(Position, dest) < destinationChair.GetChairSittingLeaway())
    //        {
    //            Seeker.enabled = false;
    //            sit = true;
    //            SetPosition(dest);
    //        }
    //    }

    //    public bool IsPointerOverGameObject()
    //    {
    //        //check mouse
    //        if (EventSystem.current.IsPointerOverGameObject())
    //        {
    //            return true;
    //        }

    //        // check touch
    //        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
    //        {
    //            if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
    //            {
    //                return true;
    //            }
    //        }

    //        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
    //        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    //        List<RaycastResult> results = new List<RaycastResult>();
    //        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
    //        return results.Count > 0;
    //    }

    //    bool CanWalkOnScreenPoint(Vector2 screenPoint, out Vector2 worldPoint)
    //    {
    //        worldPoint = cam.ScreenToWorldPoint(screenPoint);
    //        return walkArea.ColorDistance(Color.black, worldPoint) < 0.1;
    //    }

    //    bool _isClickOverUI = false; //Prevent walk over UI (we make it true on mousedown and ask state on mouseup)

    //    void ControlInputUpdate()
    //    {
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            _isClickOverUI = IsPointerOverGameObject();
    //            tapTime = 0f;

    //            if (!_isClickOverUI && checkTap)
    //            {
    //                tappedAvatar = IsTapOverAvatar();
    //            }
    //        }

    //        if (Input.GetMouseButton(0))
    //        {
    //            if (checkTap)
    //            {
    //                var lastTapped = IsTapOverAvatar();

    //                if (lastTapped != null && tappedAvatar.remotePlayerId.Equals(lastTapped.remotePlayerId))
    //                {
    //                    tapTime += Time.deltaTime;

    //                    if (tapTime >= longTapSeconds)
    //                    {
    //                        checkTap = false;
    //                        onTapAvatar.OnNext(tappedAvatar);
    //                    }
    //                }
    //                else
    //                {
    //                    checkTap = false;
    //                }
    //            }
    //        }

    //        if (Input.GetMouseButtonUp(0))
    //        {
    //            if (!_isClickOverUI)
    //            {
    //                OnTapScreen();
    //            }

    //            _isClickOverUI = false;
    //            checkTap = true;
    //        }
    //    }

    //    RemotePlayer IsTapOverAvatar()
    //    {
    //        var world = cam.ScreenToWorldPoint(Input.mousePosition);

    //        var count = Physics2D.OverlapCircleNonAlloc(world, 0.01f, cols);

    //        for (int i = 0; i < count; i++)
    //        {
    //            if (cols[i].TryGetComponent<RemotePlayer>(out var remotePlayer))
    //            {
    //                return remotePlayer;
    //            }
    //        }

    //        return null;
    //    }

    //    void OnTapScreen()
    //    {
    //        if (IsPointerOverGameObject())
    //        {
    //            return;
    //        }

    //        if (movingStart)
    //        {
    //            return;
    //        }

    //        CanWalkOnScreenPoint(Input.mousePosition, out var world);
    //        //if (CanWalkOnScreenPoint(Input.mousePosition, out var world))
    //        //{
    //        //    Debug.Log("Touch outside mask");
    //        //    return;
    //        //}

    //        Debug.Log("Touch over mask ok !! ");

    //        var count = Physics2D.OverlapCircleNonAlloc(world, 0.01f, cols);

    //        if (destinationChair != null)
    //            destinationChair.SetChairOccupied(false);

    //        destinationChair = default;
    //        for (int i = 0; i < count; i++)
    //        {
    //            if (cols[i].TryGetComponent<RoomItem>(out var item))
    //            {
    //                Debug.Log($"Tap over item {item.name}", item);
    //                if (item is Chair chair)
    //                {
    //                    if(chair.IsOccupied())
    //                    {
    //                        break;
    //                    }
    //                    destinationChair = chair;
    //                }
    //            }
    //        }

    //        sit = false;
    //        destination.position = world;
    //    }

    //    protected override void DidLoadRoom()
    //    {
    //        disposables.Clear();

    //        walkArea = Injection.Get<IRoomMask>();
    //        ResetDestination();
    //        BindTap();
    //    }

    //    void ResetDestination()
    //    {
    //        destination.position = Position;
    //    }

    //    void OnDisable()
    //    {
    //        disposables.Clear();
    //    }

    //    public Vector2 Position => playerFlip.transform.position;
    //    public Vector2 Destination => destination.position;

    //    public void SetPosition(Vector3 pos)
    //    {
    //        Seeker.transform.position = pos;
    //        destination.position = pos;
    //    }


    //    public void MoveStart()
    //    {
    //        StartCoroutine(IMoveStart());
    //    }

    //    IEnumerator IMoveStart()
    //    {
    //        movingStart = true;
    //        yield return new WaitForSeconds(1f);
    //        destination.position = new Vector3(
    //            Seeker.transform.position.x + Random.Range(-startupStepsAmount, startupStepsAmount),
    //            seeker.transform.position.y + Random.Range(-startupStepsAmount, startupStepsAmount),
    //            seeker.transform.position.z + Random.Range(-startupStepsAmount, startupStepsAmount));
    //        movingStart = false;
    //    }
    //}
}
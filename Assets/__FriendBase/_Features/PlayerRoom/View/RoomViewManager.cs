using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using Audio.SFX;
using Data;
using MemoryStorage.Core;
using Pathfinding;
using PlayerMovement;
using PlayerRoom.Core.Actions;
using PlayerRoom.Core.Domain;
using PlayerRoom.Core.Services;
using PlayerRoom.Delivery;
using Shared.Utils;
using Socket;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerRoom.View
{
    public class RoomViewManager : MonoBehaviour, IRoomLoader
    {
        // --- serialized fields ---
        // shows the list of available rooms
        public Button showListButton;
        public Button showChatButton;
        public Button goHomeButton;
        public Button furnitureStoreButton;

        // the "top bar" used now for testing purpouses
        public GameObject topBar;

        // the scriptable object that holds the information of the rooms
        public RoomsData data;

        // the room components to work with
        public List<RoomViewComponent> components;

        // the list of available rooms
        public RoomListPopup roomListPopup;
        public ChatView.View.ChatView chatView;

        // the player movement object (that holds as parent the view of the player)
        public GameObject playerParent;
        private IPlayerWorldData playerWorldData;

        //variables        
        readonly CompositeDisposable disposables = new CompositeDisposable();
        GetLastRoom getLastRoom;
        IPlayerRoomStateManager state;
        List<RoomInfo> cacheRoomsInfo;
        public AvatarCustomizationController avatarPrefab;

        AvatarCustomizationController avatarController;

        [SerializeField] AstarPath pathFindingPrefab;
        [SerializeField] Transform pathfindingParent;

        IGameData gameData;

        ISocketManager socketManager;
        IMemoryStorage memoryStorage;
        RemotePlayersPool playersPool;
        IViewManager viewManager;
        ISfxPlayer sfxPlayer;
        static public bool comingFromTransition;


        void Awake()
        {
            gameData = Injection.Get<IGameData>();
            Injection.Register<IRoomLoader>(this);
        }

        string CurrentRoomName
        {
            get => memoryStorage.Get("currentRoomName");
            set => memoryStorage.Set("currentRoomName", value);
        }

        string CurrentRoomId
        {
            get => memoryStorage.Get("currentRoomId");
            set => memoryStorage.Set("currentRoomId", value);
        }

        void Start()
        {
            Injection.Get(out getLastRoom);
            Injection.Get(out state);
            Injection.Get(out memoryStorage);
            Injection.Get(out socketManager);
            playersPool = FindObjectOfType<RemotePlayersPool>();
            playerWorldData = Injection.Get<IPlayerWorldData>();

            for (var i = components.Count - 1; i >= 0; i--)
            {
                components[i].Write();
            }

            for (var i = components.Count - 1; i >= 0; i--)
            {
                components[i].Read();
            }

            if (!string.IsNullOrEmpty(CurrentRoomName) && !string.IsNullOrEmpty(CurrentRoomId))
            {
                LoadRoom(CurrentRoomName).Subscribe();
            }
            else
            {
                LoadRoom(data.defaultRoom).Subscribe();
            }

            chatView.gameObject.SetActive(true);

            showListButton.OnClickAsObservable().Subscribe(ClickShowList).AddTo(disposables);
            showChatButton.OnClickAsObservable().Subscribe(ClickShowChat).AddTo(disposables);
            goHomeButton.OnClickAsObservable().Subscribe(GoHome).AddTo(disposables);
        }

        void ClickShowList()
        {
            var willShowThePopup = !roomListPopup.gameObject.activeSelf;
            roomListPopup.gameObject.SetActive(willShowThePopup);
        }

        void ClickShowChat()
        {
            chatView.ShowChat();
        }

        void GoHome()
        {
            playersPool.ClearRemotesPool();
            socketManager.LeaveCurrentChatRoom()
                .Do(() => CurrentRoomId = string.Empty)
                .Do(() => CurrentRoomName = data.defaultRoom)
                .Do(() => playersPool.ClearRemotesPool())
                .SelectMany(_ => LoadRoom(data.defaultRoom))
                .Subscribe();
        }

        void Update()
        {
            topBar.SetActive(!roomListPopup.gameObject.activeSelf && !chatView.IsVisible);
        }

        void OnLoadRoom(string roomId)
        {
            state.CurrentRoomId = roomId;
            for (var i = components.Count - 1; i >= 0; i--)
            {
                components[i].LoadRoom(roomId);
            }
        }

        public async Task ConnectNewRoomAsync(string room, string instanceId)
        {
            Debug.Log($"Connect to chat room --> {room} :: {instanceId}");

            if (!string.IsNullOrEmpty(CurrentRoomName) && !string.IsNullOrEmpty(CurrentRoomId))
            {
                await socketManager.LeaveChatRoom(CurrentRoomName, CurrentRoomId);
            }

            var pos = playerWorldData.Position;

            await socketManager.JoinChatRoom(room, instanceId, pos.x, pos.y);

            //cache new room info
            CurrentRoomName = room;
            CurrentRoomId = instanceId;

            await LoadRoom(room);
        }

        public IObservable<Unit> LoadRoom(string id)
        {
            chatView.SetMessageField();

            if (!comingFromTransition && !string.IsNullOrEmpty(CurrentRoomName))
            {
                DestroyAndLoadRoom();
            }
            else
            {
                comingFromTransition = false;
            }

            var fixedData = GetComponentsInChildren<RoomFixedData>(true);

            foreach (var part in fixedData)
            {
                // part.SetActive(part.RoomId == id);
                Debug.Log(part.RoomId == id ? $"<color=green> {id} </color>" : $"<color=red> {id} </color>");
                if (part.RoomId.ToLower() == id.ToLower())
                {
                    SpawnPointManager.Singleton.SetSpawnPoint(part.spawnPoint);
                    //GetComponentInChildren<PlayerMovementManager>().SetPosition(part.spawnPoint.position);
                    break;
                }
            }

            return LoadRoomAsync(id).ToObservable().ObserveOnMainThread();
        }


        private async Task LoadRoomAsync(string id)
        {
            Debug.Log($"start loading loaded room id {id}");

            await LoadRoomRoutine(id).ToObservable();

            DestroyAvatarIfExists();

            var parts = GetComponentsInChildren<IRoomPart>(true);
            foreach (var t in parts)
            {
                Debug.Log(t.RoomId + " -> " + id);
                t.SetActive(string.Equals(t.RoomId, id, StringComparison.CurrentCultureIgnoreCase));
            }


            await ScanRoutine(id).ToObservable();

            var avatarData = gameData.GetUserInformation().GetAvatarCustomizationData().GetSerializeData();
            DestroyAvatarIfExists();

            playerParent.gameObject.DestroyChildren();


            avatarController = Instantiate(avatarPrefab, playerParent.transform);
            const float theRightSize = 0.123f;
            avatarController.transform.localScale = Vector3.one * theRightSize;
            avatarController.SetAvatarCustomizationData(avatarData);
            //GetComponentInChildren<PlayerMovementManager>().MoveStart();
            showChatButton.gameObject.SetActive(!id.Equals(data.defaultRoom));
            //chatView.ShowChatButton(!id.Equals(data.defaultRoom));
            goHomeButton.gameObject.SetActive(!id.Equals(data.defaultRoom));
            furnitureStoreButton.gameObject.SetActive(id.Equals(data.defaultRoom));

            Debug.Log($"loaded room id {id}");
        }


        public void DestroyAndLoadRoom()
        {
            viewManager ??= Injection.Get<IViewManager>();
            viewManager.DebugGetOut("roomtransition");
            Debug.Log("TRANSITION");
            comingFromTransition = true;
        }

        IEnumerator ScanRoutine(string id)
        {
            var pathsInfo = Resources.Load<TextAsset>(id + "-path");

            ActivePlayer(false);

            yield return null;
            yield return null;

            pathfindingParent.gameObject.DestroyChildren();

            yield return null;
            yield return null;

            var instancePath = Instantiate(pathFindingPrefab, pathfindingParent);
            instancePath.data.file_cachedStartup = pathsInfo;
            instancePath.data.cacheStartup = true;
            instancePath.gameObject.SetActive(true);
            Debug.Log("Load data " + id + " ->> ", pathsInfo);

            yield return null;
            yield return null;
            yield return null;
            ActivePlayer(true);
            yield return null;
            yield return null;
        }

        static void ActivePlayer(bool active)
        {
            FindObjectOfType<Seeker>(true).gameObject.SetActive(active);
        }

        void DestroyAvatarIfExists()
        {
            if (avatarController)
            {
                Destroy(avatarController.gameObject);
            }
        }

        IEnumerator LoadRoomRoutine(string id)
        {
            OnLoadRoom(id);
            Debug.Log("loadded ?");
            yield return null;
        }

        List<RoomInfo> CreateRoomsInfo()
        {
            return data.GetList()
                .Where(item => item.publicArea)
                .Select(item => new RoomInfo {AreaId = item.Id, RoomName = item.roomName})
                .ToList();
        }

        public List<RoomInfo> Rooms => cacheRoomsInfo ??= CreateRoomsInfo();
    }
}
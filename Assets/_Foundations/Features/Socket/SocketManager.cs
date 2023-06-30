// ReSharper disable ConditionIsAlwaysTrueOrFalse

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using AuthFlow;
using AuthFlow.EndAuth.Repo;
using JetBrains.Annotations;
using NativeWebSocket;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;

//#define NO_CONNECTION // uncomment to test without socket connection

namespace Socket
{
    [UsedImplicitly]
    public class SocketManager : ISocketManager, IDisposable
    {
        int tryConnectAttempts = 10;

        WebSocket websocket;

        readonly SocketStateManager state = new SocketStateManager();

        readonly ISubject<byte[]> receivedMessage = new Subject<byte[]>();
        readonly ISubject<string> socketError = new Subject<string>();

        readonly ISubject<WebSocketCloseCode> connectionClosed = new Subject<WebSocketCloseCode>();
        public IObservable<byte[]> OnMessage() => receivedMessage;
        public IObservable<string> OnError() => socketError;
        public IObservable<WebSocketCloseCode> OnClose() => connectionClosed;

        readonly CompositeDisposable disposables = new CompositeDisposable();
        ILocalUserInfo userInfo;

        public IObservable<Unit> Connect()
        {
            return ConnectSocket().ToObservable().ObserveOnMainThread()
                // ReSharper disable once ConvertClosureToMethodGroup
                .DoOnError(error => { Debug.LogError(error); });
        }

        void InitDependencies()
        {
            Injection.SafeGet(ref userInfo);
        }

        async Task ConnectSocket()
        {
            //if the socket is instantiated, we are here because of an error
            if (websocket != null)
            {
                Debug.LogWarning("Socket is already connected");
                return;
            }

            //first init socket dependencies
            InitDependencies();
            //and then perform the connection (and wait for it)
            await PerformConnection();


            HeartBeatRoutine().ToObservable().Subscribe().AddTo(disposables);

            const int reconnectionAttempts = 3;


            // the first <reconnectionAttempts> times connection just closes will fall here
            OnClose()
                .Take(reconnectionAttempts) // just accept the first N emitions 
                .Do(_ => Debug.LogError("try again reconect"))
                .SelectMany(_ => TryReconnect().ToObservable())
                .Subscribe()
                .AddTo(disposables);
            // then will fall here
            OnClose()
                .Skip(reconnectionAttempts) // skips the first N emitions
                .Do(_ => Debug.LogError(reconnectionAttempts + " attempts to reconect -> nuking"))
                .Do(_ => JesseUtils.ConnectionError())
                .Subscribe()
                .AddTo(disposables);

            Observable.EveryUpdate().Subscribe(DoOnUpdate).AddTo(disposables);
            Observable.OnceApplicationQuit().Subscribe(OnDispose).AddTo(disposables);


#if UNITY_EDITOR
            //just to test disconnections
            Observable.EveryUpdate().Subscribe(() =>
            {
                if (Input.GetKeyDown(KeyCode.U))
                {
                    websocket.Close();
                }
            }).AddTo(disposables);
#endif
        }


        IEnumerator HeartBeatRoutine()
        {
            //heartbeat to keep connection alive

            const float minSecondsBetweenHeartBeats = 1f;
            const float maxSecondsBetweenHeartBeats = 3f;

            while (websocket != null)
            {
                var momentBefore = Time.time;

                yield return SendHeartbeat().ToYieldInstruction();

                yield return null; //JIC wait a frame

                var momentAfter = Time.time;
                var timeLapse = momentAfter - momentBefore;
                var desiredTimeToWait = maxSecondsBetweenHeartBeats - timeLapse;
                var timeToWait = Mathf.Max(minSecondsBetweenHeartBeats, desiredTimeToWait);

                yield return new WaitForSeconds(timeToWait);
            }
        }


        async Task PerformConnection()
        {
            var token = userInfo["firebase-login-token"];

            Debug.Log($"TOKEN : <{token}>");

            websocket = new WebSocket(Constants.SocketUrl, new Dictionary<string, string> {{"x-authorization", token}});
            websocket.OnMessage += receivedMessage.OnNext;
            websocket.OnMessage += OnReceivedMessage;
            websocket.OnError += Debug.LogError;
            websocket.OnError += socketError.OnNext;
            websocket.OnClose += _ => { Debug.Log("<color=red>web socket closes </color>"); };
            websocket.OnClose += connectionClosed.OnNext;

            Debug.Log(" <color=magenta> >> Connecting to socket ... <<  </color>");


#if NO_CONNECTION
            Debug.Log("<color=red> Alert! Socket is in mode NoConnection, change the flag to make it work!! </color> ");
#else
            //attempt connect webSocket
            websocket.Connect().ToObservable().ObserveOnMainThread()
                .DoOnError(HandleConnectionError)
                .Subscribe().AddTo(disposables);

            await Observable.QuickObservable(obs => websocket.OnOpen += obs.OnCompleted).ObserveOnMainThread();

            Debug.Log("<color=green> Socket Connection Succeed </color> ");
#endif
        }

        private void OnReceivedMessage(byte[] data)
        {
            var message = System.Text.Encoding.UTF8.GetString(data);
            //Debug.Log("********* ONMESSAGE:" + message);
        }

        void HandleConnectionError(Exception er)
        {
            //if there is an error connecting

            //remove one available attempt
            tryConnectAttempts--;


            if (tryConnectAttempts == 0)
            {
                //if there is no more attempts the reboot the application
                Debug.LogError(er.Message + " reloading ... ");
                JesseUtils.Nuke();
                return;
            }

            Debug.LogError(er.Message + " attempting reconection");
            //if there is available attempts then try to reconnect again
            TryReconnect()
                .ToObservable()
                .ObserveOnMainThread()
                .Subscribe();
        }

        void DoOnUpdate()
        {
            if (Application.isPlaying)
            {
                websocket.DispatchMessageQueue();
            }
        }

        void OnDispose()
        {
            disposables.Clear();
        }

        async Task TryReconnect()
        {
            //first do the connection process
            await PerformConnection();
            // then if player was already on a room try to join again

            if (state.OnRoom())
            {
                var pos = state.position ?? default;
                JoinChatRoom(state.chatRoomName, state.chatRoomId, pos.x, pos.y);
            }
        }

        // method to make an event to send to phoenix backend

        public IObservable<Unit> SendEvent(string topic, string eventType, JObject payload, string eventRef = "none")
        {
#if (NO_CONNECTION)
            {
                return Observable.ReturnUnit();
            }
#endif
            var eventData = new JObject
            {
                ["topic"] = topic,
                ["event"] = eventType,
                ["payload"] = payload,
                ["ref"] = eventRef == "none" ? $"{topic}_{eventType}" : eventRef
            };

            return websocket.SendText(eventData.ToString()).ToObservable().ObserveOnMainThread();
        }

        // overload of the method above so it can be called from other classes without them needing to add "using Newtonsoft.Json.Linq;" to all files
        public IObservable<Unit> SendEvent(string topic, string eventType, string payloadString,
            string eventRef = "none")
        {
            return SendEvent(topic, eventType, JObject.Parse(payloadString), eventRef);
        }

        // methods for common events that will be needed on friendbase(plus heartbeat that is always needed for phoenix sockets to keep the connection alive)
        IObservable<Unit> SendHeartbeat()
        {
            return SendEvent("phoenix", "heartbeat", "{}", "heartbeat_ref");
        }

        public IObservable<Unit> JoinChatRoom(string roomName, string roomId, float positionX, float positionY)
        {
            Debug.Log("****** roomName:"+ roomName);
            Debug.Log("****** roomId:" + roomId);
            var pos = new Vector2(positionX, positionY);
            var topic = $"chat_room:{roomName}:{roomId}";
            var payload = new JObject
            {
                ["position_x"] = pos.x,
                ["position_y"] = pos.y
            };
            var eventRef = $"join_{roomId}";
            return SendEvent(topic, "phx_join", payload, eventRef)
                .Do(() => state.OnJoinChatRoom(roomName, roomId, pos));
        }

        public IObservable<Unit> SendMessage(string roomName, string roomId, string content)
        {
            return SendEvent($"chat_room:{roomName}:{roomId}", "message", new JObject {["content"] = content},
                $"message_{roomId}");
        }

        public IObservable<Unit> SendChatMessage(string roomName, string roomId, string content, string username, string usernameColor)
        {
            return SendEvent($"chat_room:{roomName}:{roomId}",
                "message",
                new JObject
                {
                    ["content"] = content,
                    ["username"] = username,
                    ["usernameColor"] = usernameColor
                },
                $"message_{roomId}");
        }

        public IObservable<Unit> GetPlayerPositions(string roomName, string roomId, bool forceRefesh = false)
        {
            return SendEvent($"chat_room:{roomName}:{roomId}", "get_positions",
                new JObject {["force_refresh"] = forceRefesh}, $"message_{roomId}");
        }

        public IObservable<Unit> UpdatePlayerPosition(string roomName, string roomId, float positionX, float positionY)
        {
            return SendEvent($"chat_room:{roomName}:{roomId}", "change_position",
                new JObject {["position_x"] = positionX, ["position_y"] = positionY}, $"message_{roomId}");
        }

        public IObservable<Unit> UpdatePlayerDestination(string roomName, string roomId, float positionX,
            float positionY)
        {
            return SendEvent($"chat_room:{roomName}:{roomId}", "change_destination",
                new JObject {["destination_x"] = positionX, ["destination_y"] = positionY}, $"message_{roomId}");
        }

        public IObservable<Unit> UpdatePlayerState(string roomName, string roomId, string state)
        {
            return SendEvent($"chat_room:{roomName}:{roomId}", "change_player_state",
                new JObject {["state"] = state}, $"message_{roomId}");
        }

        public IObservable<Unit> LeaveChatRoom(string roomName, string roomId)
        {
            return SendEvent($"chat_room:{roomName}:{roomId}", "phx_leave", "{}", $"leave_{roomId}")
                .Do(_ => state.Clear());
        }

        public IObservable<Unit> LeaveCurrentChatRoom()
        {
            var (roomName, roomId) = (state.chatRoomName, state.chatRoomId);
            return SendEvent($"chat_room:{roomName}:{roomId}", "phx_leave", "{}", $"leave_{roomId}")
                .Do(_ => state.Clear());
        }

        public void Dispose()
        {
            try
            {
                disposables?.Clear();
                websocket?.Close();
                state.Clear();
            }
            catch
            {
                // ignored
            }
        }
    }
}
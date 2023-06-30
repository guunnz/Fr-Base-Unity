using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using System.Reflection;
using System.Threading.Tasks;
using AuthFlow;
using AuthFlow.EndAuth.Repo;
using Newtonsoft.Json.Linq;
using UnityEngine;
using NativeWebSocket;
using DebugConsole;
using Data;
using UnityEngine.SceneManagement;
using Data.Users;
using System.Net.NetworkInformation;

namespace Socket
{
    public class SimpleSocketManager : GenericSingleton<SimpleSocketManager>, ISimpleSocketManager
    {
        public delegate void CloseConnection();
        public event CloseConnection OnCloseConnection;

        public delegate void UserBanned();
        public event UserBanned OnUserBanned;

        public delegate void UserSuspended();
        public event UserSuspended OnUserSuspended;

        private Dictionary<string, List<Action<AbstractIncomingSocketEvent>>> socketEventDeliveries;

        private WebSocket websocket;
        readonly SocketStateManager state = new SocketStateManager();
        int retriesLeft = 3;
        private IDebugConsole debugConsole;
        private IncomingEventManager incomingEventManager;

        private bool flagUseNewSystem = true;
        private Coroutine _hearthbeatCoroutine;
        private bool hasCreatedPrivateChatSocket = false;

        void Start()
        {
            debugConsole = Injection.Get<IDebugConsole>();
            incomingEventManager = new IncomingEventManager();
            socketEventDeliveries = new Dictionary<string, List<Action<AbstractIncomingSocketEvent>>>();

            Suscribe(SocketEventTypes.USER_STATUS, OnUserChangeStatus);
        }

        private void OnUserChangeStatus(AbstractIncomingSocketEvent incomingEvent)
        {
            IncomingEventUserStatus eventUserStatus = incomingEvent as IncomingEventUserStatus;

            if (eventUserStatus == null)
            {
                return;
            }

            if (eventUserStatus.UserStatus.UserId != Injection.Get<IGameData>().GetUserInformation().UserId)
            {
                //It is not me
                return;
            }

            Injection.Get<IGameData>().GetUserInformation().UserStatus = eventUserStatus.UserStatus;


            CheckBanStatus(eventUserStatus.UserStatus);

            if (eventUserStatus.UserStatus.IsSuspended())
            {
                if (OnUserSuspended != null)
                {
                    OnUserSuspended();
                }
            }
        }

        public void CheckBanStatus(UserAccountStatus userStatus)
        {
            if (userStatus.IsBanned())
            {
                SceneManager.LoadScene(GameScenes.BannedScene, LoadSceneMode.Additive);
                if (OnUserBanned != null)
                {
                    OnUserBanned();
                }
                Disconnect(true);
            }
        }

        public void Connect()
        {
            if (websocket != null)
            {
                Disconnect(true);
            }

            //Create Header with Auth
            if (Injection.Get<ILocalUserInfo>() == null)
                return;
            ILocalUserInfo userInfo = Injection.Get<ILocalUserInfo>();
            var token = userInfo["firebase-login-token"];
            var headers = new Dictionary<string, string>();
            headers.Add("x-authorization", token);

            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_INFO)) debugConsole.TraceLog(LOG_TYPE.SOCKET_INFO, "CONNECT:" + token);

            //Callbacks and Connect
            websocket = new WebSocket(Constants.SocketUrl, headers);

            websocket.OnMessage += OnMessageReceive;
            websocket.OnError += HandleConnectionError;
            websocket.OnClose += OnCloseSocket;
            websocket.OnOpen += OnConnect;

            websocket.Connect();
        }

        private void OnConnect()
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_INFO)) debugConsole.TraceLog(LOG_TYPE.SOCKET_INFO, "CONNECTION SUCCEED: " + websocket.GetHashCode());
            if (_hearthbeatCoroutine != null)
            {
                StopCoroutine(_hearthbeatCoroutine);
            }
            _hearthbeatCoroutine = StartCoroutine(SendHeartbeat());
        }

        private void OnCloseSocket(WebSocketCloseCode closeCode)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_INFO)) debugConsole.TraceLog(LOG_TYPE.SOCKET_INFO, "CONNECTION CLOSED:" + closeCode);
            OnCloseSocket();
        }

        private void OnCloseSocket()
        {
            if (OnCloseConnection != null)
            {
                OnCloseConnection();
            }
            Disconnect(true);
            Injection.Get<ILoading>().Load();
            JesseUtils.ConnectionError();
        }

        private void OnMessageReceive(byte[] data)
        {
            var message = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log(message);
            AbstractIncomingSocketEvent incomingSocketEvent = incomingEventManager.GetIncomingSocketEvent(message);
            if (incomingSocketEvent != null)
            {
                if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_IN)) debugConsole.TraceLog(LOG_TYPE.SOCKET_IN, "ONMESSAGE: " + incomingSocketEvent.EventType + " " + message);
                DeliverSocketEventToSuscribers(incomingSocketEvent);
            }
        }

        void Update()
        {
            if (websocket != null)
            {
                websocket.DispatchMessageQueue();
            }
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.U))
            //{
            //    OnCloseSocket();
            //    Debug.Log("CLOSE CONNECTION");
            //}

            //if (Input.GetKeyDown(KeyCode.E))
            //{
            //    Connect();
            //}

            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log(websocket == null);
            }
#endif
        }

        IEnumerator SendHeartbeat()
        {
            while (true)
            {
                // methods for common events that will be needed on friendbase(plus heartbeat that is always needed for phoenix sockets to keep the connection alive)
                SendEvent("phoenix", "heartbeat", new JObject(), "heartbeat_ref");
                yield return new WaitForSeconds(25);
            }
        }

        private void OnApplicationQuit()
        {
            Disconnect(true);
        }

        public void Disconnect(bool hardDisconnect = false)
        {
            if (_hearthbeatCoroutine != null)
            {
                StopCoroutine(_hearthbeatCoroutine);
                _hearthbeatCoroutine = null;
            }

            if (websocket != null)
            {
                websocket.Close();
                websocket.OnMessage -= OnMessageReceive;
                websocket.OnError -= HandleConnectionError;
                websocket.OnClose -= OnCloseSocket;
                websocket.OnOpen -= OnConnect;
            }

            hasCreatedPrivateChatSocket = false;
            websocket = null;
            incomingEventManager.OnCloseConnection();
            hasCreatedPrivateChatSocket = false;
        }

        void HandleConnectionError(string er)
        {
            //if there is an error connecting
            //remove one available attempt
            retriesLeft--;
            if (retriesLeft == 0)
            {
                //if there is no more attempts the reboot the application
                Debug.LogError(er + " reloading ... ");
                return;
            }
            Debug.LogError(er + " attempting reconection");
            //if there is available attempts then try to reconnect again
            
        }

        public void JoinChatRoom(string roomName, string roomId, float positionX, float positionY)
        {
            // Supermarket
            // 7334bdf6-f63d-49ec-a37a-2a025172b789
            // 7.6
            // -1.52
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"JoinChatRoom roomName:{roomName} roomId:{roomId}");

            Vector2 pos = new Vector2(positionX, positionY);

            string topic = $"chat_room:{roomName}:{roomId}"; //This is old room socket system
            if (flagUseNewSystem)
            {
                topic = $"room:{roomId}"; //This point to new room socket system
            }

            JObject payload = new JObject
            {
                ["position_x"] = pos.x,
                ["position_y"] = pos.y
            };
            var eventRef = $"join_{roomId}";

            SendEvent(topic, "phx_join", payload, eventRef);
        }

        void SendEvent(string topic, string eventType, JObject payload, string eventRef = "none")
        {
            if (websocket != null && websocket.State == WebSocketState.Open)
            {
                var eventData = new JObject
                {
                    ["topic"] = topic,
                    ["event"] = eventType,
                    ["payload"] = payload,
                    ["ref"] = eventRef == "none" ? $"{topic}_{eventType}" : eventRef
                };

                websocket.SendText(eventData.ToString());
            }
            else
            {
                OnCloseSocket();
            }
        }
        public void SendChatMessage(string roomName, string roomId, string content, string username, string usernameColor)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"SendChatMessage roomName:{roomName} roomId:{roomId} content:{content} username:{username}");

            string topic = $"chat_room:{roomName}:{roomId}"; //This is old room socket system
            if (flagUseNewSystem)
            {
                topic = $"room:{roomId}"; //This point to new room socket system
            }

            SendEvent(topic,
                "message",
                new JObject
                {
                    ["content"] = content,
                    ["username"] = username,
                    ["usernameColor"] = usernameColor
                },
                $"message_{roomId}");
        }

        public void SendAvatarMove(string roomName, string roomId, float positionX, float positionY)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"SendAvatarMove roomName:{roomName} roomId:{roomId} positionX:{positionX} positionY:{positionY}");

            string topic = $"chat_room:{roomName}:{roomId}"; //This is old room socket system
            if (flagUseNewSystem)
            {
                topic = $"room:{roomId}"; //This point to new room socket system
            }

            SendEvent(topic, "change_position", new JObject { ["position_x"] = positionX, ["position_y"] = positionY }, $"message_{roomId}");
        }


        public void SendAvatarStatus(string roomName, string roomId, float positionX, float positionY, int orientation, string state)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"SendAvatarState roomName:{roomName} roomId:{roomId} positionX:{positionX} positionY:{positionY} state:{state}");

            string topic = $"chat_room:{roomName}:{roomId}"; //This is old room socket system
            if (flagUseNewSystem)
            {
                topic = $"room:{roomId}"; //This point to new room socket system
            }

            SendEvent(topic, "change_player_state", new JObject { ["position_x"] = positionX, ["position_y"] = positionY, ["state"] = state }, $"message_{roomId}");
        }

        public void LeaveChatRoom(string roomName, string roomId)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"LeaveChatRoom roomName:{roomName} roomId:{roomId}");

            string topic = $"chat_room:{roomName}:{roomId}"; //This is old room socket system
            if (flagUseNewSystem)
            {
                topic = $"room:{roomId}"; //This point to new room socket system
            }

            SendEvent(topic, "phx_leave", new JObject(), $"leave_{roomId}");
        }

        public void SendFurnitureAdd(string roomName, string roomId, int idInstance, int idItem, float positionx, float positiony, int orientation)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"AddFurniture roomId:{roomId} idItem:{idItem} idInstance:{idInstance} positionx{positionx} positiony{positiony}");

            string topic = $"chat_room:{roomName}:{roomId}"; //This is old room socket system
            if (flagUseNewSystem)
            {
                topic = $"room:{roomId}"; //This point to new room socket system
            }

            SendEvent(topic, "set_furniture", new JObject
            {
                ["inventory_item_id"] = idInstance,
                ["item_id"] = idItem,
                ["position_x"] = positionx,
                ["position_y"] = positiony,
                ["orientation"] = orientation
            },
            $"message_{roomId}");
        }

        public void SendFurnitureMove(string roomName, string roomId, int furnitureIdInstance, float positionx, float positiony, int orientation)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"MoveFurniture roomName:{roomName} roomId:{roomId} idInstance{furnitureIdInstance} positionx{positionx} positiony{positiony}");

            string topic = $"chat_room:{roomName}:{roomId}"; //This is old room socket system
            if (flagUseNewSystem)
            {
                topic = $"room:{roomId}"; //This point to new room socket system
            }

            SendEvent(topic, "move_furniture", new JObject
            {
                ["room_furniture_id"] = furnitureIdInstance,
                ["position_x"] = positionx,
                ["position_y"] = positiony,
                ["orientation"] = orientation
            },
            $"message_{roomId}");
        }

        public void SendCurrentPet(string roomName, string roomId, int? petId, int? petIdInGame)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"SendCurrentPet roomName:{roomName} roomId:{roomId} pet_item_id:{petId}");

            string topic = $"chat_room:{roomName}:{roomId}"; //This is old room socket system
            if (flagUseNewSystem)
            {
                topic = $"room:{roomId}"; //This point to new room socket system
            }

            if (petId == null)
            {
                SendEvent(topic, "player_pet_change", new JObject
                {
                },
               $"message_{roomId}");
            }
            else
            {
                SendEvent(topic, "player_pet_change", new JObject
                {
                    ["pet_item_id"] = petId.Value,
                    ["pet_item_id_in_game"] = petIdInGame.Value,
                },
              $"message_{roomId}");
            }
        }

        public void SendFurnitureRemove(string roomName, string roomId, int furnitureIdInstance)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"RemoveFurniture roomName:{roomName} roomId:{roomId} idInstance{furnitureIdInstance}");

            string topic = $"chat_room:{roomName}:{roomId}"; //This is old room socket system
            if (flagUseNewSystem)
            {
                topic = $"room:{roomId}"; //This point to new room socket system
            }

            SendEvent(topic, "remove_furniture", new JObject
            {
                ["room_furniture_id"] = furnitureIdInstance
            },
            $"message_{roomId}");
        }

        public void SendSitData(string roomName, string roomId, int furniture_id, int seat_spot)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"SendSitData roomName:{roomName} roomId:{roomId} furniture_id:{furniture_id} seat_spot:{seat_spot}");

            string topic = $"chat_room:{roomName}:{roomId}"; //This is old room socket system
            if (flagUseNewSystem)
            {
                topic = $"room:{roomId}"; //This point to new room socket system
            }

            SendEvent(topic, "player_sit", new JObject
            {
                ["furniture_id"] = furniture_id,
                ["seat_spot"] = seat_spot,
            },
            $"message_{roomId}");
        }

        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------
        //-------------------   M U L T I P L A Y E R  G A M E   ----------------------
        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------

        public void JoinLobby(string userId)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"JoinLobby userId:{userId}");

            string topic = $"lobby:racing:{userId}";
            var eventRef = $"join_${userId}";
            SendEvent(topic, "phx_join", new JObject
            { },
            eventRef);
        }

        public void LeaveLobby(string userId)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"LeaveLobby userId:{userId}");

            string topic = $"racing:{userId}";
            var eventRef = $"join_${userId}";
            SendEvent(topic, "phx_leave", new JObject
            { },
            eventRef);
        }

        public void JoinMatch(string matchId)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"JoinLobby matchId:{matchId}");

            string topic = $"racing:{matchId}";
            var eventRef = $"join_${matchId}";
            SendEvent(topic, "phx_join", new JObject
            { },
            eventRef);
        }

        public void RacingPlayAgain(string matchId)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"Replay matchId:{matchId}");

            string topic = $"racing:{matchId}";
            var eventRef = $"join_${matchId}";
            SendEvent(topic, "play_again", new JObject
            { },
            eventRef);
        }

        public void SendActionRacing(string matchId, RacingMultiplayerManager.RacingActions action, string time = "")
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"SendActionRacing matchId:{matchId} action: {action}");

            string topic = $"racing:{matchId}";
            var eventRef = $"join_${matchId}";

            switch (action)
            {
                case RacingMultiplayerManager.RacingActions.Accelerate:
                    SendEvent(topic, "accelerate", new JObject
                    { }, eventRef);
                    break;
                case RacingMultiplayerManager.RacingActions.RaceEnd:
                    SendEvent(topic, "user_finish", new JObject
                    { ["time"] = time, }, eventRef);
                    break;
                case RacingMultiplayerManager.RacingActions.Stop:
                    SendEvent(topic, "stop", new JObject
                    { }, eventRef);
                    break;
                case RacingMultiplayerManager.RacingActions.TurnLeft:
                    SendEvent(topic, "turn_left", new JObject
                    { }, eventRef);
                    break;
                case RacingMultiplayerManager.RacingActions.TurnRight:
                    SendEvent(topic, "turn_right", new JObject
                    { }, eventRef);
                    break;
                default:
                    return;
            }
        }

        public void SendCustomActionRacing(string matchId, string actionType, string message)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"SendCustomActionRacing matchId:{matchId} action: {actionType} message: {message}");

            string topic = $"racing:{matchId}";
            var eventRef = $"join_${matchId}";

            SendEvent(topic, "custom_msg", new JObject
            {
                ["actionType"] = actionType,
                ["message"] = message,
            }, eventRef);
        }



        public void LeaveMatch(string matchId)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"JoinLobby matchId:{matchId}");

            string topic = $"racing:{matchId}";
            var eventRef = $"join_${matchId}";
            SendEvent(topic, "phx_leave", new JObject
            { },
            eventRef);
        }

        public void SelectCar(string matchId)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"JoinLobby matchId:{matchId}");

            string topic = $"racing:{matchId}";
            var eventRef = $"join_${matchId}";
            SendEvent(topic, "phx_leave", new JObject
            { },
            eventRef);
        }

        public void PlayerWon(string matchId)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"JoinLobby matchId:{matchId}");

            string topic = $"racing:${matchId}";
            var eventRef = $"join_${matchId}";
            SendEvent(topic, "user_finish", new JObject
            {
                ["time"] = 1000,
            },
            eventRef);
        }

        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------
        //-------------------------   P R I V A T E   C H A T   -----------------------
        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------

        public void JoinPrivateChat(int userId)
        {
            if (hasCreatedPrivateChatSocket)
            {
                return;
            }
            hasCreatedPrivateChatSocket = true;
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"JoinPrivateChat userId:{userId}");

            string topic = $"user:{userId}";
            var eventRef = $"join_{userId}";

            SendEvent(topic, "phx_join", new JObject(), eventRef);
        }

        public void LeavePrivateChat(int userId)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"LeavePrivateChat userId:{userId}");

            string topic = $"user:{userId}";
            SendEvent(topic, "phx_leave", new JObject(), $"leave_{userId}");
            hasCreatedPrivateChatSocket = false;
        }

        public void SendPrivateChat(int userId, string username, string roomId, string content)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.SOCKET_OUT)) debugConsole.TraceLog(LOG_TYPE.SOCKET_OUT, $"SendPrivateChat userId:{userId} content:{content}");

            string topic = $"room:{roomId}";

            SendEvent(topic,
                 "private_msg",
                 new JObject
                 {
                     ["content"] = content,
                     ["username"] = username,
                     ["user_id"] = userId
                 },
                 $"message_{userId}");
        }

        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------
        //-------------------------   S U S C R I P T I O N S   -----------------------
        //-----------------------------------------------------------------------------
        //-----------------------------------------------------------------------------

        public bool Suscribe(string eventType, Action<AbstractIncomingSocketEvent> suscriber)
        {
            if (suscriber == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(eventType))
            {
                return false;
            }

            if (!socketEventDeliveries.ContainsKey(eventType))
            {
                socketEventDeliveries[eventType] = new List<Action<AbstractIncomingSocketEvent>>();
            }
            socketEventDeliveries[eventType].Add(suscriber);

            return true;
        }

        public bool Unsuscribe(string eventType, Action<AbstractIncomingSocketEvent> suscriber)
        {
            bool flag = false;
            if (suscriber == null)
            {
                return flag;
            }
            if (eventType == null)
            {
                return flag;
            }
            if (socketEventDeliveries.ContainsKey(eventType))
            {
                for (int j = socketEventDeliveries[eventType].Count - 1; j >= 0; j--)
                {
                    if (socketEventDeliveries[eventType][j] == suscriber)
                    {
                        socketEventDeliveries[eventType].RemoveAt(j);
                        flag = true;
                    }
                }
            }

            return flag;
        }

        public void DeliverSocketEventToSuscribers(AbstractIncomingSocketEvent socketEvent)
        {
            if (socketEvent == null)
            {
                return;
            }
            string eventType = socketEvent.EventType;

            if (socketEventDeliveries.ContainsKey(eventType))
            {
                List<Action<AbstractIncomingSocketEvent>> duplicateList = new List<Action<AbstractIncomingSocketEvent>>(socketEventDeliveries[eventType]);
                int amount = duplicateList.Count;
                for (int i = 0; i < amount; i++)
                {
                    if (duplicateList[i] != null)
                    {
                        duplicateList[i](socketEvent);
                    }
                }
            }
        }














        // overload of the method above so it can be called from other classes without them needing to add "using Newtonsoft.Json.Linq;" to all files
        //public async Task SendEvent(string topic, string eventType, string payloadString,
        //    string eventRef = "none")
        //{
        //    await SendEvent(topic, eventType, JObject.Parse(payloadString), eventRef);
        //}

        //public async void SendMessage(string roomName, string roomId, string content)
        //{
        //    await SendEvent($"chat_room:{roomName}:{roomId}", "message", new JObject { ["content"] = content },
        //        $"message_{roomId}");
        //}
        //public async void SendChatMessage(string roomName, string roomId, string content, string username)
        //{
        //    await SendEvent($"chat_room:{roomName}:{roomId}",
        //        "message",
        //        new JObject
        //        {
        //            ["content"] = content,
        //            ["username"] = username
        //        },
        //        $"message_{roomId}");
        //}

        //public async void LeaveChatRoom(string roomName, string roomId)
        //{
        //    await SendEvent($"chat_room:{roomName}:{roomId}", "phx_leave", "{}", $"leave_{roomId}");
        //}
        //public async void LeaveCurrentChatRoom()
        //{
        //    var (roomName, roomId) = (state.chatRoomName, state.chatRoomId);
        //    await SendEvent($"chat_room:{roomName}:{roomId}", "phx_leave", "{}", $"leave_{roomId}");
        //}
    }
}
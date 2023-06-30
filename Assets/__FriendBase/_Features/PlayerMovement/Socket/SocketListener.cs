using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Architecture.Injector.Core;
using Newtonsoft.Json.Linq;
using PlayerMovement.Socket;
using Socket;
using UniRx;
using UnityEngine;

namespace PlayerMovement
{
    //example json for affected_member
    /*
{
  "username": "user123",
  "user_id": 5319,
  "user_firebase_uid": "1234567897",
  "user_avatar": {
    "skin_color": null,
    "shoes": null,
    "shirt": null,
    "pants": null,
    "nose": null,
    "lips": null,
    "lip_color": null,
    "id": 5319,
    "head": null,
    "hair_color": null,
    "hair": null,
    "glasses": null,
    "eyes": null,
    "eyebrows": null,
    "eyebrow_color": null,
    "eye_color": null,
    "ear": null,
    "dress": null,
    "body": null,
    "accesory": null
  },
  "state": "walking",
  "position_y": 0.0,
  "position_x": 0.0,
  "destination_y": 0.0,
  "destination_x": 0.0
}
     */


    public class SocketListener
    {
        readonly Lazy<ISocketManager> socket = new Lazy<ISocketManager>(Injection.Get<ISocketManager>);

        IObservable<JToken> ListenEven(params string[] eventName)
        {
            return socket.Value.OnMessage().Select(BinaryToJson)
                .Where(json => eventName.Contains(json["event"].ToString()));
        }

        public IObservable<UserInfoDTO> OnInfoChanges()
        {
            return ListenEven("destinations_update", "positions_update", "player_state_update")
                .Select(JsonToUserInfoDTO);
        }

        /// <summary>
        /// emits firebase_UID
        /// </summary>
        /// <returns></returns>
        public IObservable<string> OnUserLeavesRoom()
        {
            return ListenEven("member_left")
                .Select(json => json["payload"])
                .Do(DebugUserLeaveRoom)
                .Select(json => json["affected_member"]["user_firebase_uid"])
                .Where(token => token != null).Select(token => token.ToString());
        }


        public IObservable<UserInfoDTO> OnUserJoinsRoom()
        {
            return ListenEven("member_join")
                .Select(JsonToUserInfoDTO);
        }

        static UserInfoDTO JsonToUserInfoDTO(JToken parent)
        {
            var json = parent["payload"];
            var parts = parent["topic"].ToString().Split(':');

            var roomName = parts[1];
            var roomId = parts[2];

            var s = json["affected_member"].ToString();
            var dto = JsonUtility.FromJson<UserInfoDTO>(s);
            dto.customizationInfo = json["affected_member"]["user_avatar"]?.ToString();
            dto.roomId = roomId;
            dto.roomName = roomName;
            return dto;
        }

        static JObject BinaryToJson(byte[] bytes)
        {
            return JObject.Parse(System.Text.Encoding.UTF8.GetString(bytes));
        }


        #region DEBUG

        static void DebugUserLeaveRoom(JToken json)
        {
            var user_id = json["affected_member"]["user_id"].ToString();
            var username = json["affected_member"]["username"].ToString();
            var user_firebase_uid = json["affected_member"]["user_firebase_uid"].ToString();
            Debug.Log($"player firebase:{user_firebase_uid}  name:{username} user_id:{user_id} left room");
        }

        #endregion
    }
}
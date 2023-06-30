using System;
using System.Collections;
using System.Collections.Generic;
using Data.Users;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventUserEnter : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.USER_ENTER;

        public AvatarRoomData AvatarData { get; private set; }

        public IncomingEventUserEnter(JObject message) : base(message)
        {
            try
            {
                JObject avatarJson = Payload[SocketTags.AFFECTED_MEMBER].Value<JObject>();

                float positionx = avatarJson[SocketTags.POSITION_X].Value<float>();
                float positiony = avatarJson[SocketTags.POSITION_Y].Value<float>();
                string avatarState = avatarJson[SocketTags.AVATAR_STATE].Value<string>();
                int orientation = 1;

                bool isGuest = avatarJson["guest"].Value<bool>();
                JObject userAvatar = avatarJson[SocketTags.AVATAR_DATA].Value<JObject>();

                AvatarCustomizationData avatarCustomizationData = new AvatarCustomizationData();
                avatarCustomizationData.SetDataFromJoinRoom(userAvatar);

                string firebaseId = avatarJson[SocketTags.FIREBASE_ID].Value<string>();
                string username = avatarJson[SocketTags.USER_NAME].Value<string>();

                JObject petData = avatarJson[SocketTags.PET_DATA].Value<JObject>();
                string UserId = avatarJson[SocketTags.USER_ID].Value<string>();
                int? petId = petData[SocketTags.PET_ID].Value<int?>();
                int? petIdInGame = petData[SocketTags.PET_ID_IN_GAME].Value<int?>();
                string PetPrefabName = petData[SocketTags.PET_PREFAB].Value<string>();
                AvatarData = new AvatarRoomData(firebaseId, UserId, username, avatarState, positionx, positiony, orientation, avatarCustomizationData, petId, petIdInGame, PetPrefabName, isGuest);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}


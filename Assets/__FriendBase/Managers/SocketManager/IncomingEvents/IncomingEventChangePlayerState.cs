using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventChangePlayerState : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.AVATAR_STATE;
        public float PositionX { get; private set; }
        public float PositionY { get; private set; }
        public string AvatarState { get; private set; }
        public string FirebaseId { get; private set; }
        public int Orientation { get; internal set; }

        public IncomingEventChangePlayerState(JObject message) : base(message)
        {
            JObject avatarJson = Payload[SocketTags.AFFECTED_MEMBER].Value<JObject>();
            try
            {
                PositionX = avatarJson[SocketTags.POSITION_X].Value<float>();
                PositionY = avatarJson[SocketTags.POSITION_Y].Value<float>();
                FirebaseId = avatarJson[SocketTags.FIREBASE_ID].Value<string>();
                AvatarState = avatarJson[SocketTags.AVATAR_STATE].Value<string>();
                //Orientation = avatarJson[SocketTags.ORIENTATION].Value<int>();
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}


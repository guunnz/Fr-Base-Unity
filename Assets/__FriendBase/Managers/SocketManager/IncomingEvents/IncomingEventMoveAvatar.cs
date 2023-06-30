using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventMoveAvatar : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.AVATAR_MOVE;

        public float Destinationx { get; private set; }
        public float Destinationy { get; private set; }
        public string FirebaseId { get; private set; }

        public IncomingEventMoveAvatar(JObject message) : base(message)
        {
            try
            {

                JObject avatarJson = Payload[SocketTags.AFFECTED_MEMBER].Value<JObject>();

                Destinationx = avatarJson[SocketTags.POSITION_X].Value<float>();
                Destinationy = avatarJson[SocketTags.POSITION_Y].Value<float>();
                FirebaseId = avatarJson[SocketTags.FIREBASE_ID].Value<string>();
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}


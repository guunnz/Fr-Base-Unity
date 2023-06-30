using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventUserLeave : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.USER_LEAVE;

        public string FirebaseId { get; private set; }
        public string UserId { get; private set; }

        public IncomingEventUserLeave(JObject message) : base(message)
        {
            try
            {
                JObject avatarJson = Payload[SocketTags.AFFECTED_MEMBER].Value<JObject>();

                FirebaseId = avatarJson[SocketTags.FIREBASE_ID].Value<string>();
                UserId = avatarJson[SocketTags.USER_ID].Value<string>();
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}
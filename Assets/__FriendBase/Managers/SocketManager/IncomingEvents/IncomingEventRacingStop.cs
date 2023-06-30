using System;
using System.Collections;
using System.Collections.Generic;
using Data.Users;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventRacingStop : AbstractIncomingSocketEvent
    {

        public string firebaseUid;
        public string userId;
        public override string EventType => SocketEventTypes.RACING_STOP;

        public IncomingEventRacingStop(JObject message) : base(message)
        {
            try
            {
                try
                {
                    firebaseUid = Payload[SocketTags.AFFECTED_MEMBER]["user_firebase_uid"].Value<string>();
                    userId = Payload[SocketTags.AFFECTED_MEMBER]["user_id"].Value<string>();
                }
                catch (Exception e)
                {

                }
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}
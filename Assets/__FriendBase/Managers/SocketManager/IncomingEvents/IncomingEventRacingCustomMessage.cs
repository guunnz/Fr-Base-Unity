using System;
using System.Collections;
using System.Collections.Generic;
using Data.Users;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventRacingCustomMessage : AbstractIncomingSocketEvent
    {

        public string actionType;
        public string eventMessage;
        public string firebaseUid;
        public override string EventType => SocketEventTypes.RACING_CUSTOM_MESSAGE;

        public IncomingEventRacingCustomMessage(JObject message) : base(message)
        {
            try
            {
                try
                {
                    eventMessage = Payload["message"].Value<string>();
                    actionType = Payload["actionType"].Value<string>();
                    firebaseUid = Payload["firebase_uid"].Value<string>();
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
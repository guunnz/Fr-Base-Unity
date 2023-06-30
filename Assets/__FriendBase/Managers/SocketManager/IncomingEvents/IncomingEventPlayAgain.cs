using System;
using System.Collections;
using System.Collections.Generic;
using Data.Users;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventPlayAgain : AbstractIncomingSocketEvent
    {
        public string firebaseUid;
        public string userId;
        public string matchId;
        public override string EventType => SocketEventTypes.PLAY_AGAIN;

        public IncomingEventPlayAgain(JObject message) : base(message)
        {
            try
            {
                userId = Payload["user_id"].Value<string>();
                matchId = Payload["match_id"].Value<string>();
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}
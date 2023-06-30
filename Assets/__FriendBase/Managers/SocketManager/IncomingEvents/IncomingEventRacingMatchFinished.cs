using System;
using System.Collections;
using System.Collections.Generic;
using Data.Users;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventRacingMatchFinished : AbstractIncomingSocketEvent
    {

        public string userId1;
        public string userId2;
        public string user1Time;
        public string user2Time;
        public string winnerId;
        public override string EventType => SocketEventTypes.RACING_MATCH_FINISH;

        public IncomingEventRacingMatchFinished(JObject message) : base(message)
        {
            try
            {
                try
                {
                    winnerId = Payload["winner_id"].Value<string>();
                    userId1 = Payload["user_1_id"].Value<string>();
                    userId2 = Payload["user_2_id"].Value<string>();
                    user1Time = Payload["user_1_time"].Value<string>();
                    user2Time = Payload["user_2_time"].Value<string>();
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
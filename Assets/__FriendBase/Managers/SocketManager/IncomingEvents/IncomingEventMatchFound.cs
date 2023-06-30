using System;
using System.Collections;
using System.Collections.Generic;
using Data.Users;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventMatchFound : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.MATCH_EVENT;

        public string matchId = "";
        public string player1Id = "";
        public string player1Username = "";
        public string player2Id = "";
        public string player2Username = "";
        public JObject avatar1Json;
        public JObject avatar2Json;
        public UserAccountStatus UserStatus { get; internal set; }

        public IncomingEventMatchFound(JObject message) : base(message)
        {
            try
            {
                try
                {
                    matchId = Payload["match_id"].Value<string>();
                    player1Id = Payload["users"][0]["id"].Value<string>();
                    player2Id = Payload["users"][1]["id"].Value<string>();
                    avatar1Json = Payload["users"][0]["avatar"].Value<JObject>();
                    avatar2Json = Payload["users"][1]["avatar"].Value<JObject>();
                    player1Username = Payload["users"][0]["username"].Value<string>();
                    player2Username = Payload["users"][1]["username"].Value<string>();
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
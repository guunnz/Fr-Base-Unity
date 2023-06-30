using System;
using System.Collections;
using System.Collections.Generic;
using Data.Users;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventMinigameInvite : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.MINIGAME_INVITE;
        public string gameInvitationId = "";
        public string user_id = "";
        public string guest_user_id = "";
        public string created_at = "";
        public string status = "";
        public UserAccountStatus UserStatus { get; internal set; }

        public IncomingEventMinigameInvite(JObject message) : base(message)
        {
            try
            {
                try
                {
                    gameInvitationId = Payload["data"]["id"].Value<string>();
                    user_id = Payload["data"]["user_id"].Value<string>();
                    guest_user_id = Payload["data"]["guest_user_id"].Value<string>();
                    created_at = Payload["data"]["created_at"].Value<string>();
                    status = Payload["data"]["status"].Value<string>();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.Log(e.Message);
                }
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}
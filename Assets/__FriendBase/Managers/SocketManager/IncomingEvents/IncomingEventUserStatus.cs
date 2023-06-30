using System;
using System.Collections;
using System.Collections.Generic;
using Data.Users;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventUserStatus : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.USER_STATUS;

        public UserAccountStatus UserStatus { get; internal set; }

        public IncomingEventUserStatus(JObject message) : base(message)
        {
            try
            {
                UserStatus = new UserAccountStatus();
                string status = Payload[SocketTags.STATUS].Value<string>();

                string timeSuspensionStart = "";
                string timeSuspensionEnd = "";
                string timeSuspensionLeft = "";
                try
                {
                    timeSuspensionStart = Payload["suspension_start"].Value<string>();
                    timeSuspensionEnd = Payload["suspension_end"].Value<string>();
                    timeSuspensionLeft = Payload["time_left_in_seconds"].Value<string>();
                }
                catch (Exception e)
                {
                }

                int userId = Payload["id"].Value<int>();
                UserStatus.SetStatus(status, userId, timeSuspensionStart, timeSuspensionEnd, timeSuspensionLeft);
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}
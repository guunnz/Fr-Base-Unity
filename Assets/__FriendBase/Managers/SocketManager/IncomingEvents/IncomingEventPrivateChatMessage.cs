using System;
using ChatView.Core.Domain;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventPrivateChatMessage : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.PRIVATE_CHAT_MESSAGE;

        public ChatData ChatMessageData { get; private set; }
        public string SenderUserId { get; private set; }
        public string ReceiverUserId { get; private set; }

        public IncomingEventPrivateChatMessage(JObject message) : base(message)
        {
            try
            {
                ChatMessageData = new ChatData(Payload["firebase_uid"].Value<string>(), Payload["username"].Value<string>(), Color.white, Payload["content"].Value<string>());

                SenderUserId = Payload["sender_id"].Value<string>();
                ReceiverUserId = Payload["receiver_id"].Value<string>();
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}


using System;
using ChatView.Core.Domain;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventChatMessage : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.CHAT_MESSAGE;
        
        public ChatData ChatMessageData { get; private set; }
        
        
        public IncomingEventChatMessage(JObject message) : base(message)
        {
            try
            {
                ChatMessageData = ToChatMessageModel(Payload);
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
        
        ChatData ToChatMessageModel(JObject jMessage)
        {
            var usernameColorValue = jMessage["usernameColor"].Value<string>();

            if (!usernameColorValue.StartsWith("#"))
            {
                usernameColorValue = usernameColorValue.Insert(0,"#");
            }

            ColorUtility.TryParseHtmlString(usernameColorValue, out var usernameColor);

            return new ChatData(jMessage["firebase_uid"].Value<string>(),jMessage["username"].Value<string>(),
                usernameColor,jMessage["content"].Value<string>());
        }
    }
}
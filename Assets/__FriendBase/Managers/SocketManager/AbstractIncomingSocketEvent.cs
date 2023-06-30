using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

namespace Socket
{
    public abstract class AbstractIncomingSocketEvent 
    {
        public abstract string EventType { get; }
        public JObject message { get; }

        public int State { get; protected set; }

        public string EventMsgType { get; protected set; }
        public string Reference { get; protected set; }
        public string Topic { get; protected set; }
        public JObject Payload { get; protected set; }

        public AbstractIncomingSocketEvent(JObject message)
        {
            this.message = message;
            try
            {
                EventMsgType = message[SocketTags.EVENT].Value<string>();
                Payload = message[SocketTags.PAYLOAD].Value<JObject>();
                Reference = message[SocketTags.REF].Value<string>();
                Topic = message[SocketTags.TOPIC].Value<string>();
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}
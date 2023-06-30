using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventLeaveRoom : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.LEAVE_ROOM;

        public IncomingEventLeaveRoom(JObject message) : base(message)
        {
        }
    }
}


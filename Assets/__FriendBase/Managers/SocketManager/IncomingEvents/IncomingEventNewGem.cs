using System;
using Newtonsoft.Json.Linq;

namespace Socket
{
    public class IncomingEventNewGem : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.NEW_GEM;

        public string Token { get; private set; }

        public IncomingEventNewGem(JObject message) : base(message)
        {
            try
            {
                JObject GemJson = Payload[SocketTags.BODY].Value<JObject>();

                var token = GemJson[SocketTags.TOKEN].Value<string>();

                Token = token;
               
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}
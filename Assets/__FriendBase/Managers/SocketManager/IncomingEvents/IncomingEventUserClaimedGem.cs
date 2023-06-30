using System;
using Newtonsoft.Json.Linq;

namespace Socket
{
    public class IncomingEventUserClaimedGem : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.GEM_PICKED;

        public string RewardedUserFirebaseId { get; private set; }

        public IncomingEventUserClaimedGem(JObject message) : base(message)
        {
            try
            {
                var rewardedUserFirebaseId = Payload[SocketTags.GEMS_USER_FIREBASE_UID].Value<string>();

                RewardedUserFirebaseId = rewardedUserFirebaseId;
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}
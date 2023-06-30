using System;
using Newtonsoft.Json.Linq;

namespace Socket
{
    public class IncomingEventNewPetInfo : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.PET_INFO;
        public string UserId { get; private set; }
        public int? PetId { get; private set; }
        public int? PetIdInGame { get; private set; }
        public string PetPrefabName { get; private set; }

        public IncomingEventNewPetInfo(JObject message) : base(message)
        {
            try
            {
                JObject avatarJson = Payload;
                UserId = avatarJson[SocketTags.USER_ID].Value<string>();
                PetId = avatarJson[SocketTags.PET_ID].Value<int?>();
                PetIdInGame = avatarJson[SocketTags.PET_ID_IN_GAME].Value<int?>();
                PetPrefabName = avatarJson[SocketTags.PET_PREFAB].Value<string>();
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}
using Functional.Maybe;
using JetBrains.Annotations;
using PlayerRoom.Core.Services;

namespace PlayerRoom.Core.Actions
{
    [UsedImplicitly]
    public class GetLastRoom
    {
        readonly IPlayerRoomRepository repository;

        public GetLastRoom(IPlayerRoomRepository repository)
        {
            this.repository = repository;
        }

        public Maybe<string> Execute()
        {
            return repository.LastRoomUsed;
        }
    }
}
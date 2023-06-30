using AuthFlow.EndAuth.Repo;
using Functional.Maybe;
using JetBrains.Annotations;
using PlayerRoom.Core.Services;


namespace PlayerRoom.Infrastructure
{
    [UsedImplicitly]
    public class LocalPlayerRoomRepository : IPlayerRoomRepository
    {
        //leave it on ram
        public Maybe<string> LastRoomUsed { get; private set; } = Maybe<string>.Nothing;

        public void StoreLastRoomUsed(string roomId)
        {
            LastRoomUsed = roomId.ToMaybeString();
        }
    }
}
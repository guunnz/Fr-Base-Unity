using Functional.Maybe;

namespace PlayerRoom.Core.Services
{
    public interface IPlayerRoomRepository
    {
        Maybe<string> LastRoomUsed { get; }
        void StoreLastRoomUsed(string roomId);
    }
}
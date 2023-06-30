using PlayerRoom.Core.Services;

namespace PlayerRoom.Infrastructure
{
    public class MemoryPlayerRoomStateManager : IPlayerRoomStateManager
    {
        public string CurrentRoomId { get; set; }
    }
}
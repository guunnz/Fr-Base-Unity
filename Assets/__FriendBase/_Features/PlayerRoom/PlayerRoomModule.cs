using Architecture.Context;
using Architecture.Injector.Core;
using PlayerRoom.Core.Actions;
using PlayerRoom.Core.Services;
using PlayerRoom.Infrastructure;

namespace PlayerRoom
{
    public class PlayerRoomModule : IModule
    {
        public void Init()
        {
            Injection.Register<IPlayerRoomRepository, LocalPlayerRoomRepository>();
            Injection.Register<IPlayerRoomStateManager, MemoryPlayerRoomStateManager>();
            Injection.Register<IRoomsService, HttpRoomsService>();


            Injection.Register<EnterRoom>();
            Injection.Register<GetLastRoom>();
        }
    }
}
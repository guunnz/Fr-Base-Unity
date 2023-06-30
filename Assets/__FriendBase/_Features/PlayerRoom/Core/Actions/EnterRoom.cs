using System;
using JetBrains.Annotations;
using PlayerRoom.Core.Services;
using UniRx;

namespace PlayerRoom.Core.Actions
{
    [UsedImplicitly]
    public class EnterRoom
    {
        readonly IPlayerRoomRepository repository;

        public EnterRoom(IPlayerRoomRepository repository)
        {
            this.repository = repository;
        }

        public IObservable<Unit> Execute(string roomId)
        {
            return Observable.Return(roomId)
                .Do(repository.StoreLastRoomUsed)
                .AsUnitObservable();
        }
    }
}
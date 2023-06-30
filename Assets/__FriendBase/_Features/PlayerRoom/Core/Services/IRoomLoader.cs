using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlayerRoom.Core.Domain;
using UniRx;

namespace PlayerRoom.Core.Services
{
    public interface IRoomLoader
    {
        IObservable<Unit> LoadRoom(string id);
        List<RoomInfo> Rooms { get; }

        Task ConnectNewRoomAsync(string room, string instanceId);
    }
}
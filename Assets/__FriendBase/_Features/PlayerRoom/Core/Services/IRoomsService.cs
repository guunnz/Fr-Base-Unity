using System;
using System.Collections.Generic;
using PlayerRoom.Core.Domain;

namespace PlayerRoom.Core.Services
{
    public interface IRoomsService
    {
        IObservable<List<RoomInfo>> GetRoomsIDs();
        IObservable<List<RoomInfo>> GetRoomInstances(string roomId);
    }
}
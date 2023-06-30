using System.Collections.Generic;
using System;
using Data.Rooms;
using System.Threading.Tasks;

public interface IRoomListEndpoints 
{
    IObservable<List<RoomInformation>> GetPublicRoomsList();
    IObservable<List<RoomInformation>> GetPublicRoomsListInside(int idRoom);
    IObservable<RoomInformation> GetMyIdHouse();
    IObservable<RoomInformation> ChangeTheme(int idTheme);
    IObservable<RoomInformation> CreateEvent(int eventType);
    IObservable<List<RoomInformation>> GetEventList(bool addMyEventRow);
    IObservable<RoomInformation> FinishEvent();
    IObservable<RoomInformation> GetFreePublicRoomByType(int idRoom);
}


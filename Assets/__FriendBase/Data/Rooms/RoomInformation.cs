namespace Data.Rooms
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class RoomInformation
    {
        public enum EVENT_STATE {NONE, EVENT, MY_EVENT_CARD_TO_HOST, MY_EVENT_CARD_TO_END };

        public string RoomIdInstance { get; set; }
        public string RoomName { get; set; }
        public int AmountUsers { get; set; }
        public int RoomId { get; set; }
        public string NamePrefab { get; set; }
        public bool IsEnable { get; set; }
        public int PlayerLimit { get; set; }

        public int RoomRank { get; set; }
        public string RoomType { get; set; }
        public int IdUser { get; set; }
        //For Events
        public string HostUserName{ get; set; }
        public EVENT_STATE EventState { get; set; }
        public int EventType { get; set; }

        public RoomInformation()
        {
            EventState = EVENT_STATE.NONE;
        }

        public RoomInformation(string roomIdInstance, string roomName, int amountUsers, int roomId, string namePrefab, bool isEnable, int playerLimit, int roomRank, string roomType, int idUser, string hostUserName, EVENT_STATE eventState)
        {
            RoomIdInstance = roomIdInstance;
            RoomName = roomName;
            AmountUsers = amountUsers;
            RoomId = roomId;
            NamePrefab = namePrefab;
            IsEnable = isEnable;
            PlayerLimit = playerLimit;
            RoomRank = roomRank;
            RoomType = roomType;
            IdUser = idUser;
            HostUserName = hostUserName;
            EventState = eventState;
        }

        public RoomInformation Duplicate()
        {
            return new RoomInformation(RoomIdInstance, RoomName, AmountUsers, RoomId, NamePrefab, IsEnable, PlayerLimit, RoomRank, RoomType, IdUser, HostUserName, EventState);
        }
    }
}
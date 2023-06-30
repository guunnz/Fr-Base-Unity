using System;

namespace PlayerRoom.Core.Domain
{
    public struct RoomInfo
    {
        public string AreaId { get; set; }
        public string RoomName { get; set; }

        public int PlayersOnArea { get; set; }
        public int PlayersOnRoom { get; set; }
        public string InstanceId { get; set; }


        public RoomInfo Clone()
        {
            return this; // cause it's an struct
        }
    }
}
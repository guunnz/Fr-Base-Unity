using System;
using System.Diagnostics.CodeAnalysis;

namespace PlayerMovement.Socket
{
    [Serializable, SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct UserInfoDTO
    {
        public string username;
        public string user_firebase_uid;
        public float position_x;
        public float position_y;
        public float destination_x;
        public float destination_y;
        [NonSerialized] public string customizationInfo;
        [NonSerialized] public string roomId;
        [NonSerialized] public string roomName;
    }
    
    
}
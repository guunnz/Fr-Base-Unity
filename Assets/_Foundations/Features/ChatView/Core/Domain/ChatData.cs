using UnityEngine;

namespace ChatView.Core.Domain
{
    public class ChatData
    {
        public readonly string username;
        public readonly Color usernameColor;
        public readonly string message;
        public readonly string firebaseUid;

        public ChatData(string firebaseUid, string username, Color usernameColor, string message)
        {
            this.firebaseUid = firebaseUid;
            this.username = username;
            this.usernameColor = usernameColor;
            this.message = message;
        }
    }
}
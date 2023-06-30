using System;
using System.Collections.Generic;
using UnityEngine;

namespace Socket
{
    public class SocketStateManager
    {
        public string chatRoomName;
        public string chatRoomId;
        public Vector2? position;

        public void OnJoinChatRoom(string name, string id, Vector2 pos)
        {
            chatRoomName = name;
            chatRoomId = id;
            position = pos;
        }

        public bool OnRoom()
        {
            return !string.IsNullOrEmpty(chatRoomId) && !string.IsNullOrEmpty(chatRoomName) && position.HasValue;
        }

        public void Clear()
        {
            chatRoomId = null;
            chatRoomName = null;
            position = null;
        }
    }
}
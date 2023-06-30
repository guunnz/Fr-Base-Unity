using System;
using System.Collections.Generic;
using Scriptables;
using UnityEngine;

namespace PlayerRoom.Delivery
{
    [Serializable]
    public class RoomData : IFindableItem
    {
        public string roomId;
        public string roomName;
        public string musicClipKey;
        public List<ColourMusic> musicPerArea;
        public Sprite background;
        public Sprite mask;
        public Sprite thumbnail;
        public bool publicArea;
        public string Id => roomId;
    }

    [Serializable]
    public struct ColourMusic
    {
        public string trackId;
        public Color color;
    }
}
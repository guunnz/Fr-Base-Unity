using System;
using PlayerRoom.Delivery;
using UnityEngine;

namespace PlayerRoom.View.Testers
{
    public class EditorRoomTester : MonoBehaviour
    {
        public SpriteRenderer mask, bg;

        public GameObject parent;

        RoomsData data;
        RoomsData Data => data ??= Resources.Load<RoomsData>("RoomsData");

        public int roomToTest;
        public string id;

        void OnValidate()
        {
            if (Application.isPlaying) return;
            if (!mask || !bg) return;
            if (Data.List.Count <= roomToTest || roomToTest < 0) return;
            var room = Data.List[roomToTest];
            id = room.Id;
            bg.sprite = room.background;
            mask.sprite = room.mask;
            mask.color = new Color(1, 1, 1, .1f);
            bg.color = new Color(1, 1, 1, 1);
            foreach (var part in parent.GetComponentsInChildren<IRoomPart>(true))
            {
                part.SetActive(string.Equals(part.RoomId, id, StringComparison.CurrentCultureIgnoreCase));
            }
        }
    }
}
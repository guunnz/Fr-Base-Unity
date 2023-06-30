using System;
using System.Collections.Generic;
using Functional.Maybe;
using Scriptables;
using UnityEngine;

namespace PlayerRoom.Delivery
{
    [CreateAssetMenu(menuName = "Create RoomsData", fileName = "RoomsData", order = 0)]
    public class RoomsData : ScriptableList<RoomData>
    {
        public string defaultRoom;
    }
}
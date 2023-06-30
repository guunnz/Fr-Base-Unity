using Functional.Maybe;
using PlayerRoom.Delivery;
using UnityEngine;

namespace PlayerRoom.View
{
    public abstract class RoomViewComponent : MonoBehaviour
    {
        RoomsData roomsDataCache;
        public RoomsData RoomsData => roomsDataCache ??= Resources.Load<RoomsData>("RoomsData");

        /// <summary>
        /// Write your dependencies here
        /// </summary>
        public virtual void Write()
        {
            
        }

        /// <summary>
        /// Read dependencies here
        /// </summary>
        public virtual void Read()
        {
            
        }


        public void LoadRoom(string roomId)
        {
            RoomId = roomId;
            RoomData = RoomsData.GetItem(roomId).DoOnAbsent(() => Debug.LogError($"There is no room for {roomId}"));
            DidLoadRoom();
        }

        protected string RoomId { get; private set; }

        protected Maybe<RoomData> RoomData { get; private set; }


        protected abstract void DidLoadRoom();
    }
}
using UnityEngine;

namespace PlayerRoom.View
{
    public class RoomItems : MonoBehaviour, IRoomPart
    {
        public string roomId;
        public string RoomId => roomId;

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
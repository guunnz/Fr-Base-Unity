using UnityEngine;

namespace PlayerRoom.View
{
    public interface IRoomPart
    {
        public string RoomId { get; }
        public void SetActive(bool active);
    }

    public class RoomFixedData : MonoBehaviour, IRoomPart
    {
        public Transform spawnPoint;
        public string roomId;
        public string RoomId => roomId;

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
using UnityEngine;

namespace PlayerRoom.Core.Services
{
    public interface IRoomMask
    {
        float ColorDistance(Color color, Vector2 worldPoint);
    }
}
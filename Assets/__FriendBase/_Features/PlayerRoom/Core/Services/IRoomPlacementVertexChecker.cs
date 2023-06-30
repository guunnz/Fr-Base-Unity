using UnityEngine;

namespace PlayerRoom.Core.Services
{
    public interface IRoomPlacementVertexChecker
    {
        bool CanPlace(Vector2 vertex);
    }
}
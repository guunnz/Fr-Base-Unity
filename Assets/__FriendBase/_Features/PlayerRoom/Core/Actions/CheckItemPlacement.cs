using PlayerRoom.Core.Services;
using UnityEngine;

namespace PlayerRoom.Core.Actions
{
    // checks if an item vbertex can be placed on a place
    public class CheckItemPlacement
    {
        readonly IRoomPlacementVertexChecker vertexChecker;

        public CheckItemPlacement(IRoomPlacementVertexChecker vertexChecker)
        {
            this.vertexChecker = vertexChecker;
        }

        public bool Execute(Vector2 worldVertex)
        {
            return vertexChecker.CanPlace(worldVertex);
        }
    }
}
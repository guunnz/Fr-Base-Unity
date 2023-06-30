using UnityEngine;

namespace CanvasInput.Core.Services
{
    public struct InputInfo
    {
        public long dragId;
        public int fingerId;
        public Vector2 initialTouch;
        public Vector2 currentTouch;
        public float startTime;
        public InputStateType type;
    }
}
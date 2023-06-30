using System;
using Architecture.Injector.Core;
using PlayerRoom.View;
using PlayerRoom.View.RoomComponents;
using UnityEngine;

namespace PlayerMovement
{
    public class CameraFollower : RoomViewComponent
    {
        Rect? bounds;
        public Camera cam;
        public Transform player;
        float initialZ;

        void Start()
        {
            initialZ = cam.transform.position.z;
        }

        void Update()
        {
            if (!bounds.HasValue) return;

            var playerPosition = player.position;
            var rect = bounds.Value;
            
            cam.transform.position = new Vector3
            {
                x = Mathf.Clamp(((Vector2) playerPosition).x, rect.xMin, rect.xMax),
                y = Mathf.Clamp(((Vector2) playerPosition).y, rect.yMin, rect.yMax),
                z = initialZ
            };
        }


        protected override void DidLoadRoom()
        {
            //calculate camera rect size
            Vector2 worldCamMin = cam.ViewportToWorldPoint(Vector2.zero);
            Vector2 worldCamMax = cam.ViewportToWorldPoint(Vector2.one);
            Vector2 worldCamSize = worldCamMax - worldCamMin;

            var bgBounds = Injection.Get<IRoomBackground>().Bounds;
            Vector2 boundsSize = ((Vector2) bgBounds.size) - worldCamSize;
            Vector2 boundsCenter = bgBounds.center;
            Vector2 boundsExtents = boundsSize * 0.5f;
            bounds = new Rect(boundsCenter - boundsExtents, boundsSize);
        }
    }
}
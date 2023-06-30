using System;
using Architecture.Injector.Core;
using Functional.Maybe;
using PlayerRoom.Core.Services;
using UnityEngine;

namespace PlayerRoom.View.RoomComponents
{
    public class RoomMaskView : RoomViewComponent, IRoomPlacementVertexChecker, IRoomMask
    {
        [SerializeField] SpriteRenderer maskRenderer;
        Sprite currentMask;

        public override void Write()
        {
            Injection.Register<IRoomPlacementVertexChecker>(this);
            Injection.Register<IRoomMask>(this);
        }

        void OnDestroy()
        {
            Injection.Clear<IRoomPlacementVertexChecker>();
            Injection.Clear<IRoomMask>();
        }

        public bool CanPlace(Vector2 point)
        {
            if (!currentMask) return false;
            var pixel = WorldPositionToTextureSpace(point);
            var color = currentMask.texture.GetPixel((int) pixel.x, (int) pixel.y);
            return color.r > 0.5f;
        }


        Vector2 WorldPositionToTextureSpace(Vector3 worldPos)
        {
            var ppu = currentMask.pixelsPerUnit;
            //  invTRS(world) * ppu == retrieves local position in pixels :) 
            var localPos = (Vector2) maskRenderer.transform.InverseTransformPoint(worldPos) * ppu;
            var texSpacePivot = new Vector2(currentMask.rect.x, currentMask.rect.y) + currentMask.pivot;
            return texSpacePivot + localPos;
        }

        Vector2 TextureSpaceToUv(Vector2 texPos)
        {
            var texSize = new Vector2(currentMask.texture.width, currentMask.texture.height);
            return texPos / texSize;
        }

        public Vector2 WorldPositionToUv(Vector3 worldPos)
        {
            var texPos = WorldPositionToTextureSpace(worldPos);
            return TextureSpaceToUv(texPos);
        }

        protected override void DidLoadRoom()
        {
            RoomData.Do(roomData =>
            {
                maskRenderer.sprite = roomData.mask;
                maskRenderer.enabled = false;

#if UNITY_EDITOR
                //for debugging purposes
                maskRenderer.color = new Color(1f, 1f, 1f, 0.33f);
                maskRenderer.enabled = true;
#endif
                currentMask = roomData.mask;
            });
        }
        
        
        

        public float ColorDistance(Color color, Vector2 worldPoint)
        {
            if (!currentMask) return float.MaxValue;
            var pixel = WorldPositionToTextureSpace(worldPoint);
            var pixelColor = currentMask.texture.GetPixel((int) pixel.x, (int) pixel.y);
            return Vector3.Distance((Vector4) pixelColor, (Vector4) color);
            //vector3 to avoid take alpha distance
        }
    }
}
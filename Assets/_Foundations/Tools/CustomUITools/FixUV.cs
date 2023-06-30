using System.Collections.Generic;
using UnityEngine;

namespace Tools.CustomUITools
{
    public class FixUV : CustomImageEffect
    {
        readonly List<UIVertex> stream = new List<UIVertex>(4);


        protected override void OnApplyEffect()
        {
            vertexHelper.GetUIVertexStream(stream);
            
            var (size, center) = GetRelativeSizeAndCenter(false);

            Vector2 leftBottomCorner = center - new Vector2(size.x, size.y) * 0.5f;

            for (var i = 0; i < stream.Count; i++)
            {
                var uiVertex = stream[i];

                Vector2 vertPos = uiVertex.position;
                var relPos = vertPos - leftBottomCorner;

                uiVertex.uv0 = relPos / size;

                stream[i] = uiVertex;
            }

            vertexHelper.AddUIVertexTriangleStream(stream);
            Debug.Log(vertexHelper.currentVertCount);
        }
    }
}
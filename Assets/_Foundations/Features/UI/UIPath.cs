using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIPath : MaskableGraphic
    {
        public List<PathPoint> pathPoints;
        public float strokeWidth = 1;

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            OnDirty();
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            OnDirty();
        }
#endif
        public Vector2 GetPosition(PathPoint point)
        {
            var size = rectTransform.sizeDelta;
            var bottomLeft = rectTransform.pivot * -size;
            return bottomLeft + point.position;
        }

        public void SetPosition(ref PathPoint point, Vector2 position)
        {
            var size = rectTransform.sizeDelta;
            var bottomLeft = rectTransform.pivot * -size;
            point.position = position - bottomLeft;
        }

        public void OnDirty()
        {
            SetVerticesDirty();
            SetMaterialDirty();
        }


        private void AddNode(int i, VertexHelper vh)
        {
            var vertex = new UIVertex {color = color};
            var count = pathPoints.Count;

            var proportion = i / (count - 1f);
        }


        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            const int pointsForLoop = 4;
            var defaultVertex = new UIVertex {color = color};
            for (var i = 0; i < pathPoints.Count - 1; i++)
            {
                var current = GetPosition(pathPoints[i]);
                var next = GetPosition(pathPoints[i + 1]);

                //var current = (pathPoints[i].position);
                //var next = (pathPoints[i + 1].position);

                var direction = (next - current).normalized;
                var ortho = new Vector2(direction.y, -direction.x) * strokeWidth;
                var a = defaultVertex;
                var b = defaultVertex;
                var c = defaultVertex;
                var d = defaultVertex;

                a.position = current + ortho;
                b.position = current - ortho;
                c.position = next + ortho;
                d.position = next - ortho;


                a.uv0 = Vector2.one;
                b.uv0 = Vector2.one;
                c.uv0 = Vector2.one;
                d.uv0 = Vector2.one;

                vh.AddVert(a);
                vh.AddVert(b);
                vh.AddVert(c);
                vh.AddVert(d);

                vh.AddTriangle(i * pointsForLoop + 1, i * pointsForLoop + 0, i * pointsForLoop + 2);
                vh.AddTriangle(i * pointsForLoop + 1, i * pointsForLoop + 2, i * pointsForLoop + 3);
            }
        }
    }

    [Serializable]
    public struct PathPoint
    {
        public Vector2 position;
        public float size;
    }
}
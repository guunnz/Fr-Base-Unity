using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIShape : MaskableGraphic
    {
        [Range(0, 1)] public float strokeSize = 0.5f;

        public int sides = 3;
        public bool angularUV;
        public float angle;


        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
            SetMaterialDirty();
        }

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            const int maxSides = 10000;
            sides = Mathf.Clamp(sides, 3, maxSides);

            base.OnValidate();
            SetVerticesDirty();
            SetMaterialDirty();
        }

#endif
        private void AddVertices(VertexHelper vh, int i, Vector2 add, Vector2 mul)
        {
            var sideAmount = Mathf.InverseLerp(0, sides, i);

            Vector2 direction = Quaternion.Euler(0, 0, sideAmount * 360 + angle) * Vector3.up;
            direction *= 0.5f; // 0.5 magnitude

            var externalPoint = direction + new Vector2(0.5f, 0.5f);
            var internalPoint = direction * (1 - strokeSize) + new Vector2(0.5f, 0.5f);

            var externalVertex = new UIVertex {color = color};
            var internalVertex = new UIVertex {color = color};

            if (angularUV)
            {
                externalVertex.uv0 = new Vector2(1, sideAmount);
                internalVertex.uv0 = new Vector2(0, sideAmount);
            }
            else
            {
                externalVertex.uv0 = externalPoint;
                internalVertex.uv0 = internalPoint;
            }

            externalVertex.position = externalPoint * mul + add;
            internalVertex.position = internalPoint * mul + add;

            vh.AddVert(externalVertex);
            vh.AddVert(internalVertex);
        }

        private void AddTriangles(VertexHelper vh, int i)
        {
            var firstExternal = i * 2;
            var firstInternal = firstExternal + 1;
            var secondExternal = firstInternal + 1;
            var secondInternal = secondExternal + 1;
            vh.AddTriangle(firstInternal, secondExternal, firstExternal);
            vh.AddTriangle(firstInternal, secondInternal, secondExternal);
        }


        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var size = rectTransform.sizeDelta;
            var bottomLeft = rectTransform.pivot * -size;

            for (var i = 0; i <= sides; i++) AddVertices(vh, i, bottomLeft, size);
            for (var i = 0; i < sides; i++) AddTriangles(vh, i);
        }
    }
}
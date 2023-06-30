using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public static class UIMeshUtils
    {
        public static void CreateTriangle(this VertexHelper vh, Triangle t)
        {
            var count = vh.currentVertCount;

            vh.AddVert(new UIVertex
            {
                color = t.color,
                position = t.a,
                uv0 = t.uv0A,
                uv1 = t.uv1A
            });
            vh.AddVert(new UIVertex
            {
                color = t.color,
                position = t.b,
                uv0 = t.b,
                uv1 = t.b
            });
            vh.AddVert(new UIVertex
            {
                color = t.color,
                position = t.c,
                uv0 = t.uv0C,
                uv1 = t.uv1C
            });
            vh.AddTriangle(count, count + 1, count + 2);
        }
    }

    public struct Triangle
    {
        public Vector2 a;
        public Vector2 b;
        public Vector2 c;
        public Vector2 uv0A;
        public Vector2 uv0B;
        public Vector2 uv0C;
        public Vector2 uv1A;
        public Vector2 uv1B;
        public Vector2 uv1C;
        public Color color;

        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;

            uv0A = default;
            uv0B = default;
            uv0C = default;

            uv1A = default;
            uv1B = default;
            uv1C = default;
            color = Color.white;
        }
    }
}
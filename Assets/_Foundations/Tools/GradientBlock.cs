using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class GradientBlock : BaseMeshEffect
    {
        public Color topOrRight = Color.cyan;
        public Color bottomOrLeft = Color.blue;
        [SerializeField] bool horizontal;
        [SerializeField] bool invert;

        readonly List<UIVertex> vertices = new List<UIVertex>(4);


        bool Color1OnHorizontal(int index)
        {
            if (index == 0 || index == 5) //bottom left corner
            {
                return true;
            }

            if (index == 1) // top left corener
            {
                return true;
            }

            if (index == 2 || index == 3) // top right corner
            {
                return false;
            }

            return false;
        }

        bool Color1OnVertical(int index)
        {
            if (index == 0 || index == 5) //bottom left corner
            {
                return true;
            }

            if (index == 1) // top left corener
            {
                return false;
            }

            if (index == 2 || index == 3) // top right corner
            {
                return false;
            }

            return true; // bottom right
        }

        bool Color1(int index) => horizontal ? Color1OnHorizontal(index) : Color1OnVertical(index);

        public override void ModifyMesh(VertexHelper vh)
        {
            vh.GetUIVertexStream(vertices);
            var count = vertices.Count;

            for (int i = 0; i < count; i++)
            {
                var uiVertex = vertices[i];
                uiVertex.color = Color1(i) ^ invert ? bottomOrLeft : topOrRight;

                vertices[i] = uiVertex;
            }

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertices);
        }
    }
}
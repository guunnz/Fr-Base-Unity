using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShapedImage : Image
    {
        
        
        
        private const int MaxDivisions = 10000;

        private const int LeftRectBottomLeft = 0;
        private const int LeftRectTopLeft = 1;
        private const int LeftRectTopRight = 2;
        private const int LeftRectBottomRight = 3;

        private const int TopRectBottomLeft = LeftRectTopRight;
        private const int TopRectTopLeft = 4;
        private const int TopRectTopRight = 5;
        private const int TopRectBottomRight = 6;

        private const int RightRectBottomLeft = 7;
        private const int RightRectTopLeft = TopRectBottomRight;
        private const int RightRectTopRight = 8;
        private const int RightRectBottomRight = 9;

        private const int BottomRectBottomLeft = 10;
        private const int BottomRectTopLeft = LeftRectBottomRight;
        private const int BottomRectTopRight = RightRectBottomLeft;
        private const int BottomRectBottomRight = 11;

        private const int ExtraVerticesCount = 12;


        public int cornerDivisions;
        public float desiredCornerRadius;
        public float cornerRadius;
        [Range(0, 1)] public float amount; // todo: ver como pasar de circulo a "rounded corner" 


        protected override void OnRectTransformDimensionsChange()
        {
            RecalculateValues();
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
            SetMaterialDirty();
        }
#if UNITY_EDITOR

        protected override void OnValidate()
        {
            RecalculateValues();

            base.OnValidate();
            SetVerticesDirty();
            SetMaterialDirty();
        }
#endif
        private void RecalculateValues()
        {
            var sizeDelta = rectTransform.sizeDelta;
            cornerDivisions = Mathf.Clamp(cornerDivisions, 0, MaxDivisions);
            cornerRadius = Mathf.Max(0, Mathf.Min(desiredCornerRadius, sizeDelta.x * 0.5f, sizeDelta.y * 0.5f));
        }


        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (cornerDivisions <= 0 || cornerRadius <= 0.00000001f)
            {
                base.OnPopulateMesh(vh);
                return;
            }

            vh.Clear();
            var size = rectTransform.sizeDelta;
            var bottomLeft = rectTransform.pivot * -size;

            LoadExtraCornersVertices(vh, bottomLeft);

            for (var corner = 0; corner < 4; corner++)
            for (var i = 0; i <= cornerDivisions; i++)
                LoadCornerVertices(vh, i, bottomLeft, corner);

            LoadExtraCornersTriangles(vh);

            for (var corner = 0; corner < 4; corner++)
            for (var i = 0; i < cornerDivisions; i++)
                LoadCornerTriangles(vh, i, corner);
        }

        private void LoadExtraCornersTriangles(VertexHelper vh)
        {
            vh.AddTriangle(LeftRectBottomLeft, LeftRectTopRight, LeftRectTopLeft);
            vh.AddTriangle(LeftRectTopRight, LeftRectBottomLeft, LeftRectBottomRight);

            vh.AddTriangle(TopRectBottomLeft, TopRectTopRight, TopRectTopLeft);
            vh.AddTriangle(TopRectTopRight, TopRectBottomLeft, TopRectBottomRight);

            vh.AddTriangle(RightRectBottomLeft, RightRectTopRight, RightRectTopLeft);
            vh.AddTriangle(RightRectTopRight, RightRectBottomLeft, RightRectBottomRight);

            vh.AddTriangle(BottomRectBottomLeft, BottomRectTopRight, BottomRectTopLeft);
            vh.AddTriangle(BottomRectTopRight, BottomRectBottomLeft, BottomRectBottomRight);

            // central rectangle
            vh.AddTriangle(LeftRectTopRight, RightRectBottomLeft, RightRectTopLeft);
            vh.AddTriangle(LeftRectTopRight, LeftRectBottomRight, RightRectBottomLeft);
        }

        private Vector2 ToUV(Vector2 point, Vector2 bottomLeft, Vector2 sizeDelta)
        {
            //using double to avoid loss of precision
            double pointX = point.x;
            double pointY = point.y;
            double bottomLeftX = bottomLeft.x;
            double bottomLeftY = bottomLeft.y;
            double sizeDeltaX = sizeDelta.x;
            double sizeDeltaY = sizeDelta.y;

            return new Vector2
            {
                x = (float) ((pointX - bottomLeftX) / sizeDeltaX),
                y = (float) ((pointY - bottomLeftY) / sizeDeltaY)
            };
        }

        private void LoadExtraCornersVertices(VertexHelper vh, Vector2 bottomLeft)
        {
            var sizeDelta = rectTransform.sizeDelta;
            var topRight = bottomLeft + sizeDelta;

            var vertex = new UIVertex {color = color};

            // LeftRectBottomLeft = 0;
            vertex.position = bottomLeft + new Vector2(0, cornerRadius);
            vertex.uv0 = ToUV(vertex.position, bottomLeft, sizeDelta);
            vh.AddVert(vertex);


            // LeftRectTopLeft = 1;
            vertex.position = bottomLeft + new Vector2(0, sizeDelta.y - cornerRadius);
            vertex.uv0 = ToUV(vertex.position, bottomLeft, sizeDelta);
            vh.AddVert(vertex);


            // LeftRectTopRight = 2;
            vertex.position = bottomLeft + new Vector2(cornerRadius, sizeDelta.y - cornerRadius);
            vertex.uv0 = ToUV(vertex.position, bottomLeft, sizeDelta);
            vh.AddVert(vertex);


            // LeftRectBottomRight = 3;
            vertex.position = bottomLeft + new Vector2(cornerRadius, cornerRadius);
            vertex.uv0 = ToUV(vertex.position, bottomLeft, sizeDelta);
            vh.AddVert(vertex);


            // TopRectTopLeft = 4;
            vertex.position = topRight - new Vector2(sizeDelta.x - cornerRadius, 0);
            vertex.uv0 = ToUV(vertex.position, bottomLeft, sizeDelta);
            vh.AddVert(vertex);


            // TopRectTopRight = 5;
            vertex.position = topRight - new Vector2(cornerRadius, 0);
            vertex.uv0 = ToUV(vertex.position, bottomLeft, sizeDelta);
            vh.AddVert(vertex);


            // TopRectBottomRight = 6;
            vertex.position = topRight - new Vector2(cornerRadius, cornerRadius);
            vertex.uv0 = ToUV(vertex.position, bottomLeft, sizeDelta);
            vh.AddVert(vertex);


            // RightRectBottomLeft = 7;
            vertex.position = topRight - new Vector2(cornerRadius, sizeDelta.y - cornerRadius);
            vertex.uv0 = ToUV(vertex.position, bottomLeft, sizeDelta);
            vh.AddVert(vertex);


            // RightRectTopRight = 8;
            vertex.position = topRight - new Vector2(0, cornerRadius);
            vertex.uv0 = ToUV(vertex.position, bottomLeft, sizeDelta);
            vh.AddVert(vertex);


            // RightRectBottomRight = 9;
            vertex.position = topRight - new Vector2(0, sizeDelta.y - cornerRadius);
            vertex.uv0 = ToUV(vertex.position, bottomLeft, sizeDelta);
            vh.AddVert(vertex);


            // BottomRectBottomLeft = 10;
            vertex.position = bottomLeft + new Vector2(cornerRadius, 0);
            vertex.uv0 = ToUV(vertex.position, bottomLeft, sizeDelta);
            vh.AddVert(vertex);


            // BottomRectBottomRight = 11;
            vertex.position = bottomLeft + new Vector2(sizeDelta.x - cornerRadius, 0);
            vertex.uv0 = ToUV(vertex.position, bottomLeft, sizeDelta);
            vh.AddVert(vertex);
        }

        private void LoadCornerVertices(VertexHelper vh, int i, Vector2 bottomLeft, int corner)
        {
            var sizeDelta = rectTransform.sizeDelta;
            var rotationAmount = Mathf.InverseLerp(0, cornerDivisions, i);
            var point = Vector2.zero;
            var direction = Vector2.zero;

            /*
             * van a girar todos en sentido horario completando el circulo
             */

            switch (corner)
            {
                // bottom left corner
                case 0:
                    point = bottomLeft + new Vector2(cornerRadius, cornerRadius);
                    direction = Vector2.down;
                    break;
                // top left
                case 1:
                    point = bottomLeft + new Vector2(cornerRadius, sizeDelta.y - cornerRadius);
                    direction = Vector2.left;
                    break;
                // top right
                case 2:
                    point = bottomLeft + new Vector2(sizeDelta.x - cornerRadius, sizeDelta.y - cornerRadius);
                    direction = Vector2.up;
                    break;
                // bottom right
                case 3:
                    point = bottomLeft + new Vector2(sizeDelta.x - cornerRadius, cornerRadius);
                    direction = Vector2.right;
                    break;
            }

            direction = Quaternion.Euler(0, 0, 90 * rotationAmount + 270) * direction;

            point += direction * cornerRadius;


            vh.AddVert(new UIVertex
            {
                //color = Color.HSVToRGB(amount, 1, 1),// just 4 debug
                color = color,
                position = point,
                uv0 = ToUV(point, bottomLeft, sizeDelta)
            });
        }

        private void LoadCornerTriangles(VertexHelper vh, int i, int corner)
        {
            var index = ExtraVerticesCount + corner * (cornerDivisions + 1) + i;
            var cornerVertexIndex = 0;
            switch (corner)
            {
                // bottom left corner
                case 0:
                    cornerVertexIndex = LeftRectBottomRight;
                    break;
                // top left
                case 1:
                    cornerVertexIndex = TopRectBottomLeft;
                    break;
                // top right
                case 2:
                    cornerVertexIndex = TopRectBottomRight;
                    break;
                // bottom right
                case 3:
                    cornerVertexIndex = RightRectBottomLeft;
                    break;
            }

            vh.AddTriangle(index, cornerVertexIndex, index + 1);
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace Tools.CustomUITools
{
    [RequireComponent(typeof(Image))]
    public abstract class CustomImageEffect : BaseMeshEffect
    {
        Image img;

        protected Image Image => img ??= GetComponent<Image>();

        protected RectTransform RectTransform => (RectTransform) transform;
        protected VertexHelper vertexHelper;

        protected UIVertex GetVertexBase()
        {
            var simpleVert = UIVertex.simpleVert;
            simpleVert.color = graphic.color;
            return simpleVert;
        }

        protected void RenderTriangle(Vector2 a, Vector2 b, Vector2 c)
        {
            var vertexCount = vertexHelper.currentVertCount;

            var simpleVert = GetVertexBase();
            var vertA = simpleVert;
            vertA.position = a;
            var vertB = simpleVert;
            vertB.position = b;
            var vertC = simpleVert;
            vertC.position = c;
            vertexHelper.AddVert(vertA);
            vertexHelper.AddVert(vertB);
            vertexHelper.AddVert(vertC);
            vertexHelper.AddTriangle(vertexCount + 0, vertexCount + 1, vertexCount + 2);
        }

        protected void RenderRectangle(Vector2 center, Vector2 size)
        {
            var rightUpExtent = size * 0.5f;
            var rightDownExtent = new Vector2(rightUpExtent.x, -rightUpExtent.y);

            var a = center - rightDownExtent;
            var b = center + rightUpExtent;
            var c = center - rightUpExtent;
            var d = center + rightDownExtent;

            var simpleVert = GetVertexBase();

            var vertA = simpleVert;
            vertA.position = a;
            var vertB = simpleVert;
            vertB.position = b;
            var vertC = simpleVert;
            vertC.position = c;
            var vertD = simpleVert;
            vertD.position = d;

            var vertexCount = vertexHelper.currentVertCount;
            vertexHelper.AddVert(vertA);
            vertexHelper.AddVert(vertB);
            vertexHelper.AddVert(vertC);
            vertexHelper.AddVert(vertD);
            // 
            vertexHelper.AddTriangle(vertexCount + 2, vertexCount + 1, vertexCount + 0);
            vertexHelper.AddTriangle(vertexCount + 2, vertexCount + 3, vertexCount + 1);
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            vertexHelper = vh;
            OnApplyEffect();
        }

        protected abstract void OnApplyEffect();
        protected Vector2 RotateV2(Vector2 direction, float angle) => Quaternion.Euler(0, 0, angle) * direction;

        /*
         * Always count angles counter clock wise
         * initial angle will be the angle using (1,0) as a 0 degrees angle reference
         *  and angles will complement initialAngle rotating counter clockwise {angles} degrees
         *
         * NOTE: parameters uses Degrees
         */
        protected void RenderCircularSector(Vector2 center, float initialAngle, float angles, float radius, int steps)
        {
            var initialDirection = RotateV2(new Vector2(radius, 0), initialAngle);


            var stepAngle = angles / steps;
            var currentDirection = initialDirection;
            for (var i = 0; i < steps; i++)
            {
                var nextDirection = RotateV2(currentDirection, stepAngle);
                var a = center;
                var b = a + currentDirection;
                var c = a + nextDirection;
                RenderTriangle(a, b, c);
                currentDirection = nextDirection;
            }
        }

        protected Vector2 FitRectPreservingAspect(Vector2 container, Vector2 imageSize)
        {
            const bool fitContained = true;
            var fitX = new Vector2
            {
                x = container.x,
                y = imageSize.y * container.x / imageSize.x,
            };
            var fitY = new Vector2
            {
                x = imageSize.x * container.y / imageSize.y,
                y = container.y,
            };

            var containerAspect = container.x / container.y;
            var imageAspect = imageSize.x / imageSize.y;


            return ((containerAspect > imageAspect) == fitContained) ? fitY : fitX;
        }

        protected (Vector2 size, Vector2 center) GetRelativeSizeAndCenter(bool userPreserveAspect = true)
        {
            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(RectTransform);

            Vector2 center = bounds.center;
            Vector2 size = bounds.size;

            if (Image.preserveAspect && Image.sprite)
            {
                var spriteSize = Image.sprite.rect.size;

                size = FitRectPreservingAspect(size, spriteSize);
            }


            //center = RectTransform.TransformPoint(center);
            return (size, center);
        }
    }
}
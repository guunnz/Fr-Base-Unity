using UnityEngine;
using static UnityEngine.Mathf;

namespace Tools.CustomUITools
{
    public enum CircleQuality
    {
        High,
        Mid,
        Low
    }

    
    
    public class SuperImage : CustomImageEffect
    {
        public CircleQuality quality = CircleQuality.High;


        public float radius;

        float Radius
        {
            get
            {
                var (size, _) = GetRelativeSizeAndCenter();
                return Min(Min(radius, Abs(size.y / 2)), Abs(size.x / 2));
            }
        }



        public bool topLeft, topRight, bottomLeft, bottomRight;


        bool RenderRectangle()
        {
            return !topLeft && !topRight && !bottomLeft && !bottomRight;
        }

        int Steps => quality switch
        {
            CircleQuality.High => 50,
            CircleQuality.Mid => 25,
            CircleQuality.Low => 10,
            _ => 10,
        };

        void LoadDefaultRectangle()
        {
            vertexHelper.Clear();
            var (size, center) = GetRelativeSizeAndCenter();
            RenderRectangle(center, size);
        }


        protected override void OnApplyEffect()
        {
            if (RenderRectangle())
            {
                //return;
                LoadDefaultRectangle();
            }
            else
            {
                RenderRoundedCorners();
            }
        }


        void RenderRoundedCorners()
        {
            if (!enabled) return;
            vertexHelper.Clear();

            var rad = Radius;

            var halfRadius = rad * 0.5f;

            var (size, center) = GetRelativeSizeAndCenter();
            
            
            var middleRectCenter = center;
            var middleRectSize = new Vector2(size.x - rad * 2, size.y - rad * 2);
            RenderRectangle(middleRectCenter, middleRectSize);

            var bottomTopRectangleSize = new Vector2(middleRectSize.x, rad);
            var bottomRectangleCenter = new Vector2(center.x, center.y - (middleRectSize.y * 0.5f + halfRadius));
            var topRectangleCenter = new Vector2(center.x, center.y + (middleRectSize.y * 0.5f + halfRadius));
            RenderRectangle(bottomRectangleCenter, bottomTopRectangleSize);
            RenderRectangle(topRectangleCenter, bottomTopRectangleSize);


            var sidesRectangleSize = new Vector2(rad, middleRectSize.y);
            var leftRectangleCenter = new Vector2(center.x - (middleRectSize.x / 2 + halfRadius), center.y);
            var rightRectangleCenter = new Vector2(center.x + (middleRectSize.x / 2 + halfRadius), center.y);

            RenderRectangle(leftRectangleCenter, sidesRectangleSize);
            RenderRectangle(rightRectangleCenter, sidesRectangleSize);

            var topRightCorner = new Vector2(center.x + middleRectSize.x / 2, center.y + middleRectSize.y * 0.5f);
            var topLeftCorner = new Vector2(center.x - middleRectSize.x / 2, center.y + middleRectSize.y * 0.5f);
            var bottomRightCorner = new Vector2(center.x + middleRectSize.x / 2, center.y - middleRectSize.y * 0.5f);
            var bottomLeftCorner = new Vector2(center.x - middleRectSize.x / 2, center.y - middleRectSize.y * 0.5f);

            var steps = Steps;
            var radSquareSize = new Vector2(rad, rad);
            var halfRadSquareSize = new Vector2(halfRadius, halfRadius);

            if (topRight)
            {
                RenderCircularSector(topRightCorner, 0, 90, rad, steps);
            }
            else
            {
                RenderRectangle(topRightCorner + halfRadSquareSize, radSquareSize);
            }

            if (topLeft)
            {
                RenderCircularSector(topLeftCorner, 90, 90, rad, steps);
            }
            else
            {
                RenderRectangle(topLeftCorner + new Vector2(-halfRadius, halfRadius), radSquareSize);
            }

            if (bottomLeft)
            {
                RenderCircularSector(bottomLeftCorner, 180, 90, rad, steps);
            }
            else
            {
                RenderRectangle(bottomLeftCorner - halfRadSquareSize, radSquareSize);
            }

            if (bottomRight)
            {
                RenderCircularSector(bottomRightCorner, 270, 90, rad, steps);
            }
            else
            {
                RenderRectangle(bottomRightCorner + new Vector2(halfRadius, -halfRadius), radSquareSize);
            }
        }
    }
}
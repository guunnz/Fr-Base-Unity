using UnityEngine;
using UnityEngine.UI;

namespace Shared.UI
{
    public class LinesAround : MonoBehaviour
    {
        public Image top, bottom, right, left;

        public int pixels;
        public Color lineColor;


        private void OnValidate()
        {
            if (!Initialized()) return;

            Vector2 delta;

            delta = top.rectTransform.sizeDelta;
            delta.y = pixels;
            top.rectTransform.sizeDelta = delta;

            delta = bottom.rectTransform.sizeDelta;
            delta.y = pixels;
            bottom.rectTransform.sizeDelta = delta;

            delta = left.rectTransform.sizeDelta;
            delta.x = pixels;
            left.rectTransform.sizeDelta = delta;

            delta = right.rectTransform.sizeDelta;
            delta.x = pixels;
            right.rectTransform.sizeDelta = delta;

            top.color = lineColor;
            bottom.color = lineColor;
            right.color = lineColor;
            left.color = lineColor;
        }

        private bool Initialized()
        {
            return top && bottom && right && left;
        }
    }
}
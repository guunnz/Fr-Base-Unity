using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Gradients.Example
{
    public class GradientRenderer
    {
        public Texture2D RenderGradient(Gradient gradient, int quality)
        {
            var tex = new Texture2D(quality, 1, TextureFormat.RGB24, false);

            for (var i = 0; i < quality; ++i)
            {
                var color = gradient.Evaluate(Mathf.InverseLerp(0, quality - 1, i));
                tex.SetPixel(i, 0, color);
            }

            return tex;
        }
    }
}
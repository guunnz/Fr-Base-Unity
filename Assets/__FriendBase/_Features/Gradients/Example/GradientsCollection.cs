using System.Collections.Generic;
using Functional.Maybe;
using UnityEngine;
using System.Linq;


namespace Gradients.Example
{
    [CreateAssetMenu(menuName = "Create ListGradients", fileName = "ListGradients", order = 0)]
    public class GradientsCollection : ScriptableObject
    {
        public int quality = 512;
        
        [SerializeField] List<GradientInfo> gradients;

        readonly GradientRenderer renderer = new GradientRenderer();

        public Texture2D GetGradientTexture(int gradientID)
        {
            var gradient = gradients.First(g => g.id == gradientID);
            if (!gradient.render)
            {
                gradient.render = renderer.RenderGradient(gradient.gradient, quality);
                gradient.render.Compress(true);
            }
            return gradient.render;
        }

        public Color GetColorRepresentation(int gradientID)
        {
            return gradients.First(g => g.id == gradientID).representation;
        }
    }
}
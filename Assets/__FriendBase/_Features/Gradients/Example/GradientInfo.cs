using System;
using UnityEngine;
using WebAssets;

namespace Gradients.Example
{
    [Serializable]
    public class GradientInfo
    {
        public int id;
        public Gradient gradient;
        public Color representation;
        public Texture2D render;
    }
}
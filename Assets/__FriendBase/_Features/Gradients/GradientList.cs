using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gradients
{
    [CreateAssetMenu(menuName = "Create GradientList", fileName = "GradientList", order = 0)]
    public class GradientList : ScriptableObject
    {
        [SerializeField] List<Texture2D> gradientsPNG;

        public Texture2D GetTextureByName(string name)
        {
            return gradientsPNG.Find(item => item.name.Equals(name));
        }
    }
}


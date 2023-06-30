using UnityEngine;

namespace Gradients.Example
{
    [RequireComponent(typeof(SpriteRenderer))]

    public class GradientSpriteExample : MonoBehaviour
    {
        static readonly int GradientProperty = Shader.PropertyToID("_Gradient");

        [SerializeField] int gradientID;
        [SerializeField] GradientsCollection gradients;
        private SpriteRenderer spriteRenderer;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.material = new Material(Shader.Find("Friendbase/GradientSprite"));
            var gradientTexture = gradients.GetGradientTexture(gradientID);
            spriteRenderer.material.SetTexture(GradientProperty, gradientTexture);
        }
    }
}
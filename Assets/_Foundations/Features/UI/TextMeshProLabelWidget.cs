using TMPro;
using UnityEngine;

namespace UI
{
    public interface ISetColor
    {
        void SetColor(Color newColor);
    }
    public class TextMeshProLabelWidget : StringWidget, ISetColor
    {
        public TextMeshProUGUI label;
        public Color color;
    

        public override string Value
        {
            get => label.text;
            set
            {
                base.Value = value;
                label.text = value;
            }
        }

        private void Bind()
        {
            Value = value;
            label.color = color;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            FetchComponent();
        }


#endif
        private void FetchComponent()
        {
            if (!label)
            {
                TryGetComponent(out label);
                if (label)
                {
                    color = label.color;
                    base.Value = label.text;
                }
            }
        }


        private void OnValidate()
        {
#if UNITY_EDITOR
            FetchComponent();
#endif

            Bind();
        }

        private void Start()
        {
            FetchComponent();
            if (label)
            {
                Bind();
            }
            else
            {
                Debug.LogError("No label>>>",this);
            }
        }

        public void SetColor(Color newColor)
        {
            label.color = newColor;
            color = newColor;
        }
    }
}
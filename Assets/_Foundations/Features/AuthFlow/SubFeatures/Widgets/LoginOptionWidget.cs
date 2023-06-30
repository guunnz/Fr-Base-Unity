using UI;
using UnityEngine;
using UnityEngine.UI;

namespace AuthFlow.Widgets
{
    public class LoginOptionWidget : MonoBehaviour
    {
        // serialized data
        [SerializeField] Sprite iconSprite;
        [SerializeField] string labelTid;
        [SerializeField] Color color = Color.gray;

        // widget attributes
        [SerializeField] StringWidget label;
        [SerializeField] Image icon;
        [SerializeField] Image tinteable;

#if UNITY_EDITOR
        void OnValidate()
        {
            bool change = false;
            if (icon)
            {
                if (iconSprite)
                {
                    change |= icon.sprite != iconSprite;
                    icon.sprite = iconSprite;
                }
                else
                {
                    if (icon.sprite)
                    {
                        iconSprite = icon.sprite;
                    }
                }
            }

            if (tinteable)
            {
                change |= Vector4.Distance(tinteable.color, color) > 0.000001;
                tinteable.color = color;
            }

            if (label)
            {
                change |= label.Value != labelTid;
                label.Value = labelTid;
            }

            if (change)
            {
                UnityEditor.EditorUtility.SetDirty(iconSprite);
                UnityEditor.EditorUtility.SetDirty(label);
            }

            
        }
#endif


    }
}
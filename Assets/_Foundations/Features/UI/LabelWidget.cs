using UnityEngine;
using Object = UnityEngine.Object;

namespace UI
{
    public abstract class StringWidget : UIWidget<string>
    {
        
    }

    public abstract class UIWidget : MonoBehaviour
    {
    }

    public static class UIWidgetUtil
    {
        public static UIWidget<T> Instantiate<T>(this UIWidget<T> widget, T initialValue, Transform parent = null,
            bool keepOff = false)
        {
            var instance = parent ? Object.Instantiate(widget, parent) : Object.Instantiate(widget);
            instance.Value = initialValue;
            if (!keepOff && !instance.gameObject.activeSelf)
            {
                instance.gameObject.SetActive(true);
            }

            Debug.Log(instance + "  " + initialValue, instance);
            return instance;
        }
    }

    public abstract class UIWidget<T> : UIWidget
    {
        [TextArea]
        [SerializeField] protected T value;

        public virtual T Value
        {
            get => value;
            set => this.value = value;
        }


        private void OnValidate()
        {
            Value = value;
        }
    }
}
using Architecture.ViewManager;
using Shared.Utils;
using TMPro;
using UnityEngine;

namespace _Prefabs.Features.AuthFlow
{
    [ExecuteAlways]
    public class TemplateView : MonoBehaviour
    {
        void Update()
        {
            if (TryGetComponent<ViewNode>(out var node))
            {
                var tmp = GetComponentInChildren<TextMeshProUGUI>();
                var nodeName = node.GetType().Name;
                gameObject.name = nodeName;
                if (tmp)
                {
                    tmp.text = nodeName;
                }

                this.SmartDestroy();
            }
        }
    }
}
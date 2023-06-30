using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class VerticalSize : MonoBehaviour, IResizable
    {
        public float extraSize = 0;

        readonly List<RectTransform> children = new List<RectTransform>();

        void FetchChildren()
        {
            children.Clear();
            for (var i = 0; i < transform.childCount; i++)
            {
                if (!(transform.GetChild(i) is RectTransform rt)) continue;
                if (rt.TryGetComponent<LayoutElement>(out var element) && element.ignoreLayout) continue;

                children.Add(rt);
            }
        }

        float ChildrenSize => children.Sum(t => t.sizeDelta.y) + extraSize;

        RectTransform RectTransform => (RectTransform) transform;

        public void DoResize()
        {
            FetchChildren();
            var height = ChildrenSize;

            var sizeDelta = RectTransform.sizeDelta;
            sizeDelta.y = height;
            RectTransform.sizeDelta = sizeDelta;
        }


#if UNITY_EDITOR
        void OnValidate() => DoResize();
#endif
        void Reset() => DoResize();
        void OnRectTransformDimensionsChange() => DoResize();
        void Start() => DoResize();
        void OnTransformChildrenChanged() => DoResize();

        void OnTransformParentChanged() => DoResize();
    }
}
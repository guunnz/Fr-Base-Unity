using UnityEngine;

namespace Tools
{
    public class CompositeResizable : MonoBehaviour, IResizable
    {
        public void DoResize()
        {
            var elems = GetComponentsInChildren<IResizable>();
            foreach (var elem in elems)
            {
                if (!ReferenceEquals(elem, this))
                {
                    elem.DoResize();
                }
            }
        }
    }
}
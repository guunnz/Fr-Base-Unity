using UnityEngine;
using UnityEngine.UI;

namespace UI.TabController
{
    public class TabController : MonoBehaviour
    {
        private bool selected = false;

        public void SelectTab()
        {
            selected = true;
            OnSelectTab();
        }

        protected virtual void OnSelectTab()
        {

        }

        public void UnselectTab()
        {
            selected = false;
            OnUnselectTab();
        }

        protected virtual void OnUnselectTab()
        {

        }

        public bool IsSelected()
        {
            return selected;
        }
    }
}

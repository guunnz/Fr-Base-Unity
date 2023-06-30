using Architecture.Injector.Core;
using Architecture.ViewManager;
using DeepLink.Presentation;
using UnityEngine;

namespace DeepLink.Delivery
{
    public class DeepLinkHandlerView : ViewNode, IDlhViuew
    {
        protected override void OnInit()
        {
            this.CreatePresenter<DeepLinkHandlerPresenter, IDlhViuew>();
            Debug.Log("creation  " + gameObject + " ok!", gameObject);
        }
    }
}
using System.Linq;
using Architecture.ViewManager;
using DeepLink.Core;
using JetBrains.Annotations;
using Shared.Utils;
using UniRx;
using UnityEngine;

namespace DeepLink.Presentation
{
    [UsedImplicitly]
    public class DeepLinkHandlerPresenter
    {
        private readonly IViewManager viewManager;
        private readonly IDeepLinkService deepLinkService;

        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly IDlhViuew screen;

        public DeepLinkHandlerPresenter(IDlhViuew screen, IViewManager viewManager,
            IDeepLinkService deepLinkService)
        {
            this.screen = screen;
            this.viewManager = viewManager;
            this.deepLinkService = deepLinkService;
            screen.OnShowView.Subscribe(_ => Present()).AddTo(disposables);
            screen.OnDisposeView.Subscribe(_ => CleanUp()).AddTo(disposables);
        }

        private bool TryPerformViewMovement()
        {
            var deepLinkInfo = deepLinkService.GetDeepLinkInfo();

            LogDeepLinkInfo(deepLinkInfo);

            if (deepLinkInfo == null)
            {
                return false;
            }


            Debug.Log(deepLinkInfo.deepLinkEntries);

            var dict = deepLinkInfo.deepLinkEntries;

            return
                Print("trying deep handler") &&
                dict.Pattern("view_movement", true) &&
                Print("view movement ok") &&
                dict.Pattern("view_name", out var viewName) &&
                Print("view name ok") &&
                viewManager.GetOut(viewName) &&
                Print("get out ok");
        }

        private bool Print(string s)
        {
            Debug.Log(s);
            return true;
        }

        private void LogDeepLinkInfo(DeepLinkInfo deepLinkInfo)
        {
            if (deepLinkInfo == null)
            {
                Debug.LogWarning("Deep Link Info Null");
                return;
            }

            if (!deepLinkInfo.ValidDeepLink) return;

            var data = deepLinkInfo
                .deepLinkEntries
                .Select(kv => $" {{ {kv.Key} : {kv.Value} }} ");

            Debug.Log("Deep Link Info : " + string.Join(", ", data));
        }

        void Present()
        {
            
            if (TryPerformViewMovement())
            {
                Debug.Log("no continue navigation");
            }
            else
            {
                Debug.Log("continue navigation");
                ContinueNavigation();
            }
        }


        private void ContinueNavigation()
        {
            viewManager.DebugGetOut("start");
        }

        private void CleanUp()
        {
            disposables.Clear();
        }
    }
}
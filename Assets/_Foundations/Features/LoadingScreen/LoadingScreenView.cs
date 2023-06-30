using Architecture.Context;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace LoadingScreen
{
    public class LoadingScreenView : ViewNode
    {
        public StringWidget moduleName;
        public Image loadingBar;

        private int count = 1;
        private int currentIndex = 0;


        protected override void OnShow()
        {
            var loadingService = Injection.Get<ILoadingService>();
            loadingBar.fillAmount = 0;
            currentIndex = 0;
            count = loadingService.ModulesCount;
            // Debug.Log("on show", this);

            loadingService.OnNewModuleStartLoading.Do(NewModule).Subscribe();
            loadingService.OnLoadModules.First()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .SelectMany(DoWait(0.1f))
                .DoOnError(er => Debug.LogError(er))
                .Subscribe(_ =>
                {
                    var viewManager = Injection.Get<IViewManager>();
                    loadingBar.fillAmount = 1;

                    viewManager.DebugGetOut("start");
                });
        }


        private void NewModule(string module)
        {
            moduleName.Value = module;
            currentIndex++;
            loadingBar.fillAmount = Mathf.InverseLerp(0, count, currentIndex);
        }
    }
}
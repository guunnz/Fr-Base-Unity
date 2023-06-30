using Architecture.Injector.Core;
using Architecture.ViewManager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace WorldNode.View
{
    public class BackToMenuView : MonoBehaviour
    {
        public Button button;

        readonly CompositeDisposable disposables = new CompositeDisposable();

        void Awake()
        {
            button
                .OnClickAsObservable()
                .Subscribe(_ => Injection.Get<IViewManager>().GetOut("menu"))
                .AddTo(disposables);
        }
        
        

        void OnDestroy() => disposables.Clear();
    }
}
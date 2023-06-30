using Architecture.Injector.Core;
using Architecture.ViewManager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ExampleView.View
{
    public class CloseButton : MonoBehaviour
    {
        public Button button;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private void Awake()
        {
            button.OnClickAsObservable().Subscribe(_ => { Injection.Get<IViewManager>().GoBack(); }).AddTo(disposables);
        }

        private void OnDestroy()
        {
            disposables.Clear();
        }
    }
}
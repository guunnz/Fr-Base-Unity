using Architecture.Injector.Core;
using Architecture.ViewManager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MoveViewButton : MonoBehaviour
    {
        public string outPort;
        IViewManager viewManager;
        
        void Start()
        {
            if (TryGetComponent<Button>(out var button))
            {
                button.OnClickAsObservable().Subscribe(() =>
                {
                    viewManager ??= Injection.Get<IViewManager>();
                    viewManager.DebugGetOut(outPort);
                });
            }
        }
    }
}
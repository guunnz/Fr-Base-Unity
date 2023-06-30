using System;
using Architecture.Injector.Core;
using Architecture.MVP;
using Architecture.ViewManager;
using ExampleView.Presentation;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ExampleView.View
{
    [RequireComponent(typeof(Button))]
    public class NavigationButtonWidget : WidgetBase, INavigationButtonWidget
    {
        [SerializeField] private Button button;
        [SerializeField] private string outPort;

        private void Awake()
        {
            var presenter = new NavigationButtonWidgetPresenter(this, Injection.Get<IViewManager>());
        }

        public IObservable<Unit> OnClick => button.OnClickAsObservable();
        public string OutPort => outPort;
    }
}
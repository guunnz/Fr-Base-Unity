using System;
using UniRx;
using UnityEngine;

namespace Architecture.MVP
{
    public class WidgetBase : MonoBehaviour, IPresentable
    {
        readonly ISubject<Unit> onShow = new Subject<Unit>();
        readonly ISubject<Unit> onHide = new Subject<Unit>();
        readonly ISubject<Unit> onDispose = new Subject<Unit>();

        public IObservable<Unit> OnFocusView => Observable.Never<Unit>(); // unimplemented focus for widgets
        public IObservable<Unit> OnUnfocusView => Observable.Never<Unit>(); // unimplemented focus for widgets

        public IObservable<Unit> OnShowView => onShow.ObserveOnMainThread();
        public IObservable<Unit> OnHideView => onHide.ObserveOnMainThread();
        public IObservable<Unit> OnDisposeView => onDispose.ObserveOnMainThread();

        void OnEnable()
        {
            onShow.OnNext(Unit.Default);
        }

        void OnDisable()
        {
            onHide.OnNext(Unit.Default);
        }

        void OnDestroy()
        {
            onDispose.OnNext(Unit.Default);
        }
    }
}
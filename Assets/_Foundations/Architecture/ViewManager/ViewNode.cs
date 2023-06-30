using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.MVP;
using UniRx;
using UnityEngine;

namespace Architecture.ViewManager
{
    public interface INode : IDisposable
    {
        public bool Enabled { get; }
    }

    public abstract class ViewNode : MonoBehaviour, IPresentable, INode
    {
        public RectTransform RectTransform => transform as RectTransform;

        readonly ISubject<Unit> onShow = new Subject<Unit>();
        readonly ISubject<Unit> onHide = new Subject<Unit>();
        readonly ISubject<Unit> onDispose = new Subject<Unit>();
        readonly ISubject<Unit> onFocusView = new Subject<Unit>();
        readonly ISubject<Unit> onUnfocusView = new Subject<Unit>();

        protected readonly CompositeDisposable showDisposables = new CompositeDisposable();

        public IObservable<Unit> OnFocusView => onFocusView.ObserveOnMainThread();
        public IObservable<Unit> OnUnfocusView => onUnfocusView.ObserveOnMainThread();
        public IObservable<Unit> OnShowView => onShow.ObserveOnMainThread();
        public IObservable<Unit> OnHideView => onHide.ObserveOnMainThread();
        public IObservable<Unit> OnDisposeView => onDispose.ObserveOnMainThread();

        protected IReadOnlyList<object> Parameters { get; private set; } = Array.Empty<object>();

        public void Dispose()
        {
            showDisposables.Clear();
            OnDispose();
            onDispose.OnNext(Unit.Default);
            if (this && gameObject)
                Destroy(gameObject);
        }

        public ViewNode SetParameters(object[] parameters)
        {
            Parameters = parameters;
            return this;
        }

        private bool initialized = false;


        protected IObservable<Unit> DoWait(float time)
        {
            IEnumerator Routine()
            {
                yield return new WaitForSeconds(time);
            }

            return Routine().ToObservable();
        }


        public void ShowView()
        {
            showDisposables.Clear();
            if (!initialized)
            {
                initialized = true;
                OnInit();
            }

            gameObject.SetActive(true);
            OnShow();
            onShow.OnNext(Unit.Default);
            //Debug.Log("On Show " + GetType().Name + " - " + gameObject, gameObject);
        }

        public void HideView()
        {
            showDisposables.Clear();
            OnHide();
            onHide.OnNext(Unit.Default);
            gameObject.SetActive(false);
            //Debug.Log("On Hide " + GetType().Name + " - " + gameObject, gameObject);
        }

        void OnDestroy()
        {
            //Debug.Log(" destroyed " + GetType().Name + " -> " + gameObject);
        }

        public void Focus()
        {
        }

        public void Unfocus()
        {
        }

        protected virtual void OnFocus()
        {
        }

        protected virtual void OnUnfocus()
        {
        }

        protected virtual void OnInit()
        {
        }

        protected virtual void OnDispose()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        public bool Enabled => gameObject && gameObject.activeSelf && gameObject.activeInHierarchy && enabled;
    }
}
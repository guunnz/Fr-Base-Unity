using System;
using Architecture.ViewManager;
using AuthFlow.Terms.Core.Actions;
using AuthFlow.Terms.Core.Services;
using JetBrains.Annotations;
using Localization.Actions;
using UniRx;
using UnityEngine;

namespace AuthFlow.Terms.Presentation
{
    [UsedImplicitly]
    public class TermsPresenter
    {
        readonly ITermsView view;
        readonly GetLanguageKey getLanguageKey;
        readonly IViewManager viewManager;
        readonly CompositeDisposable disposables = new CompositeDisposable();
        readonly CompositeDisposable sessionDisposables = new CompositeDisposable();
        readonly SetTermsAccepted setTermsAccepted;
        readonly ITermsService termsService;

        public TermsPresenter(ITermsView view, GetLanguageKey getLanguageKey, IViewManager viewManager,
            SetTermsAccepted setTermsAccepted, ITermsService termsService)
        {
            this.view = view;
            this.getLanguageKey = getLanguageKey;
            this.viewManager = viewManager;
            this.setTermsAccepted = setTermsAccepted;
            this.termsService = termsService;
            view.OnShowView.Subscribe(Present).AddTo(disposables);
            view.OnHideView.Subscribe(Hide).AddTo(disposables);
            view.OnDisposeView.Subscribe(CleanUp).AddTo(disposables);
            sessionDisposables.AddTo(disposables);
        }

        void Present()
        {
            UpdateView().Subscribe().AddTo(sessionDisposables);
            view.OnSelectLanguageModalClose.SelectMany(_ => UpdateView()).Subscribe().AddTo(sessionDisposables);
            view.OnAccept.Subscribe(AcceptTerms).AddTo(sessionDisposables);
            view.OnShowSelectLanguage.Subscribe(ShowSelectLanguage).AddTo(sessionDisposables);
        }

        void ShowSelectLanguage()
        {
            view.ShowSelectLanguage();
        }

        // TODO(Jesse): Does this throw exceptions?  What happens if the terms
        // network request fails?
        IObservable<Unit> UpdateView()
        {
            view.SetLoadingPanelActive(true);

            var langKey = getLanguageKey.Execute();
            return termsService
                .GetTerms(langKey)
                .First()
                .Do(a => Debug.Log(a.title + ", " + a.content))
                .Do(() => view.SetLanguageKey(langKey))
                .Do(terms => view.SetTitle(terms.title))
                .Do(terms => view.SetTermsAndConditions(terms.content))
                .DoOnError(e => view.SetLoadingPanelActive(false))
                .AsUnitObservable();
        }

        void AcceptTerms()
        {
            setTermsAccepted.Execute();
            view.CloseWebView();
            viewManager.GetOut("next-view");
        }

        void Hide()
        {
            sessionDisposables.Clear();
        }

        void CleanUp()
        {
        }
    }
}

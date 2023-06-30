using System;
using Architecture.MVP;
using UniRx;

namespace AuthFlow.Terms.Presentation
{
    public interface ITermsView : IPresentable
    {
        IObservable<Unit> OnAccept { get; }
        IObservable<Unit> OnSelectLanguageModalClose { get; }
        IObservable<Unit> OnShowSelectLanguage { get; }
        void SetLanguageKey(string langKey);
        void ShowSelectLanguage();
        void SetTitle(string title);
        void SetTermsAndConditions(string termsAndConditions);
        void SetLoadingPanelActive(bool state);

        void CloseWebView();
    }
}

using System;
using System.Collections;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using AuthFlow.Terms.Presentation;
using LocalizationSystem;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace AuthFlow.Terms.View
{
    public class TermsView : ViewNode, ITermsView
    {
        [SerializeField] Button acceptButton;
        [SerializeField] Button selectLanguageButton;
        //[SerializeField] Image languageSelectedIcon;
        [SerializeField] StringWidget languageSelectedLabel;
        [SerializeField] CanvasWebView webView;
        [SerializeField] GameObject webViewContainer;
        //[SerializeField] StringWidget title;
        //[SerializeField] StringWidget content;
        public string termsAndConditionsUrl = "https://friendbase.com/terms-of-use-cookie-policy-app/";
        [SerializeField] LanguageInfo langInfos;
        ISelectLanguageView selectLanguageView;

        private ILanguage language;

        [SerializeField] GameObject loadingPanel;

        protected override void OnInit()
        {
            selectLanguageView = GetComponentInChildren<ISelectLanguageView>(true);
            this.CreatePresenter<TermsPresenter, ITermsView>();
        }

        IEnumerator Start()
        {
            language = Injection.Get<ILanguage>();
            yield return null;
            termsAndConditionsUrl = language.GetTextByKey(LangKeys.AUTH_TERMS_AND_CONDITIONS_LINK);
            yield return new WaitForFixedUpdate();
            SetWebView();
        }

        public IObservable<Unit> OnAccept => acceptButton.OnClickAsObservable();
        public IObservable<Unit> OnShowSelectLanguage => selectLanguageButton.OnClickAsObservable();
        public IObservable<Unit> OnSelectLanguageModalClose => selectLanguageView.ViewClosed;

        public void SetLanguageKey(string langKey)
        {
            SetWebView();
            //languageSelectedIcon.sprite = langInfos.GetSprite(langKey);
            languageSelectedLabel.Value = langKey.ToUpper();
        }

        public void ShowSelectLanguage()
        {
            webView.close();
            selectLanguageView.Show();
        }

        public void SetWebView()
        {
            termsAndConditionsUrl = language.GetTextByKey(LangKeys.AUTH_TERMS_AND_CONDITIONS_LINK);

            webView.setUrl(termsAndConditionsUrl);
        }
        public void CloseWebView()
        {
            webView.close();
        }

        public void SetLoadingPanelActive(bool state)
        {
            loadingPanel.SetActive(state);
            acceptButton.gameObject.SetActive(!state);
        }

        public void SetTermsAndConditions(string termsAndConditions)
        {
            SetLoadingPanelActive(false);
            //content.Value = termsAndConditions;
        }

        public void SetTitle(string text)
        {
            //title.Value = text;
        }
    }
}

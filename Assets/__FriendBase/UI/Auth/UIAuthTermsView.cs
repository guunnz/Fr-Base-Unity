using System;
using Architecture.Injector.Core;
using AuthFlow.EndAuth.Repo;
using AuthFlow.Terms.Core.Services;
using AuthFlow.Terms.Presentation;
using AuthFlow.Terms.View;
using Localization.Actions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Auth
{
    public class UIAuthTermsView : AbstractUIPanel
    {
        [SerializeField] private UIAuthSelectLanguage selectLanguagePanel;
        [SerializeField] private TextMeshProUGUI TxtButtonAccept;

        //public Button acceptButton;
        //public Image languageSelectedIcon;
        public StringWidget languageSelectedLabel;
        //public StringWidget title;
        //public StringWidget content;
        public LanguageInfo langInfos;

        private ITermsService _termsService;
        private AuthFlowManager _authFlowManager;

        protected override void Start()
        {
            base.Start();
            _termsService = Injection.Get<ITermsService>();
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];

            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }
            selectLanguagePanel.OnSelectLanguage += OnSelectLanguage;
        }

        private void OnDestroy()
        {
            selectLanguagePanel.OnSelectLanguage -= OnSelectLanguage;
        }

        public override void OnOpen()
        {
            UpdateUI();
        }

        public void OnAccept()
        {
            ILocalUserInfo userInfo = Injection.Get<ILocalUserInfo>();
            userInfo["terms"] = "True";
            IAnalyticsSender analyticsSender = Injection.Get<IAnalyticsSender>();
            analyticsSender.SendAnalytics(AnalyticsEvent.AcceptTerms);
            _authFlowManager.Finish(setTerms: true);
        }

        public void ShowLanguageSelector()
        {
            selectLanguagePanel.Open();
        }

        public void OnSelectLanguage(string langKey)
        {
            language.SetCurrentLanguage(langKey);
            UpdateUI();
            GetTerms();
        }

        private async void GetTerms()
        {

            (string termsTitle, string termsContent) = await _termsService
                .GetTerms(language.GetCurrentLanguage())
                .ObserveOnMainThread()
                .ToTask();

            // TODO: Error state
            // ..
            SetTermsAndConditions(termsContent);
            SetLoadingPanelActive(false);
        }

        private void SetLoadingPanelActive(bool state)
        {
            _authFlowManager.SetLoading(state);
        }

        private void SetTermsAndConditions(string termsAndConditions)
        {
            SetLoadingPanelActive(false);
        }

        private void UpdateUI()
        {
            string langKey = language.GetCurrentLanguage();

            languageSelectedLabel.Value = langKey.ToUpper();
            language.SetTextByKey(TxtButtonAccept, LangKeys.AUTH_ACCEPT);
        }
    }
}
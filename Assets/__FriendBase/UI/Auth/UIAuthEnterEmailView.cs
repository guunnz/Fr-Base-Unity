using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Architecture.Injector.Core;
using AuthFlow;
using TMPro;
using UnityEngine;

namespace UI.Auth
{
    public class UIAuthEnterEmailView : AbstractUIPanel
    {
        public UIComponentInput inputField;
        public TMP_Text errorText;
        public GameObject continueButton;
        public TextMeshProUGUI TxtTitle;
        public TextMeshProUGUI TxtButton;

        public AuthFirebaseManager authFirebaseManager;
        private AuthFlowManager _authFlowManager;

        private void Awake()
        {
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];

            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }
        }

        protected override void Start()
        {
            base.Start();
            inputField.SetLabel(language.GetTextByKey(LangKeys.AUTH_EMAIL));
            inputField.SetPlaceholder(language.GetTextByKey(LangKeys.AUTH_ENTER_YOUR_EMAIL));
            if (_authFlowManager.LegacyLogin)
            {
                if (_authFlowManager.LegacyFacebookLogin)
                {
                    language.SetTextByKey(TxtTitle, LangKeys.LEGACY_ENTER_EMAIL_FACEBOOK);
                }
                else
                {
                    language.SetTextByKey(TxtTitle, LangKeys.AUTH_ENTER_YOUR_EMAIL);
                }
            }
            else
            {
                language.SetTextByKey(TxtTitle, LangKeys.AUTH_ENTER_YOUR_EMAIL);
            }
            language.SetTextByKey(TxtButton, LangKeys.COMMON_LABEL_CONTINUE);
        }

        private async void OnValidEmailSubmit(string inputText)
        {
            // Set the email
            authFirebaseManager.Email = inputText;
            Injection.Get<IAnalyticsSender>().SendAnalytics(AnalyticsEvent.EmailLoginButton);
            // Set the providers associates with the email
            IEnumerable<string> providers = await JesseUtils.EmailProviders(inputText);

            if (_authFlowManager.LegacyLogin && (providers.Contains(AuthProvidersFirebase.PASSWORD) || providers.Contains("email")))
            {
                _authFlowManager.UICheckEmailLegacyView.emailAddress = inputText;
                _authFlowManager.SetView(_authFlowManager.UICheckEmailLegacyView);
                return;
            }
            else if (_authFlowManager.LegacyLogin)
            {
                AuthModalError modalError2 = _authFlowManager.modalError;
                modalError2.Show(
                    language.GetTextByKey(LangKeys.LEGACY_INCORRECT_EMAIL),
                    language.GetTextByKey(LangKeys.LEGACY_CHECK_IF_WROTE_CORRECTLY),
                    null
                );
                return;
            }

            if (providers.Count() == 0 || providers.Contains(AuthProvidersFirebase.PASSWORD)) // "password" is the provider when the user registered with email
            {
                authFirebaseManager.EmailProviders = providers.ToList();
                // Go to next screen

                _authFlowManager.SetView(_authFlowManager.UILoginView);
                return;
            }

            AuthModalError modalError = _authFlowManager.modalError;
            modalError.Show(
                modalError.TemplateEmailInUse.Title,
                modalError.TemplateEmailInUse.Message + $"<br><br> Use {providers.First().ToString()} to login",
                null
            );
        }

        public void OnSubmit()
        {
            Regex regexEmail = new Regex(@"^([\w\.\-\+]+)@([\w\-]+)((\.(\w){2,3})+)$");
            string inputText = inputField.value;

            if (string.IsNullOrEmpty(inputText))
            {
                inputField.SetError(language.GetTextByKey(LangKeys.AUTH_FIELD_REQUIRED));
            }
            else if (regexEmail.Match(inputText).Success)
            {
                OnValidEmailSubmit(inputText);
            }
            else
            {
                inputField.SetError(language.GetTextByKey(LangKeys.AUTH_ENTER_VALID_EMAIL));
            }
        }

        public void OnBack()
        {
            if (_authFlowManager.LegacyLogin)
            {
                _authFlowManager.SetView(_authFlowManager.UILandingViewLegacy);
            }
            else
            {
                _authFlowManager.SetView(_authFlowManager.UILandingView);
            }
        }
    }
}

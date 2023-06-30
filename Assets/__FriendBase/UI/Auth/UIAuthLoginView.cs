using System;
using Architecture.Injector.Core;
using AuthFlow.Infrastructure;
using TMPro;
using UnityEngine;

namespace UI.Auth
{
    public class UIAuthLoginView : AbstractUIPanel
    {
        public TMP_Text screenTitle;
        public UIComponentInput emailAddress;
        public UIComponentInput passwordInputField;
        public GameObject continueButton;
        public GameObject forgotPassButton;
        public AuthFirebaseManager authFirebaseManager;

        private PasswordValidator _passwordValidator;
        private AuthFlowManager _authFlowManager;

        public TextMeshProUGUI TxtTitle;
        public TextMeshProUGUI TxtButtonContinue;
        public TextMeshProUGUI TxtButtonForgot;
        IAnalyticsSender analyticsSender;
        private void Awake()
        {
            _passwordValidator = new PasswordValidator();
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];

            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void OnOpen()
        {
            Debug.Log("Open login");
            Injection.Get<ILoading>().Unload();
            language.SetTextByKey(screenTitle, LangKeys.AUTH_LOG_INTO_YOUR_ACCOUNT);
            emailAddress.value = authFirebaseManager.Email;
            emailAddress.SetLabel(language.GetTextByKey(LangKeys.AUTH_EMAIL));
            emailAddress.SetPlaceholder(language.GetTextByKey(LangKeys.AUTH_ENTER_YOUR_EMAIL));
            emailAddress.SetValues();

            passwordInputField.SetLabel(language.GetTextByKey(LangKeys.AUTH_PASSWORD));
            passwordInputField.SetPlaceholder(language.GetTextByKey(LangKeys.AUTH_ENTER_YOUR_PASSWORD));
            //passwordInputField.SetMessageError(language.GetTextByKey(LangKeys.AUTH_ENTER_YOUR_PASSWORD));
            passwordInputField.SetValues();

            language.SetTextByKey(TxtTitle, LangKeys.AUTH_LOG_INTO_YOUR_ACCOUNT);
            language.SetTextByKey(TxtButtonContinue, LangKeys.COMMON_LABEL_CONTINUE);
            language.SetTextByKey(TxtButtonForgot, LangKeys.AUTH_FORGOT);

            // If user has no account with that email, show sign up screen
            if (_authFlowManager.LegacyLogin)
            {
                forgotPassButton.SetActive(false);
                language.SetTextByKey(screenTitle, LangKeys.AUTH_LOG_INTO_YOUR_ACCOUNT);
            }
            else if (authFirebaseManager.EmailProviders.Count == 0)
            {
                forgotPassButton.SetActive(false);
                language.SetTextByKey(screenTitle, LangKeys.AUTH_CREATE_AN_ACCOUNT);
            }
            else
            {
                forgotPassButton.SetActive(true);
            }

        }

        public void OnForgotPassword()
        {
            analyticsSender = Injection.Get<IAnalyticsSender>();
            analyticsSender.SendAnalytics(AnalyticsEvent.ForgotPassword);
            _authFlowManager.UICheckEmailView.ForgotPassword = true;
            _authFlowManager.SetView(_authFlowManager.UICheckEmailView);
            _authFlowManager.UICheckEmailView.SendForgotPassword();
        }

        public async void OnSubmit()
        {
            string inputText = passwordInputField.value;
            (bool passwordIsValid, string validationError) = _passwordValidator.Validate(inputText);

            if (!passwordIsValid)
            {
                passwordInputField.SetError(validationError);
                return;
            }

            _authFlowManager.SetLoading(true);

            analyticsSender = Injection.Get<IAnalyticsSender>();
            // Login
            if (authFirebaseManager.UserHasAccount() || _authFlowManager.LegacyLogin)
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.LoginAccountContinue);
                (bool userAuthenticated, string loginError) = await authFirebaseManager.SignInWithEmail(inputText);

                //_authFlowManager.SetLoading(false);

                if (userAuthenticated)
                {
                    _authFlowManager.Finish();
                    return;
                }
                else
                {
                    _authFlowManager.SetLoading(false);
                    passwordInputField.SetError(loginError);
                    return;
                }
            }

            // Create email account
            if (!authFirebaseManager.UserHasAccount())
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.CreateAccountContinue);
                bool userCreated = await authFirebaseManager.SignUpWithEmail(authFirebaseManager.Email, inputText);

                _authFlowManager.SetLoading(false);
                if (userCreated)
                {
                    _authFlowManager.Finish();
                }
            }
        }

        public void OnBack()
        {
            _authFlowManager.SetView(_authFlowManager.UIEnterEmailView);
        }
    }
}
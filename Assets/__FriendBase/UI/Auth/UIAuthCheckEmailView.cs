using System;
using System.Collections;
using System.Linq;
using AuthFlow;
using Firebase.Auth;
using TMPro;
using UnityEngine;
using Architecture.Injector.Core;
using Data;

namespace UI.Auth
{
    public class UIAuthCheckEmailView : AbstractUIPanel
    {
        public TMP_Text screenTitle;
        public TMP_Text subtitle1;
        public TMP_Text subtitle2;
        public UIComponentInput emailAddress;
        public CanvasGroup resendCanvasGroup;
        private const string QueryRoot = "?userID=";
        private AuthFirebaseManager _authFirebaseManager;
        private AuthFlowManager _authFlowManager;
        public TextMeshProUGUI TxtEmailDidNotArrive;
        public TextMeshProUGUI TxtBtnResend;
        internal bool ForgotPassword = false;

        IAnalyticsSender analyticsSender;
        protected override void Start()
        {
            base.Start();
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];
            _authFirebaseManager = FindObjectsOfType<AuthFirebaseManager>()[0];

            analyticsSender = Injection.Get<IAnalyticsSender>();
            if (_authFirebaseManager == null)
            {
                throw new Exception("AuthFirebaseManager not found");
            }
        }



        public override void OnOpen()
        {
            if (_authFirebaseManager.Email == "")
            {
                // Try to obtain the email address from the provider data
                _authFirebaseManager.Email = FirebaseAuth.DefaultInstance.CurrentUser.ProviderData.First().Email;
            }
            emailAddress.value = _authFirebaseManager.Email;
            emailAddress.SetValues();
            if (!_authFlowManager.LegacyLogin &&_authFirebaseManager.UserHasAccount() && (FirebaseAuth.DefaultInstance.CurrentUser != null && FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified) || ForgotPassword && !_authFlowManager.LegacyLogin)
            {

                language.SetTextByKey(screenTitle, LangKeys.AUTH_CHECK_YOUR_EMAIL);
                language.SetTextByKey(subtitle1, LangKeys.AUTH_SENT_RECOVER_EMAIL_TO);
                language.SetTextByKey(subtitle2, LangKeys.AUTH_CLICK_ON_THE_LINK);
            }
            else
            {
                language.SetTextByKey(screenTitle, LangKeys.AUTH_VERIFY_YOUR_EMAIL);
                language.SetTextByKey(subtitle1, LangKeys.AUTH_SENT_EMAIL_TO);
                language.SetTextByKey(subtitle2, LangKeys.AUTH_CLICK_ON_THE_LINK_IN);

                StartCoroutine(CheckVerifyEmail());
            }

            language.SetTextByKey(TxtEmailDidNotArrive, LangKeys.AUTH_EMAIL_DIDNT_ARRIVE);
            language.SetTextByKey(TxtBtnResend, LangKeys.AUTH_RESEND);

            if (!ForgotPassword || _authFlowManager.LegacyLogin)
                SendEmail();
        }

        public override void OnClose()
        {
            StopCoroutine(CheckVerifyEmail());
        }


        public void SendForgotPassword()
        {
            // Send email
            if (_authFirebaseManager.UserHasAccount() && !_authFlowManager.LegacyLogin)
            {
                JesseUtils.SendEmailResetPassword(_authFirebaseManager.Email);
            }
            else
            {
                JesseUtils.SendEmailVerification(_authFirebaseManager.Email);
            }

            if (resendCanvasGroup != null)
            {
                resendCanvasGroup.alpha = 0;
                resendCanvasGroup.interactable = false;
                StartCoroutine(EnableResendButton());
            }
            ForgotPassword = false;
        }

        public void SendEmail(bool resend = false)
        {
            if (resend)
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.ResendEmailVerification);
            }
            // Send email
            if (_authFlowManager.LegacyLogin)
            {
                Debug.Log("***** SendEmail LegacyLogin");
                JesseUtils.LegacyEmailVerification(_authFirebaseManager.Email);
            }
            else if (_authFirebaseManager.UserHasAccount())
            {
                if (FirebaseAuth.DefaultInstance.CurrentUser == null || FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified)
                {
                    Debug.Log("***** SendEmail 00");
                    JesseUtils.SendEmailResetPassword(_authFirebaseManager.Email);
                }
                else
                {
                    Debug.Log("***** SendEmail 01");
                    JesseUtils.SendEmailVerification(_authFirebaseManager.Email);
                }
            }
            else
            {
                Debug.Log("***** SendEmail 02");
                JesseUtils.SendEmailVerification(_authFirebaseManager.Email);
            }

            if (resendCanvasGroup != null)
            {
                resendCanvasGroup.alpha = 0;
                resendCanvasGroup.interactable = false;
                StartCoroutine(EnableResendButton());
            }
        }

        // Wait for email verification
        private IEnumerator CheckVerifyEmail()
        {
            while (FirebaseAuth.DefaultInstance.CurrentUser == null)
                yield return null;

            while (!FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified)
            {
                yield return new WaitForSeconds(1);
                yield return FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync();
            }

            analyticsSender.SendAnalytics(AnalyticsEvent.EmailVerified);

            _authFlowManager.Finish();
        }

        // Wait for 3 seconds before enabling resend button
        private IEnumerator EnableResendButton()
        {
            yield return new WaitForSeconds(3);

            resendCanvasGroup.alpha = 1;
            resendCanvasGroup.interactable = true;
        }

        public void OnBack()
        {
            _authFlowManager.SetView(_authFlowManager.UILoginView);
        }
    }
}

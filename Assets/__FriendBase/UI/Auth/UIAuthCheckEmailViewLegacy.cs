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
    public class UIAuthCheckEmailViewLegacy : AbstractUIPanel
    {
        public TMP_Text screenTitle;
        public TMP_Text subtitle1;
        public TMP_Text subtitle2;
        public string emailAddress;
        public UIComponentInput emailAddressField;
        public CanvasGroup resendCanvasGroup;
        private AuthFirebaseManager _authFirebaseManager;
        private AuthFlowManager _authFlowManager;
        public TextMeshProUGUI TxtEmailDidNotArrive;
        public TextMeshProUGUI TxtBtnResend;
        internal bool ForgotPassword = false;

        protected override void Start()
        {
            base.Start();
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];
            _authFirebaseManager = FindObjectsOfType<AuthFirebaseManager>()[0];

            if (_authFirebaseManager == null)
            {
                throw new Exception("AuthFirebaseManager not found");
            }
        }

        public override void OnOpen()
        {


            language.SetTextByKey(screenTitle, LangKeys.LEGACY_EMAIL_SENT);
            language.SetTextByKey(subtitle1, LangKeys.LEGACY_CHECK_EMAIL);
            language.SetTextByKey(subtitle2, LangKeys.LEGACY_SET_NEW_PASSWORD);

            emailAddressField.value = emailAddress;
            emailAddressField.inputFieldText.text = emailAddress;
            language.SetTextByKey(TxtEmailDidNotArrive, LangKeys.AUTH_EMAIL_DIDNT_ARRIVE);
            language.SetTextByKey(TxtBtnResend, LangKeys.AUTH_RESEND);

            SendForgotPassword();
        }


        public void SendForgotPassword()
        {

            JesseUtils.SendEmailResetPassword(emailAddress);


            if (resendCanvasGroup != null)
            {
                resendCanvasGroup.alpha = 0;
                resendCanvasGroup.interactable = false;
                StartCoroutine(EnableResendButton());
            }
        }

        private IEnumerator EnableResendButton()
        {
            yield return new WaitForSeconds(3);

            resendCanvasGroup.alpha = 1;
            resendCanvasGroup.interactable = true;
        }

        public void Continue()
        {
            _authFlowManager.SetView(_authFlowManager.UILoginView);
        }

        public void OnBack()
        {
            _authFlowManager.SetView(_authFlowManager.UILoginView);
        }
    }
}

using Architecture.Injector.Core;
using System;
using TMPro;
using UnityEngine;

namespace UI.Auth
{
    public class UIAuthLandingView : AbstractUIPanel
    {
        [SerializeField] private TextMeshProUGUI TxtButtonEmail;
        [SerializeField] private TextMeshProUGUI TxtButtonGoogle;
        [SerializeField] private TextMeshProUGUI TxtButtonFacebook;
        [SerializeField] private TextMeshProUGUI TxtButtonApple;

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
            language.SetTextByKey(TxtButtonEmail, LangKeys.AUTH_CONTINUE_EMAIL);
            language.SetTextByKey(TxtButtonGoogle, LangKeys.AUTH_CONTINUE_GOOGLE);
            language.SetTextByKey(TxtButtonFacebook, LangKeys.AUTH_CONTINUE_FACEBOOK);
            language.SetTextByKey(TxtButtonApple, LangKeys.AUTH_CONTINUE_APPLE);
        }

        public void OnContinueEmail()
        {
            Injection.Get<IAnalyticsSender>().SendAnalytics(AnalyticsEvent.EmailLoginButton);
            _authFlowManager.SetView(_authFlowManager.UIEnterEmailView);
        }
    }
}

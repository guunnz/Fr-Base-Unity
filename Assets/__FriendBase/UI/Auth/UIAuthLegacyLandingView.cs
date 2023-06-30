using System;
using TMPro;
using UnityEngine;

namespace UI.Auth
{
    public class UIAuthLegacyLandingView : AbstractUIPanel
    {
        [SerializeField] private TextMeshProUGUI TxtButtonEmail;
        [SerializeField] private TextMeshProUGUI TxtButtonFacebook;
        [SerializeField] private TextMeshProUGUI TxtWelcomeBack;
        [SerializeField] private TextMeshProUGUI TxtCloseRestoreTitle;
        [SerializeField] private TextMeshProUGUI TxtCloseRestoreDesc;
        [SerializeField] private TextMeshProUGUI TxtYes;
        [SerializeField] private TextMeshProUGUI TxtNoStay;

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
            language.SetTextByKey(TxtButtonEmail, LangKeys.LEGACY_I_USED_MY_EMAIL);
            language.SetTextByKey(TxtButtonFacebook, LangKeys.LEGACY_I_USED_MY_FACEBOOK);
            language.SetTextByKey(TxtWelcomeBack, LangKeys.LEGACY_WELCOME_BACK);
            language.SetTextByKey(TxtCloseRestoreTitle, LangKeys.LEGACY_RESTORE);
            language.SetTextByKey(TxtCloseRestoreDesc, LangKeys.LEGACY_REDIRECT);
            language.SetTextByKey(TxtYes, LangKeys.GENERAL_YES);
            language.SetTextByKey(TxtNoStay, LangKeys.LEGACY_NO_STAY);
        }

        public void OnContinueEmail()
        {
            _authFlowManager.SetView(_authFlowManager.UIEnterEmailView);
        }
    }
}

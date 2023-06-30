using System;
using Architecture.Injector.Core;
using AuthFlow.Infrastructure;
using Firebase.Auth;
using Firebase.Extensions;
using LocalizationSystem;
using TMPro;
using UnityEngine;

namespace UI.Auth
{
    public class UIAuthPostLegacyLogin : AbstractUIPanel
    {
        [SerializeField] private TextMeshProUGUI TxtTitle;
        [SerializeField] private TextMeshProUGUI TxtDescription;
        [SerializeField] private TextMeshProUGUI TxtButtonContinue;

        [SerializeField] private TextMeshProUGUI TxtModalTitle;
        [SerializeField] private TextMeshProUGUI TxtModalDescription;

        private AuthFlowManager _authFlowManager;

        protected override void Start()
        {
            base.Start();
            language.SetTextByKey(TxtTitle, LangKeys.LEGACY_DONE);
            language.SetTextByKey(TxtDescription, LangKeys.LEGACY_FROM_NOW_ON);
            language.SetTextByKey(TxtModalTitle, LangKeys.LEGACY_WHAT_IS_GOING_ON);
            language.SetTextByKey(TxtModalDescription, LangKeys.LEGACY_WORKING_HARD);
            language.SetTextByKey(TxtButtonContinue, LangKeys.COMMON_LABEL_CONTINUE);
        }

        private void Awake()
        {
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];

            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }
        }

        public void OnSubmit()
        {
            _authFlowManager.Finish();
        }

        public void OnBack()
        {
            _authFlowManager.SetView(_authFlowManager.UILoginView);
        }
    }
}
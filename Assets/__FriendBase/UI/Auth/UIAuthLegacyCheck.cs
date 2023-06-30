using System;
using TMPro;
using UnityEngine;

namespace UI.Auth
{
    public class UIAuthLegacyCheck : AbstractUIPanel
    {
        [SerializeField] private TextMeshProUGUI TxtDescription;
        [SerializeField] private TextMeshProUGUI TxtTitle;
        [SerializeField] private TextMeshProUGUI TxtSkip;
        [SerializeField] private TextMeshProUGUI TxtGoToRestore;

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
            language.SetTextByKey(TxtTitle, LangKeys.LEGACY_HEY);
            language.SetTextByKey(TxtDescription, LangKeys.LEGACY_ACCOUNT_CREATED_BEFORE);
            language.SetTextByKey(TxtSkip, LangKeys.LEGACY_SKIP);
            language.SetTextByKey(TxtGoToRestore, LangKeys.LEGACY_GO_TO_RESTORE);
        }
    }
}

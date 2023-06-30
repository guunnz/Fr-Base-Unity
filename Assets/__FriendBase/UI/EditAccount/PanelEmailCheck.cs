using AuthFlow;
using BurguerMenu.Core.Domain;
using BurguerMenu.Infractructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.EditAccount
{
    public class PanelEmailCheck : AbstractUIPanel
    {
        public EditAccountManager editAccountManager;

        [SerializeField] private Button close;

        [SerializeField] private Button back;

        [SerializeField] private Button resendEmail;

        [SerializeField] private TMP_InputField emailText;

        private string email;

        private bool isReady;

        public enum CheckEmailType
        {
            ChangeEmail,
            ResetPassword,
            None
        }

        private CheckEmailType type;
        private EditAccountSections returnToSection;

        private void OnEnable()
        {
            back.onClick.AddListener(Back);
            close.onClick.AddListener(Exit);
        }

        public void Init(CheckEmailType type, string emailToSend, bool open, bool sendEmailNow,
            EditAccountSections editAccountSections)
        {
            resendEmail.onClick.RemoveAllListeners();
            email = emailToSend;
            emailText.text = email;
            isReady = true;
            this.type = type;
            returnToSection = editAccountSections;

            if (open)
            {
                Open();
            }

            switch (type)
            {
                case CheckEmailType.ResetPassword:
                    resendEmail.onClick.AddListener(SendResetPasswordEmail);
                    if (!sendEmailNow) return;
                    SendResetPasswordEmail();
                    break;
                case CheckEmailType.ChangeEmail:
                    resendEmail.onClick.AddListener(SendChangeEmail);
                    if (!sendEmailNow) return;
                    SendChangeEmail();
                    break;
            }
        }

        private void SendChangeEmail()
        {
            if (isReady)
            {
                BurguerMenuWebClient.RequestChangeEmailAsync(email);
            }
            else
            {
                Debug.LogError(language.GetTextByKey(LangKeys.MAIN_ENTER_VALID_EMAIL));
            }
        }

        void SendResetPasswordEmail()
        {
            if (isReady)
            {
                JesseUtils.SendEmailResetPassword(email);
            }
            else
            {
                Debug.LogError("No valid email provided");
            }
        }

        private void OnDisable()
        {
            email = "";
            emailText.text = "";
            isReady = false;
            type = CheckEmailType.None;
            returnToSection = EditAccountSections.None;

            resendEmail.onClick.RemoveAllListeners();
            back.onClick.RemoveAllListeners();
            close.onClick.RemoveAllListeners();
        }

        void Back()
        {
            if (returnToSection.Equals(EditAccountSections.ChangeEmail))
            {
                editAccountManager.ShowSection(EditAccountSections.ChangeEmail);
            }
            Close();
        }

        void Exit()
        {
            Close();
            editAccountManager.Exit();
        }
    }
}
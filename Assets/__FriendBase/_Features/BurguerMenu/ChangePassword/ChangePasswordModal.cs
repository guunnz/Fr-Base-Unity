using System.Collections.Generic;
using Architecture.Injector.Core;
using AuthFlow;
using AuthFlow.Infrastructure;
using BurguerMenu.Core.Domain;
using BurguerMenu.Infractructure;
using Data;
using LocalizationSystem;
using UI.Auth;
using UI.EditAccount;
using UnityEngine;
using UnityEngine.UI;

namespace BurguerMenu.ChangePassword
{
    public class ChangePasswordModal : MonoBehaviour
    {
        public string WrongPassword = "Wrong password";
        public string PasswordsDoNotMatch = "Passwords do not match";
        public string FieldIsRequired = "Field is required";

        private PasswordValidator passwordValidator;

        private bool canUpdatePassword;

        [Header("Dependencies")]
        [SerializeField]
        private EditAccountManager editAccountManager;


        [Header("Containers")]
        [Space(10)]
        [SerializeField] private GameObject oldPasswordContainer;
        [SerializeField] private GameObject newPasswordContainer;
        [SerializeField] private PanelEmailCheck panelResetPassword;
        [SerializeField] private PanelSuccess panelSuccess;


        [Header("Fields")]
        [Space(10)]
        [SerializeField] private UIComponentInput oldPasswordField;

        [SerializeField] private UIComponentInput newPasswordField;
        [SerializeField] private UIComponentInput confirmNewPasswordField;

        [Header("Buttons")]
        [Space(10)]
        [SerializeField] private Button oldPasswordContinue;
        [SerializeField] private Button oldPasswordForgot;
        [SerializeField] private Button newPasswordContinue;
        [SerializeField] private List<Button> backButtons;
        [SerializeField] private ILanguage language;

        private void Start()
        {
            language = Injection.Get<ILanguage>();
            WrongPassword = language.GetTextByKey(LangKeys.AUTH_WRONG_PASSWORD);
            PasswordsDoNotMatch = language.GetTextByKey(LangKeys.MAIN_PASSWORDS_DONT_MATCH);
            FieldIsRequired = language.GetTextByKey(LangKeys.COMMON_LABEL_FIELD_IS_REQUIRED);
            passwordValidator = new PasswordValidator();
        }

        private void OnEnable()
        {

            foreach (var backButton in backButtons)
            {
                backButton.onClick.AddListener(() => editAccountManager.ShowSection(EditAccountSections.Home));
            }
            oldPasswordContinue.onClick.AddListener(CheckOldPassword);
            oldPasswordForgot.onClick.AddListener(ShowResetPasswordEmail);
            oldPasswordField.ClearPassword();
            newPasswordField.ClearPassword();
            confirmNewPasswordField.ClearPassword();

            ShowCheckCurrentPasswordModal();
            canUpdatePassword = false;

        }

        private void OnDisable()
        {
            RemoveSubscriptions();
        }

        private void OnDestroy()
        {
            RemoveSubscriptions();
        }

        private void RemoveSubscriptions()
        {
            BurguerMenuWebClient.OnPasswordUpdated -= WaitPasswordUpdate;

            oldPasswordContinue.onClick.RemoveAllListeners();
            oldPasswordForgot.onClick.RemoveAllListeners();
            newPasswordContinue.onClick.RemoveAllListeners();

            foreach (var backButton in backButtons)
            {
                backButton.onClick.RemoveAllListeners();
            }
        }

        void ShowCheckCurrentPasswordModal()
        {
            oldPasswordContainer.SetActive(true);
            newPasswordContainer.SetActive(false);
        }

        void ShowNewPasswordModal()
        {
            newPasswordField.ClearPassword();
            confirmNewPasswordField.ClearPassword();
            oldPasswordContainer.SetActive(false);
            newPasswordContainer.SetActive(true);

            canUpdatePassword = true;
        }

        void ShowSuccessModal()
        {
            panelSuccess.Open();
            newPasswordContainer.SetActive(false);
        }

        async void CheckOldPassword()
        {
            var oldPassword = oldPasswordField.GetPassword();

            var isCorrectPassword = BurguerMenuWebClient.CheckPassword(oldPassword);
            await isCorrectPassword;

            if (!isCorrectPassword.Result)
            {
                oldPasswordField.SetError(WrongPassword);
                return;
            }

            ShowNewPasswordModal();
            newPasswordContinue.onClick.AddListener(() => TrySetNewPassword(oldPassword));
        }

        void TrySetNewPassword(string oldPassword)
        {
            if (!canUpdatePassword) return;

            var newPassword = newPasswordField.GetPassword();

            if (IsValidPassword() && DoesPasswordMatch())
            {
                BurguerMenuWebClient.UpdatePassword(oldPassword, newPassword);
                BurguerMenuWebClient.OnPasswordUpdated += WaitPasswordUpdate;
            }
        }

        void WaitPasswordUpdate(bool passwordUpdated)
        {
            canUpdatePassword = true;
            BurguerMenuWebClient.OnPasswordUpdated -= WaitPasswordUpdate;

            if (!passwordUpdated) return;
            ShowSuccessModal();
        }

        private void ShowResetPasswordEmail()
        {
            oldPasswordContainer.SetActive(false);

            var email = Injection.Get<IGameData>().GetUserInformation().Email;

            var checkEmailType = PanelEmailCheck.CheckEmailType.ResetPassword;
            var sectionToReturn = EditAccountSections.None;
            panelResetPassword.Init(checkEmailType, email, true, true, sectionToReturn);

            AuthStateChangeManager.Instance.SubscribeOnAuthStateChange();
        }

        private bool DoesPasswordMatch()
        {
            string newPassword = newPasswordField.GetPassword();
            string passwordValidation = confirmNewPasswordField.GetPassword();

            if (string.IsNullOrEmpty(passwordValidation))
            {
                confirmNewPasswordField.SetError(FieldIsRequired);
                return false;
            }

            if (!passwordValidation.Equals(newPassword))
            {
                confirmNewPasswordField.SetError(PasswordsDoNotMatch);
                return false;
            }

            return true;
        }

        private bool IsValidPassword()
        {
            string passwordInput = newPasswordField.GetPassword();
            (bool passwordIsValid, string validationError) = passwordValidator.Validate(passwordInput);

            if (!passwordIsValid)
            {
                newPasswordField.SetError(validationError);
                return false;
            }

            return true;
        }
    }
}
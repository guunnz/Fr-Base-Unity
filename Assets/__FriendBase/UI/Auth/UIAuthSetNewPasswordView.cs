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
    public class UIAuthSetNewPasswordView : AbstractUIPanel
    {
        [SerializeField] private TextMeshProUGUI TxtTitle;
        [SerializeField] private TextMeshProUGUI TxtButtonContinue;
        private event Action<bool> OnPasswordUpdated;
        public UIComponentInput newPasswordInputField;
        public UIComponentInput confirmNewPasswordInputField;
        private PasswordValidator _passwordValidator;
        private AuthFlowManager _authFlowManager;

        protected override void Start()
        {
            base.Start();
            language.SetTextByKey(TxtTitle, LangKeys.AUTH_SET_NEW_PASSWORD);
            language.SetTextByKey(TxtButtonContinue, LangKeys.COMMON_LABEL_CONTINUE);
            newPasswordInputField.SetLabel(language.GetTextByKey(LangKeys.AUTH_NEW_PASSWORD));
            newPasswordInputField.SetPlaceholder(language.GetTextByKey(LangKeys.AUTH_ENTER_YOUR_NEW_PASSWORD));

            confirmNewPasswordInputField.SetLabel(language.GetTextByKey(LangKeys.AUTH_CONFIRM_NEW_PASSWORD));
            confirmNewPasswordInputField.SetPlaceholder(language.GetTextByKey(LangKeys.AUTH_ENTER_YOUR_NEW_PASSWORD));
            OnPasswordUpdated += WaitPasswordUpdate;
        }

        private void Awake()
        {
            _passwordValidator = new PasswordValidator();
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];

            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }
        }

        private bool IsValidPassword()
        {
            string passwordInput = newPasswordInputField.value;
            (bool passwordIsValid, string validationError) = _passwordValidator.Validate(passwordInput);

            if (!passwordIsValid)
            {
                newPasswordInputField.SetError(validationError);
                return false;
            }

            return true;
        }

        private bool DoesPasswordMatch()
        {
            string newPassword = newPasswordInputField.value;
            string passwordValidation = confirmNewPasswordInputField.value;

            if (!passwordValidation.Equals(newPassword))
            {
                confirmNewPasswordInputField.SetError("Passwords do not match");
                return false;
            }

            return true;
        }

        public void OnSubmit()
        {
            if (IsValidPassword() && DoesPasswordMatch())
            {
                FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

                user.UpdatePasswordAsync(newPasswordInputField.value).ContinueWithOnMainThread(updatePasswordTask =>
                {
                    if (updatePasswordTask.IsCanceled)
                    {
                        if (OnPasswordUpdated != null) OnPasswordUpdated.Invoke(false);
                        Debug.LogError("UpdatePasswordAsync was canceled.");
                        return;
                    }

                    if (updatePasswordTask.IsFaulted)
                    {
                        if (OnPasswordUpdated != null) OnPasswordUpdated.Invoke(false);
                        Debug.LogError("UpdatePasswordAsync encountered an error: " + updatePasswordTask.Exception);
                        return;
                    }

                    if (OnPasswordUpdated != null) OnPasswordUpdated.Invoke(true);
                    Debug.Log("Password updated successfully.");
                });
            }
        }

        void WaitPasswordUpdate(bool passwordUpdated)
        {
            OnPasswordUpdated -= WaitPasswordUpdate;

            if (!passwordUpdated) return;
            //SUCCESS
            _authFlowManager.DoneLegacy();
        }

        public void OnBack()
        {
            _authFlowManager.SetView(_authFlowManager.UILoginView);
        }
    }
}
using System;
using BurguerMenu.Core.Domain;
using UI.Auth;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Architecture.Injector.Core;
using AuthFlow;
using BurguerMenu.Infractructure;
using Data;
using Firebase.DynamicLinks;

namespace UI.EditAccount
{
    public class PanelEmailChange : AbstractUIPanel
    {
        private const string ChangeEmailUrlPath = "/change-email";
        private const string QueryRoot = "?userID=";

        public EditAccountManager editAccountManager;

        [SerializeField] private PanelEmailCheck panelEmailCheck;
        [SerializeField] private PanelEmailCheck panelResetPassword;
        [SerializeField] private PanelSuccess panelSuccess;

        [SerializeField] Button continueButton;

        [SerializeField] Button forgotPasswordButton;

        [SerializeField] private Button newEmailBackButton;

        public UIComponentInput inputField;
        public UIComponentInput passwordField;

        private string newEmailProspect;
        
        private void OnEnable()
        {
            ClearFields();

            newEmailBackButton.onClick.AddListener(() => editAccountManager.ShowSection(EditAccountSections.Home));

            continueButton.onClick.AddListener(OnSubmit);
            forgotPasswordButton.onClick.AddListener(ShowResetPasswordEmail);
        }

        private void OnDisable()
        {
            newEmailBackButton.onClick.RemoveAllListeners();

            continueButton.onClick.RemoveAllListeners();
            forgotPasswordButton.onClick.RemoveAllListeners();

            DynamicLinks.DynamicLinkReceived -= OnDynamicLink;
        }

        public async void OnSubmit()
        {
            bool isCorrectPassword = false;
            bool isValidEmail = false;

            var password = passwordField.GetPassword();

            if (string.IsNullOrEmpty(password))
            {
                passwordField.SetError(language.GetTextByKey(LangKeys.COMMON_LABEL_FIELD_IS_REQUIRED));
            }
            else
            {
                var checkPasswordTask = BurguerMenuWebClient.CheckPassword(password);
                await checkPasswordTask;

                isCorrectPassword = checkPasswordTask.Result;

                if (!isCorrectPassword)
                {
                    passwordField.SetError(language.GetTextByKey(LangKeys.AUTH_WRONG_PASSWORD));
                }
            }


            Regex regexEmail = new Regex(@"^([\w\.\-\+]+)@([\w\-]+)((\.(\w){2,3})+)$");
            string inputText = inputField.value;

            if (string.IsNullOrEmpty(inputText))
            {
                inputField.SetError(language.GetTextByKey(LangKeys.COMMON_LABEL_FIELD_IS_REQUIRED));
            }
            else if (regexEmail.Match(inputText).Success)
            {
                isValidEmail = true;
                inputField.SetError("");
            }
            else
            {
                inputField.SetError(language.GetTextByKey(LangKeys.MAIN_ENTER_VALID_EMAIL));
            }

            if (isCorrectPassword && isValidEmail)
            {
                OnValidEmailSubmit(inputText);
            }
        }

        private async void OnValidEmailSubmit(string newEmail)
        {
            var task = BurguerMenuWebClient.RequestChangeEmailAsync(newEmail);
            await task;

            if (!task.Result) return;
            newEmailProspect = newEmail;

            DynamicLinks.DynamicLinkReceived += OnDynamicLink;

            ShowCheckEmailSection(newEmail);
        }

        private void ShowResetPasswordEmail()
        {
            Close();
            var email = Injection.Get<IGameData>().GetUserInformation().Email;

            var typeOfEmail = PanelEmailCheck.CheckEmailType.ResetPassword;
            var sectionToReturn = EditAccountSections.None;

            panelResetPassword.Init(typeOfEmail, email, true, true, sectionToReturn);

            AuthStateChangeManager.Instance.SubscribeOnAuthStateChange();
        }

        void ShowCheckEmailSection(string email)
        {
            ClearFields();
            Close();

            var checkEmailType = PanelEmailCheck.CheckEmailType.ChangeEmail;
            var sectionToReturn = EditAccountSections.ChangeEmail;
            panelEmailCheck.Init(checkEmailType, email, true, false, sectionToReturn);
        }

        // Display the dynamic link received by the application.
        void OnDynamicLink(object sender, EventArgs args)
        {
            var dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;

            if (dynamicLinkEventArgs != null)
            {
                var query = QueryRoot + Injection.Get<IGameData>().GetUserInformation().UserId;

                bool isValidLink = dynamicLinkEventArgs.ReceivedDynamicLink.Url.AbsolutePath.Equals(ChangeEmailUrlPath);

                bool isSameUser = dynamicLinkEventArgs.ReceivedDynamicLink.Url.Query.Equals(query);

                if (isValidLink && isSameUser)
                {
                    ContinueEmailChanging();
                }
            }
        }

        private async void ContinueEmailChanging()
        {
            var task = BurguerMenuWebClient.ConfirmEmailChangeAsync();
            await task;
           
            if (task.Result)
            {
                Injection.Get<IGameData>().GetUserInformation().SetEmail(newEmailProspect);

                //Todo: remove this line or replace logic when new auth be ready
                Injection.Get<IAuthStateManager>().Email = newEmailProspect;

                ShowSuccessSection();
            }

        }

        private void ShowSuccessSection()
        {
            panelEmailCheck.Close();
            panelSuccess.Open();
        }

        void ClearFields()
        {
            inputField.ClearInput();
            passwordField.ClearPassword();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using AuthFlow.AboutYou.Core.Services;
using Data;
using Firebase.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Auth
{
    public class UIAuthAboutYouView : AbstractUIPanel
    {
        public TextMeshProUGUI TxtTitleStep1;
        public TextMeshProUGUI TxtTitleStep2;
        public TextMeshProUGUI TxtTitleStep3;
        public TextMeshProUGUI TxtTitleSelectGender;
        public TextMeshProUGUI TxtBtnContinue;

        public List<GameObject> stepGameObjects;
        public int stepCurrent = 0;

        public UIComponentInput inputFirstName;
        public UIComponentInput inputLastName;
        public UIComponentInput inputUsername;

        [Header("Birthday")]
        public UIComponentInput inputBirthday;

        public TMP_Dropdown birthdayDay;
        public TMP_Dropdown birthdayMonth;
        public TMP_Dropdown birthdayYear;

        [Header("Gender")]
        public UIComponentInput inputGender;

        public GameObject modalGender;
        public GameObject modalGenderOptionGameObject;
        public GameObject modalGenderOptionsContainer;

        public UIDialogPanel dialogPanel;

        private AuthFlowManager _authFlowManager;
        private List<string> _modalGenderOptions;
        private List<string> _modalGenderOptionsForBackEnd;
        private IAboutYouStateManager _aboutYouState;
        private IAboutYouWebClient _ayWebClient;
        private IGameData gameData;
        private string ErrorRequired;
        private int indexGender;
        IAnalyticsSender analyticsSender;
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
            analyticsSender = Injection.Get<IAnalyticsSender>();
            gameData = Injection.Get<IGameData>();

            _modalGenderOptions = new List<string>();
            _modalGenderOptions.Add(language.GetTextByKey(LangKeys.AUTH_PREFER_NOT_TO_SAY));
            _modalGenderOptions.Add(language.GetTextByKey(LangKeys.AUTH_MALE));
            _modalGenderOptions.Add(language.GetTextByKey(LangKeys.AUTH_FEMALE));
            _modalGenderOptions.Add(language.GetTextByKey(LangKeys.AUTH_OTHER));

            //This are the ids that the backend expect for gender (Only in english)
            _modalGenderOptionsForBackEnd = new List<string>();
            _modalGenderOptionsForBackEnd.Add("Prefer not to say");
            _modalGenderOptionsForBackEnd.Add("Male");
            _modalGenderOptionsForBackEnd.Add("Female");
            _modalGenderOptionsForBackEnd.Add("Other");

            _aboutYouState = Injection.Get<IAboutYouStateManager>();
            _ayWebClient = Injection.Get<IAboutYouWebClient>();

            // Set first option in Gender dropdown
            inputGender.placeholder = _modalGenderOptions[0];
            inputGender.value = _modalGenderOptions[0];
            inputGender.SetLabel(language.GetTextByKey(LangKeys.AUTH_GENDER));
            inputGender.SetPlaceholder(language.GetTextByKey(LangKeys.AUTH_SELECT_GENDER));
            inputGender.SetValues();
        }

        public override void OnOpen()
        {
            language.SetTextByKey(TxtTitleStep1, LangKeys.AUTH_ABOUT_YOU);
            language.SetTextByKey(TxtTitleStep2, LangKeys.AUTH_ABOUT_YOU);
            language.SetTextByKey(TxtTitleStep3, LangKeys.AUTH_CHOOSE_A_USERNAME);

            language.SetTextByKey(TxtBtnContinue, LangKeys.COMMON_LABEL_CONTINUE);

            inputFirstName.SetLabel(language.GetTextByKey(LangKeys.AUTH_FIRST_NAME));
            inputFirstName.SetPlaceholder(language.GetTextByKey(LangKeys.AUTH_ENTER_FIRST_NAME));

            inputLastName.SetLabel(language.GetTextByKey(LangKeys.AUTH_LAST_NAME));
            inputLastName.SetPlaceholder(language.GetTextByKey(LangKeys.AUTH_ENTER_LAST_NAME));

            inputBirthday.SetLabel(language.GetTextByKey(LangKeys.AUTH_BIRTHDAY));

            ErrorRequired = language.GetTextByKey(LangKeys.AUTH_FIELD_REQUIRED);

            language.SetTextByKey(TxtTitleSelectGender, LangKeys.AUTH_SELECT_GENDER);

            inputUsername.SetLabel(language.GetTextByKey(LangKeys.AUTH_USERNAME));
            inputUsername.SetPlaceholder("Enter your Username");

            int currentStep = 0;

            if (_aboutYouState.FirstName != null && _aboutYouState.LastName != null)
            {
                currentStep = 1;
            }

            SetStep(currentStep);
        }

        public void OnBackButton()
        {
            string title = language.GetTextByKey(LangKeys.AUTH_YOU_ARE_ABOUT_TO_EXIT_REGISTRATION);
            string desc = language.GetTextByKey(LangKeys.AUTH_WILL_START_FROM_THIS_SAME_STEP);
            string btnAccept = language.GetTextByKey(LangKeys.COMMON_LABEL_STAY_HERE);
            string btnDiscard = language.GetTextByKey(LangKeys.COMMON_LABEL_EXIT);

            dialogPanel.Open(title, desc, btnAccept, btnDiscard, null, SetLanding);
        }

        public void OnBack()
        {
            _authFlowManager.SetView(_authFlowManager.UIEnterEmailView);
        }

        public void SetLanding()
        {
            _authFlowManager.SetView(_authFlowManager.UILandingView);
        }

        private void SetStep(int newStep)
        {
            foreach (GameObject stepGameObject in stepGameObjects)
            {
                stepGameObject.SetActive(false);
            }

            if (stepGameObjects[newStep] != null)
            {
                stepGameObjects[newStep].SetActive(true);
            }

            stepCurrent = newStep;
        }

        public async void NextStep()
        {
            // Step 1
            if (stepCurrent == 0)
            {
                // Validation: FirstName (required)
                if (!ValidateString(inputFirstName.value))
                {
                    inputFirstName.SetError(ErrorRequired);
                    return;
                }

                // Validation: LastName (required)
                if (!ValidateString(inputLastName.value))
                {
                    inputLastName.SetError(ErrorRequired);
                    return;
                }

                analyticsSender.SendAnalytics(AnalyticsEvent.EnterNameContinue);
            }

            // Step 2
            if (stepCurrent == 1)
            {
                // Validation: Birthday (required)
                if (!ValidateString(inputBirthday.value))
                {
                    inputBirthday.SetError(ErrorRequired);
                    return;
                }

                // TODO: Additional Birthday validation
                // ..

                // Validation: Gender (required)
                if (!ValidateString(inputGender.value))
                {
                    inputGender.SetError(ErrorRequired);
                    return;
                }
                analyticsSender.SendAnalytics(AnalyticsEvent.EnterBirthdayContinue);
            }

            // Step 3
            if (stepCurrent == 2)
            {
                // Validation: Username (required)
                if (!ValidateString(inputUsername.value))
                {
                    inputUsername.SetError(ErrorRequired);
                    return;
                }

                // Validation: Username (valid)
                _authFlowManager.SetLoading(true);
                (bool _, string errorMessage) = await ValidateUsername(inputUsername.value)
                    .ContinueWithOnMainThread(task => task.Result);
                _authFlowManager.SetLoading(false);

                if (errorMessage != null)
                {
                    inputUsername.SetError(errorMessage);
                    return;
                }
                analyticsSender.SendAnalytics(AnalyticsEvent.EnterUsernameContinue);
            }

            // Advance step
            if (stepCurrent < stepGameObjects.Count - 1)
            {
                SetStep(stepCurrent + 1);
                return;
            }

            // Save data
            _authFlowManager.SetLoading(true);
            if (!_aboutYouState.FirstName.HasValue)
            {
                _aboutYouState.FirstName = inputFirstName.value.Trim();
            }

            if (!_aboutYouState.LastName.HasValue)
            {
                _aboutYouState.LastName = inputLastName.value.Trim();
            }

            _aboutYouState.Gender = _modalGenderOptionsForBackEnd[indexGender];
            _aboutYouState.UserName = inputUsername.value.Trim();
            _aboutYouState.BirthDate = DateTime.ParseExact(inputBirthday.value, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            await _ayWebClient.UpdateUserData(
                _aboutYouState.FirstName,
                _aboutYouState.LastName,
                _aboutYouState.BirthDate,
                _aboutYouState.Gender,
                _aboutYouState.UserName
            );
            _authFlowManager.SetLoading(false);
            gameData.GetUserInformation().Do_avatar_customization = true;
            // Try to finish
            analyticsSender.SendAnalytics(AnalyticsEvent.AccountCreateSuccess);
            
            _authFlowManager.Finish();
        }

        public void ToggleModalGender()
        {
            modalGender.SetActive(!modalGender.activeSelf);

            if (!modalGender.activeSelf)
            {
                return;
            }

            foreach (Transform child in modalGenderOptionsContainer.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (string option in _modalGenderOptions)
            {
                GameObject optionGameObject =
                    Instantiate(modalGenderOptionGameObject, modalGenderOptionsContainer.transform);

                // Set option text
                language.SetText(optionGameObject.GetComponentInChildren<TMP_Text>(), option);

                // Set option onClick
                optionGameObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    string optionSelected = optionGameObject.GetComponentInChildren<TMP_Text>().text;

                    indexGender = _modalGenderOptions.IndexOf(optionSelected);

                    inputGender.placeholder = optionSelected;
                    inputGender.value = optionSelected;
                    inputGender.SetValues();
                    inputGender.SetError(null);
                    inputGender.SetInfo(null);
                    ToggleModalGender();
                });
            }

            List<VerticalLayoutGroup> modalGenderVerticalLayoutGroups =
                modalGender.GetComponentsInChildren<VerticalLayoutGroup>().ToList();
            foreach (VerticalLayoutGroup layout in modalGenderVerticalLayoutGroups.Where(layout => layout != null))
            {
                // Update Vertical Layout Group to fix content position                
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout.transform as RectTransform);
            }
        }

        public void SetBirthdayDateValue()
        {
            string day = birthdayDay.options[birthdayDay.value].text.PadLeft(2, '0');
            string month = (birthdayMonth.value + 1).ToString().PadLeft(2, '0');
            string year = birthdayYear.options[birthdayYear.value].text;

            inputBirthday.value = $"{month}/{day}/{year}";
            inputBirthday.SetError(null);
            inputBirthday.SetInfo(null);
        }

        private static bool ValidateString(string text)
        {
            text = text.Trim();
            return !string.IsNullOrEmpty(text) && text.Length >= 3;
        }

        private async Task<(bool, string)> ValidateUsername(string username)
        {
            bool usernameIsAvailable = false;
            const int userNameCharLimit = 3;
            const int userNameCharMaxLimit = 20;
            string errorMessage = null;

            if (string.IsNullOrEmpty(username))
                errorMessage = language.GetTextByKey(LangKeys.AUTH_CHOOSE_A_USERNAME);
            else if (username.Length < userNameCharLimit)
                errorMessage = string.Format(language.GetTextByKey(LangKeys.AUTH_USERNAME_MUST_CONTAIN_AT_LEAST_X_CHARACTERS), userNameCharLimit);
            else if (username.Length > userNameCharMaxLimit)
                errorMessage = string.Format(language.GetTextByKey(LangKeys.AUTH_USERNAME_MUST_CONTAIN_LESS_THAN_X_CHARACTERS), userNameCharMaxLimit);

            if (errorMessage == null)
            {
                try
                {
                    usernameIsAvailable = await _ayWebClient.IsAvailableUserName(username);
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                }

                if (!usernameIsAvailable)
                    errorMessage = $"the name {username} is already taken";
            }

            return (usernameIsAvailable, errorMessage);
        }
    }
}
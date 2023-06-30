using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Firebase.Auth;
using Data.Users;
using static UINauthFlowGenericInputText;
using static NauthFlowEndpoints;
using LocalizationSystem;

public class UINauthFlowEnterNickScreen : AbstractAuthFlowScreen
{
    public override NauthFlowScreenType ScreenType => NauthFlowScreenType.ENTER_NICK;

    public enum REGISTER_MODE { REGISTER, CONVERT_GUEST_USER };

    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private UINauthFlowGenericInputText inputUsername;
    [SerializeField] private UINauthFlowDateInput inputBirthday;
    [SerializeField] private RectTransform containerInputFields;
    [SerializeField] private TextMeshProUGUI txtRegister;
    [SerializeField] private UINauthFlowCheckboxTerms checkboxTerms;
    [SerializeField] private DatePickerModal datePicker;
    [SerializeField] private TextMeshProUGUI txtTerms;
    
    private IProvider providerManager;

    private static REGISTER_MODE registerMode;
    private static ProviderType providerTypeNick;

    protected override void Start()
    {
        base.Start();
        providerManager = Injection.Get<IProvider>();
        inputUsername.OnInputFieldEvent += OnInputFieldEvent;
    }

    public static void SetRegistrationMode(REGISTER_MODE registerModeValue, ProviderType providerTypeValue)
    {
        registerMode = registerModeValue;
        providerTypeNick = providerTypeValue;
    }

    public override void OnOpen()
    {
        checkboxTerms.Open();

        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenEnterUsername);

        language.SetTextByKey(txtTerms, LangKeys.NAUTH_ACCEPT_TERMS);
        language.SetTextByKey(txtTitle, LangKeys.NAUTH_JUST_ONE_MORE_THING);
        string usernameTxt = language.GetTextByKey(LangKeys.NAUTH_USERNAME);
        string yourNameTxt = language.GetTextByKey(LangKeys.NAUTH_YOUR_NAME_IN_THE_GAME);
        string dateBirthTxt = language.GetTextByKey(LangKeys.NAUTH_YOUR_BIRTHDAY);
        string birthFormatTxt = language.GetTextByKey(LangKeys.NAUTH_BIRTHDAY_FORMAT);

        inputUsername.SetUp(UINauthFlowGenericInputText.InputType.Text, usernameTxt, yourNameTxt);
        inputBirthday.SetTitle(dateBirthTxt);
        language.SetTextByKey(txtRegister, LangKeys.NAUTH_REGISTER);
        SetFormDefault();
    }

    public void SetFormDefault()
    {
        containerInputFields.anchoredPosition = Vector2.zero;
    }

    public void MoveFormUp()
    {
        containerInputFields.DOAnchorPosY(175, 0.5f).SetEase(Ease.OutExpo);
    }

    public void MoveFormDown()
    {
        containerInputFields.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutExpo);
    }

    private void OnDestroy()
    {
        inputUsername.OnInputFieldEvent -= OnInputFieldEvent;
    }

    void OnInputFieldEvent(UINauthFlowGenericInputText inputText, EVENT_TYPE eventType)
    {
        if (eventType == EVENT_TYPE.OnSelect)
        {
            MoveFormUp();
        }
        if (eventType == EVENT_TYPE.OnDeselect)
        {
            MoveFormDown();
        }
    }

    public async void OnBtnRegisterDown()
    {
        inputUsername.HideBubbles();

        string birth = inputBirthday.GetText();
        //birth = "2020-02-22T00:00:00.0000000-03:00";
        if (birth == null)
        {
            datePicker.Open();
            return;
        }

        string userName = inputUsername.GetText();
        if (!IsValidNick(userName))
        {
            string limitUsername = language.GetTextByKey(LangKeys.NAUTH_LIMIT_USERNAME);
            inputUsername.ShowBubbleMedium(limitUsername, UINauthFlowBoobleController.IconType.INFO);
            return;
        }

        //Check Terms and conditions
        if (!checkboxTerms.FlagTermsAndConditions)
        {
            checkboxTerms.SetWrong();
            return;
        }
        
        SetLoadingState(true);


        UserNameResult nameResult = await authFlowEndpoint.IsAvailableUserName(userName);
        if (nameResult == UserNameResult.EXISTS)
        {
            string limitUsername = language.GetTextByKey(LangKeys.NAUTH_USERNAME_IN_USE);
            inputUsername.ShowBubbleMedium(limitUsername, UINauthFlowBoobleController.IconType.ALERT);
            SetLoadingState(false);
            return;
        }

        if (nameResult == UserNameResult.ERROR || nameResult == UserNameResult.NOT_ALLOW)
        {
            string invalidUsername = language.GetTextByKey(LangKeys.NAUTH_INVALID_USERNAME);
            inputUsername.ShowBubbleMedium(invalidUsername, UINauthFlowBoobleController.IconType.ALERT);
            SetLoadingState(false);
            return;
        }

        var firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
        string loginToken = await SaveToken(firebaseUser);

        //TODO Check username

        UserInformation userInformation = null;
        if (registerMode == REGISTER_MODE.CONVERT_GUEST_USER)
        {
            analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenEnterUsernameGuestPress);
            userInformation = await authFlowEndpoint.LinkPhoenixGuestUser(firebaseUser.Email, firebaseUser.UserId, loginToken, providerTypeNick, userName, birth);
        }
        else
        {
            analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenEnterUsernameRegisterPress);
            userInformation = await authFlowEndpoint.CreatePhoenixUser(firebaseUser.Email, firebaseUser.UserId, loginToken, providerTypeNick, userName, birth);
        }

        if (userInformation!=null)
        {
            if (registerMode == REGISTER_MODE.CONVERT_GUEST_USER)
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenEnterUsernameGuestSucceed);
            }
            else
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenEnterUsernameRegisterSucceed);
            }

            await authFlowEndpoint.GetInitialAvatarEndpoints();
            SelectPlaceToLeave();
        }
        else
        {
            if (registerMode == REGISTER_MODE.CONVERT_GUEST_USER)
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenEnterUsernameGuestError);
            }
            else
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenEnterUsernameRegisterError);
            }
            errorModal.Open();
            SetLoadingState(false);
        }
    }

    public void OnBtnBackDown()
    {
        if (registerMode == REGISTER_MODE.CONVERT_GUEST_USER)
        {
            authFlowManager.GoScreen(NauthFlowScreenType.LINK_PROVIDER);
        }
        else
        {
            authFlowManager.GoScreen(NauthFlowScreenType.REGISTER);
        }
    }
}

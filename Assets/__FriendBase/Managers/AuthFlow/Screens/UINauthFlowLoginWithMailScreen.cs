using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DebugConsole;
using Data.Users;
using UniRx;
using static UINauthFlowGenericInputText;

public class UINauthFlowLoginWithMailScreen : AbstractAuthFlowScreen
{
    public override NauthFlowScreenType ScreenType => NauthFlowScreenType.LOGIN_WITH_MAIL;

    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private UINauthFlowGenericInputText inputMail;
    [SerializeField] private UINauthFlowGenericInputText inputPassword;
    [SerializeField] private RectTransform containerInputFields;
    [SerializeField] private TextMeshProUGUI txtLogin;
    private IProvider providerManager;

    protected override void Start()
    {
        base.Start();
        providerManager = Injection.Get<IProvider>();

        inputPassword.OnBubblePressed += OnBubblePressed;
        inputMail.OnInputFieldEvent += OnInputFieldEvent;
    }

    public override void OnOpen()
    {
        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenLoginWithMail);

        language.SetTextByKey(txtTitle, LangKeys.NAUTH_LOGIN_EMAIL);
        language.SetTextByKey(txtLogin, LangKeys.NAUTH_LOGIN);

        string email = language.GetTextByKey(LangKeys.NAUTH_EMAIL);
        string enterEmail = language.GetTextByKey(LangKeys.NAUTH_ENTER_MAIL);
        string password = language.GetTextByKey(LangKeys.NAUTH_PASSWORD);
        string enterPassword = language.GetTextByKey(LangKeys.NAUTH_ENTER_PASSWORD);
        string forgotPassword = language.GetTextByKey(LangKeys.NAUTH_FORGOT_PASSWORD);

        inputMail.SetUp(UINauthFlowGenericInputText.InputType.Mail, email, enterEmail);
        inputPassword.SetUp(UINauthFlowGenericInputText.InputType.Password, password, enterPassword);
        inputPassword.ShowBubbleSmall(forgotPassword, UINauthFlowBoobleController.IconType.QUESTION);
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
        inputPassword.OnBubblePressed -= OnBubblePressed;
        inputMail.OnInputFieldEvent -= OnInputFieldEvent;
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

    void OnBubblePressed(UINauthFlowGenericInputText inputText)
    {
        authFlowManager.GoScreen(NauthFlowScreenType.PASSWORD_RECOVERY);
    }

    public void OnBtnLoginDown()
    {
        inputMail.HideBubbles();

        string forgotPassword = language.GetTextByKey(LangKeys.NAUTH_FORGOT_PASSWORD);
        inputPassword.ShowBubbleSmall(forgotPassword, UINauthFlowBoobleController.IconType.QUESTION);
        inputPassword.SetBoxFieldDefault();

        string mail = inputMail.GetText();
        string password = inputPassword.GetText();

        if (!IsValidEmail(mail))
        {
            string incorrectMail = language.GetTextByKey(LangKeys.NAUTH_EMAIL_IS_INCORRECT);
            inputMail.ShowBubbleMedium(incorrectMail, UINauthFlowBoobleController.IconType.ALERT);
            return;
        }

        if (string.IsNullOrEmpty(inputPassword.GetText()))
        {
            inputPassword.ShowBubbleSmall(forgotPassword, UINauthFlowBoobleController.IconType.ALERT);
            inputPassword.SetBoxFieldOrangeStroke();
            return;
        }

        SetLoadingState(true);

        providerManager.GetProvider(ProviderType.EMAIL).LoginWithMailAndPassword(mail, password, OnAuthFlowLoginWithMailCallback);
    }

    public void OnBtnBackDown()
    {
        authFlowManager.GoScreen(NauthFlowScreenType.LOGIN);
    }

    protected virtual async void OnAuthFlowLoginWithMailCallback(GenericAuthFlowResult authFlowResult)
    {

        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowLoginWithMailCallback, State:" + authFlowResult.State + " ErrorType:" + authFlowResult.ErrorType);

        if (authFlowResult.State == GenericAuthFlowResult.STATE.SUCCEED)
        {
            SetLoadingState(true);

            await SaveToken(authFlowResult.UserFirebase);

            UserInformation userInformation = await Injection.Get<IAvatarEndpoints>().GetUserInformation();

            if (userInformation == null)
            {
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowLoginWithMailCallback, userInformation NULL");

                string mailDoesNotExist = language.GetTextByKey(LangKeys.NAUTH_EMAIL_DOES_NOT_EXIST);
                inputMail.ShowBubbleMedium(mailDoesNotExist, UINauthFlowBoobleController.IconType.ALERT);
            }
            else if (string.IsNullOrEmpty(userInformation.UserName))
            {
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowLoginWithProviderCallback, userName NULL");
                UINauthFlowEnterNickScreen.SetRegistrationMode(UINauthFlowEnterNickScreen.REGISTER_MODE.REGISTER, ProviderType.EMAIL);
                authFlowManager.GoScreen(NauthFlowScreenType.ENTER_NICK);
            }
            else
            {
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowLoginWithMailCallback, has userInformation");
                await authFlowEndpoint.GetInitialAvatarEndpoints();
                SelectPlaceToLeave();
            }
        }
        else
        {
            SetLoadingState(false);

            if (authFlowResult.ErrorType == GenericAuthFlowResult.ERROR_TYPE.ACCOUNT_NOT_FOUND)
            {
                string mailDoesNotExist = language.GetTextByKey(LangKeys.NAUTH_EMAIL_DOES_NOT_EXIST);
                inputMail.ShowBubbleMedium(mailDoesNotExist, UINauthFlowBoobleController.IconType.ALERT);
            }
            else if (authFlowResult.ErrorType == GenericAuthFlowResult.ERROR_TYPE.INCORRECT_PASSWORD)
            {
                string forgotPassword = language.GetTextByKey(LangKeys.NAUTH_FORGOT_PASSWORD);
                inputPassword.ShowBubbleSmall(forgotPassword, UINauthFlowBoobleController.IconType.ALERT);
                inputPassword.SetBoxFieldOrangeStroke();
            }
            else if (authFlowResult.ErrorType == GenericAuthFlowResult.ERROR_TYPE.DIFFERENT_PROVIDER)
            {
                string mailRegisteredWithOtherProvider = language.GetTextByKey(LangKeys.NAUTH_MAIL_REGISTERED_WITH_OTHER_PROVIDER);
                inputPassword.ShowBubbleSmall(mailRegisteredWithOtherProvider, UINauthFlowBoobleController.IconType.ALERT);
                inputPassword.SetBoxFieldOrangeStroke();
            }
        }
    }
}

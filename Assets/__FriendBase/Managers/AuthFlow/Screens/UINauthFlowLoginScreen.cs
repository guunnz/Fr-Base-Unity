using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Architecture.Injector.Core;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Architecture.ViewManager;
using MainMenu;
using Firebase.Auth;
using AuthFlow.EndAuth.Repo;
using AuthFlow;
using Data.Users;
using UniRx;
using DebugConsole;
using System;
using static UINauthFlowModalGenericError;

public class UINauthFlowLoginScreen : AbstractAuthFlowScreen
{
    private static bool isFirstTime = true;

    public override NauthFlowScreenType ScreenType => NauthFlowScreenType.LOGIN;

    [SerializeField] private List<UINauthFlowGenericLoginButton> listLoginButtons;
    [SerializeField] private TextMeshProUGUI txtBtnRegister;
    [SerializeField] private TextMeshProUGUI txtBtnPlayAsGuest;
    [SerializeField] private TextMeshProUGUI txtLabelOr;
    [SerializeField] private UINauthFlowModalCreateAnAccount createAccountModal;

    private IProvider providerManager;

    protected override void Start()
    {
        base.Start();
        providerManager = Injection.Get<IProvider>();

        language.SetTextByKey(txtBtnRegister, LangKeys.NAUTH_REGISTER);
        language.SetTextByKey(txtBtnPlayAsGuest, LangKeys.NAUTH_PLAY_AS_GUEST);
        language.SetTextByKey(txtLabelOr, LangKeys.NAUTH_OR);

        foreach (UINauthFlowGenericLoginButton loginButton in listLoginButtons)
        {
            loginButton.OnPressButton += OnPressButton;
        }
        createAccountModal.OnGoToCreateAccount += OnGoToCreateAccount;
    }

    public static void ResetFirstTime()
    {
        isFirstTime = true;
    }

    private void OnDestroy()
    {
        foreach (UINauthFlowGenericLoginButton loginButton in listLoginButtons)
        {
            loginButton.OnPressButton -= OnPressButton;
        }
        createAccountModal.OnGoToCreateAccount -= OnGoToCreateAccount;
    }

    public override void Open(NauthFlowManager authFlowManager)
    {
        //Do not call base.Open() because we do not want to hide loading yet
        this.authFlowManager = authFlowManager;
        base.Open();
    }

    public override void OnOpen()
    {
        leftPanel.SetActive(true);
        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenLogin);
        if (isFirstTime)
        {
            isFirstTime = false;
            OnAutomaticLogin();
        }
        else
        {
            SetLoadingState(false);
        }
    }

    void OnGoToCreateAccount()
    {
        OnBtnRegister();
    }

    void OnPressButton(ProviderType providerType)
    {
        this.providerType = providerType;
        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenLoginPress);
        switch (providerType)
        {
            case ProviderType.GOOGLE:
                SetLoadingState(true);
                providerManager.GetProvider(ProviderType.GOOGLE).LoginSSO(OnAuthFlowLoginWithProviderCallback, AbstractProvider.OPERATION_TYPE.Login);
                break;
            case ProviderType.APPLE:
                SetLoadingState(true);
                providerManager.GetProvider(ProviderType.APPLE).LoginSSO(OnAuthFlowLoginWithProviderCallback, AbstractProvider.OPERATION_TYPE.Login);
                break;
            case ProviderType.FACEBOOK:
                SetLoadingState(true);
                providerManager.GetProvider(ProviderType.FACEBOOK).LoginSSO(OnAuthFlowLoginWithProviderCallback, AbstractProvider.OPERATION_TYPE.Login);
                break;
            case ProviderType.EMAIL:
                authFlowManager.GoScreen(NauthFlowScreenType.LOGIN_WITH_MAIL);
                break;
        }
    }

    public void OnBtnRegister()
    {
        authFlowManager.GoScreen(NauthFlowScreenType.REGISTER);
    }
      
    public void OnBtnLoginAsGuest()
    {
        SetLoadingState(true);
        providerManager.GetProvider(ProviderType.GUEST).LoginSSO(OnAuthFlowCallbackGuest, AbstractProvider.OPERATION_TYPE.Login);
    }

    protected virtual async void OnAuthFlowLoginWithProviderCallback(GenericAuthFlowResult authFlowResult)
    {

        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowLoginWithProviderCallback, State:" + authFlowResult.State + " ErrorType:" + authFlowResult.ErrorType);

        if (authFlowResult.State == GenericAuthFlowResult.STATE.SUCCEED)
        {
            SetLoadingState(true);

            await SaveToken(authFlowResult.UserFirebase);

            UserInformation userInformation = await Injection.Get<IAvatarEndpoints>().GetUserInformation();

            if (userInformation == null)
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenLoginNoUser);
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowLoginWithProviderCallback, userInformation NULL");
                SetLoadingState(false);
                createAccountModal.Open();
                Logout();
            }
            else if (string.IsNullOrEmpty(userInformation.UserName))
            {
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowLoginWithProviderCallback, userName NULL");
                authFlowManager.GoScreen(NauthFlowScreenType.ENTER_NICK);
            }
            else
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenLoginSucceed);
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowLoginWithProviderCallback, has userInformation");
                await authFlowEndpoint.GetInitialAvatarEndpoints();
                SelectPlaceToLeave();
            }
        }
        else
        {
            analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenLoginError);
            SetLoadingState(false);

            if (authFlowResult.ErrorType == GenericAuthFlowResult.ERROR_TYPE.DIFFERENT_PROVIDER)
            {
                errorModal.Open(MessageType.DIFFERENT_PROVIDER);
            }
            else
            {
                errorModal.Open();
            }
        }
    }

    void Logout()
    {
        providerManager.GetProvider(providerType).LogOut();
        try
        {
            FirebaseAuth.DefaultInstance.SignOut();
        }
        catch (Exception e)
        {
            // Is not necessary to do anything here
        }
    }

    public async void OnAutomaticLogin()
    {
        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenLoginAutomatic);
        SetLoadingState(true);
        string loginToken = await authFlowEndpoint.IsUserLoggedIn();

        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAutomaticLogin, loginToken:" + loginToken);

        if (loginToken!=null)
        {
            UserInformation userInformation = await Injection.Get<IAvatarEndpoints>().GetUserInformation();

            if (userInformation == null || string.IsNullOrEmpty(userInformation.UserName))
            {
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAutomaticLogin, userInformation NULL");

                analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenLoginCancel);
                SetLoadingState(false);
            }
            else
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenLoginSucceed);
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAutomaticLogin, has userInformation");
                await authFlowEndpoint.GetInitialAvatarEndpoints();
                SelectPlaceToLeave();
            }
        }
        else
        {
            SetLoadingState(false);
        }
    }
}

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
using Data.Users;
using System;
using UniRx;
using static UINauthFlowModalGenericError;
using DebugConsole;

public class UINauthFlowRegisterScreen : AbstractAuthFlowScreen
{
    public override NauthFlowScreenType ScreenType => NauthFlowScreenType.REGISTER;

    [SerializeField] private List<UINauthFlowGenericLoginButton> listLoginButtons;
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtBtnLogin;
    [SerializeField] private TextMeshProUGUI txtBtnPlayAsGuest;
    [SerializeField] private TextMeshProUGUI txtLabelOr;

    private IProvider providerManager;

    protected override void Start()
    {
        providerManager = Injection.Get<IProvider>();

        base.Start();

        foreach (UINauthFlowGenericLoginButton loginButton in listLoginButtons)
        {
            loginButton.OnPressButton += OnPressButton;
        }
    }

    public override void OnOpen()
    {
        language.SetTextByKey(txtTitle, LangKeys.NAUTH_CREATE_ACCOUNT);
        language.SetTextByKey(txtBtnLogin, LangKeys.NAUTH_LOGIN);
        language.SetTextByKey(txtBtnPlayAsGuest, LangKeys.NAUTH_PLAY_AS_GUEST);
        language.SetTextByKey(txtLabelOr, LangKeys.NAUTH_OR);

        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenRegister);
        leftPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        foreach (UINauthFlowGenericLoginButton loginButton in listLoginButtons)
        {
            loginButton.OnPressButton -= OnPressButton;
        }
    }

    void OnPressButton(ProviderType providerType)
    {
        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenRegisterPress);
        this.providerType = providerType;
        switch (providerType)
        {
            case ProviderType.GOOGLE:
                SetLoadingState(true);
                providerManager.GetProvider(ProviderType.GOOGLE).LoginSSO(OnAuthFlowRegisterCallback, AbstractProvider.OPERATION_TYPE.Login);
                break;
            case ProviderType.APPLE:
                SetLoadingState(true);
                providerManager.GetProvider(ProviderType.APPLE).LoginSSO(OnAuthFlowRegisterCallback, AbstractProvider.OPERATION_TYPE.Login);
                break;
            case ProviderType.FACEBOOK:
                SetLoadingState(true);
                providerManager.GetProvider(ProviderType.FACEBOOK).LoginSSO(OnAuthFlowRegisterCallback, AbstractProvider.OPERATION_TYPE.Login);
                break;
        }
    }

    public void OnBtnLoginClick()
    {
        authFlowManager.GoScreen(NauthFlowScreenType.LOGIN);
    }

    public void OnBtnLoginAsGuest()
    {
        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowGuest);
        SetLoadingState(true);
        providerManager.GetProvider(ProviderType.GUEST).LoginSSO(OnAuthFlowCallbackGuest, AbstractProvider.OPERATION_TYPE.Login);
    }

    protected override void OnAuthFlowGenericCallbackError(GenericAuthFlowResult authFlowResult)
    {
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

    protected virtual async void OnAuthFlowRegisterCallback(GenericAuthFlowResult authFlowResult)
    {
        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowRegisterCallback, State:" + authFlowResult.State + " ErrorType:" + authFlowResult.ErrorType);

        if (authFlowResult.State == GenericAuthFlowResult.STATE.SUCCEED)
        {
            await SaveToken(authFlowResult.UserFirebase);

            UserInformation userInformation = await Injection.Get<IAvatarEndpoints>().GetUserInformation();

            if (userInformation == null || string.IsNullOrEmpty(userInformation.UserName))
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenRegisterSucceed);

                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowRegisterCallback, userInformation NULL");

                UINauthFlowEnterNickScreen.SetRegistrationMode(UINauthFlowEnterNickScreen.REGISTER_MODE.REGISTER, providerType);
                authFlowManager.GoScreen(NauthFlowScreenType.ENTER_NICK);
            }
            else
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenRegisterAccountAlreadyCreated);

                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowRegisterCallback, has userInformation");

                providerManager.GetProvider(providerType).LogOut();

                SetLoadingState(false);
                errorModal.Open(MessageType.DIFFERENT_PROVIDER);
            }
        }
        else
        {
            OnAuthFlowGenericCallbackError(authFlowResult);
        }
    }
}

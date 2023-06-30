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
using DebugConsole;
using static UINauthFlowModalGenericError;
public class UINauthFlowLinkProviderScreen : AbstractAuthFlowScreen
{
    public override NauthFlowScreenType ScreenType => NauthFlowScreenType.LINK_PROVIDER;

    [SerializeField] private List<UINauthFlowGenericLoginButton> listLoginButtons;
    [SerializeField] private TextMeshProUGUI txtTitle;

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
        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenLink);
        language.SetTextByKey(txtTitle, LangKeys.NAUTH_REGISTER);
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
        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenLinkPress);
        this.providerType = providerType;
        switch (providerType)
        {
            case ProviderType.GOOGLE:
                SetLoadingState(true);
                providerManager.GetProvider(ProviderType.GOOGLE).LoginSSO(OnAuthFlowLinkProviderCallback, AbstractProvider.OPERATION_TYPE.Link);
                break;
            case ProviderType.APPLE:
                SetLoadingState(true);
                providerManager.GetProvider(ProviderType.APPLE).LoginSSO(OnAuthFlowLinkProviderCallback, AbstractProvider.OPERATION_TYPE.Link);
                break;
            case ProviderType.FACEBOOK:
                SetLoadingState(true);
                providerManager.GetProvider(ProviderType.FACEBOOK).LoginSSO(OnAuthFlowLinkProviderCallback, AbstractProvider.OPERATION_TYPE.Link);
                break;
        }
    }
    
    protected void OnAuthFlowLinkProviderCallback(GenericAuthFlowResult authFlowResult)
    {
        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowLinkProviderCallback, State:" + authFlowResult.State + " ErrorType:" + authFlowResult.ErrorType);

        if (authFlowResult.State == GenericAuthFlowResult.STATE.SUCCEED)
        {gameData.SetWasGuest(true);
            analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenLinkSucceed);
            UINauthFlowEnterNickScreen.SetRegistrationMode(UINauthFlowEnterNickScreen.REGISTER_MODE.CONVERT_GUEST_USER, providerType);
            authFlowManager.GoScreen(NauthFlowScreenType.ENTER_NICK);
        }
        else
        {
            analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenLinkError);
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

    public void OnBtnClose()
    {
        StartCoroutine(GoToLastRoom());
    }

    IEnumerator GoToLastRoom()
    {
        SceneManager.LoadScene(GameScenes.RoomScene, LoadSceneMode.Additive);
        yield return new WaitForEndOfFrame();
        SceneManager.UnloadSceneAsync(GameScenes.NewAuthFlow);
    }
}

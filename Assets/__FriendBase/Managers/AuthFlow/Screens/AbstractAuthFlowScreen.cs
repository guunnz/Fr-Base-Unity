using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using UnityEngine;

using UnityEngine.SceneManagement;
using Architecture.ViewManager;
using MainMenu;
using AuthFlow.EndAuth.Repo;
using System.Text.RegularExpressions;
using Firebase.Auth;
using System.Threading.Tasks;
using Data.Users;
using Data;
using DebugConsole;
using UniRx;

public abstract class AbstractAuthFlowScreen : AbstractUIPanel
{
    public abstract NauthFlowScreenType ScreenType { get; }

    [SerializeField] protected UINauthFlowModalGenericError errorModal;
    [SerializeField] protected GameObject leftPanel;

    protected NauthFlowManager authFlowManager;
    protected IAuthFlowEndpoint authFlowEndpoint;
    protected IGameData gameData = Injection.Get<IGameData>();
    protected ILoading loading;
    protected IDebugConsole debugConsole;
    protected IAvatarEndpoints avatarEndpoints;
    protected IAnalyticsSender analyticsSender;

    protected ProviderType providerType;

    protected override void Start()
    {
        base.Start();
        loading = Injection.Get<ILoading>();
        authFlowEndpoint = Injection.Get<IAuthFlowEndpoint>();
        avatarEndpoints = Injection.Get<IAvatarEndpoints>();
        debugConsole = Injection.Get<IDebugConsole>();
        analyticsSender = Injection.Get<IAnalyticsSender>();
    }

    public virtual void Open(NauthFlowManager authFlowManager)
    {
        this.authFlowManager = authFlowManager;
        Injection.Get<ILoading>().Unload();
        leftPanel.SetActive(false);
        base.Open();
    }

    protected virtual async void OnAuthFlowGenericCallback(GenericAuthFlowResult authFlowResult)
    {

        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowGenericCallback, State:" + authFlowResult.State + " ErrorType:" + authFlowResult.ErrorType);

        if (authFlowResult.State == GenericAuthFlowResult.STATE.SUCCEED)
        {
            await SaveToken(authFlowResult.UserFirebase);

            UserInformation userInformation = await Injection.Get<IAvatarEndpoints>().GetUserInformation();

            if (userInformation == null || string.IsNullOrEmpty( userInformation.UserName) )
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowLoginFirebaseSucceed);

                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowGenericCallback, userInformation NULL");

                UINauthFlowEnterNickScreen.SetRegistrationMode(UINauthFlowEnterNickScreen.REGISTER_MODE.REGISTER, providerType);
                authFlowManager.GoScreen(NauthFlowScreenType.ENTER_NICK);
            }
            else 
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowLoginFirebaseError);

                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowGenericCallback, has userInformation");
                await authFlowEndpoint.GetInitialAvatarEndpoints();
                SelectPlaceToLeave();
            }
        }
        else
        {
            OnAuthFlowGenericCallbackError(authFlowResult);
        }
    }

    protected virtual void OnAuthFlowGenericCallbackError(GenericAuthFlowResult authFlowResult)
    {

    }

    protected virtual async void OnAuthFlowCallbackGuest(GenericAuthFlowResult authFlowResult)
    {
        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "OnAuthFlowCallbackGuest State:" + authFlowResult.State);

        if (authFlowResult.State == GenericAuthFlowResult.STATE.SUCCEED)
        {
            analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowGuestSucceed);

            string loginToken = await SaveToken(authFlowResult.UserFirebase);

            //Create Phoenix User
            UserInformation userInformation = await authFlowEndpoint.CreatePhoenixGuestUser(authFlowResult.UserFirebase.UserId, loginToken);
            if (userInformation!=null)
            {
                await authFlowEndpoint.GetInitialAvatarEndpoints();
                SelectPlaceToLeave();
            }
            else
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowGuestError);

                debugConsole.ErrorLog("AbstractAuthFlowScreen:OnAuthFlowCallbackGuest", "Error", "");
                SetLoadingState(false);
                errorModal.Open();
            }
        }
        else
        {
            analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowGuestError);

            debugConsole.ErrorLog("AbstractAuthFlowScreen:OnAuthFlowCallbackGuest", "Error", "");
            SetLoadingState(false);
            errorModal.Open();
        }
    }

    protected virtual async Task<string> SaveToken(FirebaseUser firebaseUser)
    {
        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "SaveToken, UserId:" + firebaseUser.UserId);

        string loginToken = await firebaseUser.TokenAsync(true);
        ILocalUserInfo userInfo = Injection.Get<ILocalUserInfo>();
        userInfo["firebase-login-token"] = loginToken;
        return loginToken;
    }

    protected virtual void SelectPlaceToLeave()
    {
        if (gameData.GetUserInformation().Do_avatar_customization)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "SelectPlaceToLeave, GoToAvatarCustomization");
            GoToAvatarCustomization();
        }
        else
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "SelectPlaceToLeave, GoToPublicRoom");
            GoToPublicRoom();
        }
    }

    protected virtual void GoToPublicRoom()
    {
        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowGoToPublicRoom);
        SceneManager.UnloadSceneAsync(GameScenes.NewAuthFlow);
        Injection.Get<IViewManager>().Show<MainMenuView>();
    }

    protected virtual void GoToAvatarCustomization()
    {
        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowGoToAvatarCustomization);
        SceneManager.UnloadSceneAsync(GameScenes.NewAuthFlow);
        SceneManager.LoadSceneAsync(GameScenes.AvatarCustomization, LoadSceneMode.Additive);
    }

    public static bool IsValidEmail(string mail)
    {
        Regex regexEmail = new Regex(@"^([\w\.\-\+]+)@([\w\-]+)((\.(\w){2,3})+)$");
        if (string.IsNullOrEmpty(mail))
        {
            return false;
        }
        if (regexEmail.Match(mail).Success)
        {
            return true;
        }
        return false;
    }

    public static bool IsValidNick(string nick)
    {
        if (string.IsNullOrEmpty(nick))
        {
            return false;
        }
        int amountChars = nick.Length;
        if (amountChars < 3)
        {
            return false;
        }
        if (amountChars > 20)
        {
            return false;
        }
        return true;
    }

    public void SetLoadingState(bool isLoading)
    {
        if (isLoading)
        {
            loading.Load();
        }
        else
        {
            loading.Unload();
        }
    }
}

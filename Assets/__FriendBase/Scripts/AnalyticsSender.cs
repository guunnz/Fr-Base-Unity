using Architecture.Injector.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum AnalyticsEvent
{
    EmailLoginButton,
    FacebookLoginButton,
    EnterEmailLogin,
    CreateAccountContinue,
    LoginAccountContinue,
    ResendEmailVerification,
    EmailVerified,
    AcceptTerms,
    EnterNameContinue,
    EnterBirthdayContinue,
    EnterUsernameContinue,
    AccountCreateSuccess,
    CreatedAvatarFirstTime,
    OnboardingFinish,
    ForgotPassword,
    OpensChatIcon,
    OpensMoreGemsModal,
    OpensEventsTab,
    JoinRoom,
    EnterAvatarCustomization,
    ChangedInRoomSpent,
    EnterMinigames,
    OpenEditHouse,
    RetryErrorScreen,
    CloseDuringLoading,
    MinimizeDuringLoading,
    LoginWithFacebook,
    RegistrationWithFacebook,
    LoginWithApple,
    RegistrationWithApple,
    RacingMinigame,
    SpacejumpMinigame,

    SSO_GoogleSignIn,
    SSO_GoogleSignInCancelled,
    SSO_GoogleSignInError,
    SSO_GoogleSignInSucceed,
    SSO_GoogleLinkProvider,
    SSO_GoogleLinkProviderError,
    SSO_GoogleLinkProviderSucceed,
    SSO_GoogleFirebaseLogin,
    SSO_GoogleFirebaseLoginError,
    SSO_GoogleFirebaseLoginSucceed,

    SSO_FacebookSignIn,
    SSO_FacebookSignInCancelled,
    SSO_FacebookSignInError,
    SSO_FacebookSignInSucceed,
    SSO_FacebookLinkProvider,
    SSO_FacebookLinkProviderError,
    SSO_FacebookLinkProviderSucceed,
    SSO_FacebookFirebaseLogin,
    SSO_FacebookFirebaseLoginError,
    SSO_FacebookFirebaseLoginSucceed,

    SSO_AppleNotSupport,
    SSO_AppleSignIn,
    SSO_AppleSignInCancelled,
    SSO_AppleSignInError,
    SSO_AppleSignInCredentials,
    SSO_AppleSignInSucceed,
    SSO_AppleLinkProvider,
    SSO_AppleLinkProviderError,
    SSO_AppleLinkProviderCancelled,
    SSO_AppleLinkProviderSucceed,
    SSO_AppleFirebaseLogin,
    SSO_AppleFirebaseLoginCancelled,
    SSO_AppleFirebaseLoginError,
    SSO_AppleFirebaseLoginSucceed,
    SSO_AppleWebSignIn,
    SSO_AppleWebSignInError,
    SSO_AppleWebSignInSucceed,

    SSO_GuestFirebaseLogin,
    SSO_GuestFirebaseLoginCancelled,
    SSO_GuestFirebaseLoginError,
    SSO_GuestFirebaseLoginSucceed,

    SSO_MailLogin,
    SSO_MailLoginInvalidMail,
    SSO_MailLoginIncorrectPassword,
    SSO_MailLoginMailWithDifferentProvider,
    SSO_MailLoginSucceed,

    AuthFlowInitFirebase,
    AuthFlowInitFirebaseError,
    AuthFlowInitFirebaseSuccedd,

    AuthFlowScreenLogin,
    AuthFlowScreenLoginWithMail,
    AuthFlowScreenRegister,
    AuthFlowScreenLink,
    AuthFlowScreenEnterUsername,
    AuthFlowScreenRecoverPassword,

    AuthFlowScreenLoginPress,
    AuthFlowScreenLoginNoUser,
    AuthFlowScreenLoginSucceed,
    AuthFlowScreenLoginError,
    AuthFlowScreenLoginAutomatic,
    AuthFlowScreenLoginCancel,

    AuthFlowScreenRegisterSucceed,
    AuthFlowScreenRegisterAccountAlreadyCreated,

    AuthFlowScreenRegisterPress,
    AuthFlowGuest,
    AuthFlowGuestSucceed,
    AuthFlowGuestError,
    AuthFlowLoginFirebaseSucceed,
    AuthFlowLoginFirebaseError,
    AuthFlowGoToPublicRoom,
    AuthFlowGoToAvatarCustomization,
    AuthFlowScreenEnterUsernameGuestPress,
    AuthFlowScreenEnterUsernameGuestSucceed,
    AuthFlowScreenEnterUsernameGuestError,
    AuthFlowScreenEnterUsernameRegisterPress,
    AuthFlowScreenEnterUsernameRegisterSucceed,
    AuthFlowScreenEnterUsernameRegisterError,
    AuthFlowScreenLinkPress,
    AuthFlowScreenLinkSucceed,
    AuthFlowScreenLinkError,
    AuthFlowScreenRecoverPasswordPress,
    AuthFlowScreenRecoverPasswordSucceed,
    AuthFlowScreenRecoverPasswordError
}

public class AnalyticsSender : MonoBehaviour, IAnalyticsSender
{
    private List<IAnalytics> analyticsList = new List<IAnalytics>();

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (Injection.Get<IAnalyticsSender>() != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Injection.Register<IAnalyticsSender>(this);
    }

    void OnDestroy()
    {
        Injection.Clear<IAnalyticsSender>();
    }

    public void AddAnalytics(IAnalytics analytics)
    {
        analyticsList.Add(analytics);
    }

    public void SendAnalytics(AnalyticsEvent analyticsEvent, string extraText = "")
    {
        if (EnvironmentData.FORCE_DEVELOP)
            return;
        
        analyticsList.ForEach(x => x.SendAnalytics(analyticsEvent, extraText));
    }
}
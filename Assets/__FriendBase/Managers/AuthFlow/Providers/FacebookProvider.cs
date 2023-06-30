using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using AuthFlow.EndAuth.Repo;
using DebugConsole;
using Facebook.Unity;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

public class FacebookProvider : AbstractProvider
{
    public override ProviderType TypeProvider => ProviderType.FACEBOOK;

    private IDebugConsole debugConsole;
    private IAnalyticsSender analyticsSender;
    private OPERATION_TYPE operationType;

    public FacebookProvider()
    {
        debugConsole = Injection.Get<IDebugConsole>();
        analyticsSender = Injection.Get<IAnalyticsSender>();

        // Initialize Facebook and toggle the Facebook Login button
        if (!FB.IsInitialized)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Facebook Try Initialize");

            FB.Init(() =>
            {
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                    if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Facebook Initialize Succeed");
                }
                else
                {
                    if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Facebook Initialize Failed");
                }
            });
        }
        else
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Facebook already initialized => ActivateApp");
            // Enable Facebook features
            // More info: https://developers.facebook.com/docs/unity/reference/current/FB.ActivateApp/
            FB.ActivateApp();
        }
    }

    public override void LogOut()
    {
        try
        {
            FB.LogOut();
        }
        catch (Exception e)
        {
            // Is not necessary to do anything here
        }
    }

    public override void LoginWithMailAndPassword(string email, string password, Action<GenericAuthFlowResult> authFlowCallback)
    {
    }

    public override void LoginSSO(Action<GenericAuthFlowResult> authFlowCallback, OPERATION_TYPE operationType)
    {
        this.operationType = operationType;
        if (authFlowCallback==null)
        {
            return;
        }
        try
        {
            analyticsSender.SendAnalytics(AnalyticsEvent.SSO_FacebookSignIn, operationType.ToString());

            var permissions = new List<string>()
            {
                "email",
                "public_profile",
            };

            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Facebook Try LoginSSO");

            FB.LogInWithReadPermissions(permissions, result =>
            {
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Facebook Try LoginSSO 00");

                if (!FB.IsLoggedIn)
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.SSO_FacebookSignInError, operationType.ToString());

                    if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Facebook LoginSSO user cancel login");
                    authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.CANCELED));
                    return;
                }

                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Facebook Try LoginSSO 01");

                // Get user email from Facebook
                FB.API("/me?fields=email", HttpMethod.GET, async (resultGraph) =>
                {
                    string emailValue = resultGraph.ResultDictionary["email"].ToString();

                    if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Facebook email: " + emailValue);

                    analyticsSender.SendAnalytics(AnalyticsEvent.SSO_FacebookSignInSucceed, operationType.ToString());

                    //Todo check if it is signed with mail and password or has another provider
                    //authFlowCallback(new LoginWithFacebookResult(AbstractAuthFlowResult.STATE.DIFFERENT_PROVIDER));

                    if (operationType == OPERATION_TYPE.Link)
                    {
                        LinkProvider(authFlowCallback);
                    }
                    else 
                    {
                        SignInWithFirebase(authFlowCallback);
                    }
                });
            });
        }
        catch (Exception ex)
        {
            authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.NONE));
        }
    }

    private void LinkProvider(Action<GenericAuthFlowResult> authFlowCallback)
    {
        try
        {
            // AccessToken class will have session details
            Facebook.Unity.AccessToken fullAccessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            string accessToken = fullAccessToken.TokenString;

            Credential firebaseCredential = FacebookAuthProvider.GetCredential(accessToken);

            analyticsSender.SendAnalytics(AnalyticsEvent.SSO_FacebookLinkProvider, operationType.ToString());

            FirebaseAuth.DefaultInstance.CurrentUser.LinkWithCredentialAsync(firebaseCredential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.SSO_FacebookLinkProviderError, operationType.ToString());

                    if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "LinkProviderWithFacebook Error: " + task.Exception.InnerExceptions[0].Message);

                    FirebaseException fbEx = task.Exception.InnerExceptions[0].GetBaseException() as FirebaseException;
                    LogOut();

                    if (fbEx != null && fbEx.ErrorCode == 10)
                    {
                        authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.DIFFERENT_PROVIDER));
                        return;
                    }

                    authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.ERROR_SIGNING_WITH_CREDENTIAL));
                    return;
                }

                analyticsSender.SendAnalytics(AnalyticsEvent.SSO_FacebookLinkProviderSucceed, operationType.ToString());

                FirebaseUser firebaseUser = task.Result;
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "LinkProviderWithFacebook Succeed, DisplayName: " + firebaseUser.DisplayName + " UserId: " + firebaseUser.UserId);

                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.SUCCEED, firebaseUser));
            });
        }
        catch (Exception e)
        {
            authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.ERROR_SIGNING_WITH_CREDENTIAL));
        }
    }

    private void SignInWithFirebase(Action<GenericAuthFlowResult> authFlowCallback)
    {
        // AccessToken class will have session details
        Facebook.Unity.AccessToken fullAccessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
        string accessToken = fullAccessToken.TokenString;

        Credential firebaseCredential = FacebookAuthProvider.GetCredential(accessToken);

        analyticsSender.SendAnalytics(AnalyticsEvent.SSO_FacebookFirebaseLogin, operationType.ToString());

        FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(firebaseCredential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.SSO_FacebookFirebaseLoginError, operationType.ToString());

                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "SignInUserWithFacebook Error: " + task.Exception.InnerExceptions[0].Message);

                FirebaseException fbEx = task.Exception.InnerExceptions[0].GetBaseException() as FirebaseException;

                if (fbEx != null && fbEx.ErrorCode ==6)
                {
                    authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.DIFFERENT_PROVIDER));
                    return;
                }
                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.ERROR_SIGNING_WITH_CREDENTIAL));
                return;
            }

            analyticsSender.SendAnalytics(AnalyticsEvent.SSO_FacebookFirebaseLoginSucceed, operationType.ToString());

            FirebaseUser firebaseUser = task.Result;
            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "SignInUserWithFacebook Succeed, DisplayName: " + firebaseUser.DisplayName + " UserId: " + firebaseUser.UserId);

            authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.SUCCEED, firebaseUser));
        });
    }
}

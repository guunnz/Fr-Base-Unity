using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Google;
using UnityEngine;

using Firebase.Extensions;
using DebugConsole;
using Architecture.Injector.Core;
using AuthFlow;
using Firebase;

public class GoogleProvider : AbstractProvider
{
    public override ProviderType TypeProvider => ProviderType.GOOGLE;

    private IDebugConsole debugConsole;
    private IAnalyticsSender analyticsSender;
    private OPERATION_TYPE operationType;

    public GoogleProvider()
    {
        debugConsole = Injection.Get<IDebugConsole>();
        analyticsSender = Injection.Get<IAnalyticsSender>();
    }

    public string GetGoogleSingInKey()
    {
        //return "534255011920-ua42u5laffnu2itme87pvm8s6ghe7v0d.apps.googleusercontent.com";

        
        if (EnvironmentData.IsProduction())
        {
            return "534255011920-mj139mcv411k1cg24cafk78lb19mhrmg.apps.googleusercontent.com";
        }
        else
        {
            return "1045385706868-0l0p89r0o4c8ro6kqnt43e2ali9loo1n.apps.googleusercontent.com";
        }
        
    }

    public override void LogOut()
    {
        try
        {
            GoogleSignIn.DefaultInstance.SignOut();
        }
        catch (Exception e)
        {
            // Is not necessary to do anything here
        }
    }

    public override void LoginWithMailAndPassword(string email, string password, Action<GenericAuthFlowResult> authFlowCallback)
    {
    }

    public async override void LoginSSO(Action<GenericAuthFlowResult> authFlowCallback, OPERATION_TYPE operationType)
    {
        this.operationType = operationType;

        if (authFlowCallback == null)
        {
            return;
        }

        analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GoogleSignIn, operationType.ToString());

        string webClientId = GetGoogleSingInKey();

        GoogleSignInConfiguration configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();
        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();

        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Google Try LoginSSO");

        await signIn.ContinueWithOnMainThread(async task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                if (task.IsCanceled)
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GoogleSignInCancelled, operationType.ToString());
                }
                else
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GoogleSignInError, operationType.ToString());
                }

                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Sign in with Google failed " + task.Exception);

                FirebaseException fbEx = task.Exception.InnerExceptions[0].GetBaseException() as FirebaseException;
                if (fbEx != null && fbEx.ErrorCode == 6)
                {
                    authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.DIFFERENT_PROVIDER));
                    return;
                }

                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.CANCELED));
                return;
            }

            string userMail = task.Result.Email;
            string userName = task.Result.DisplayName;

            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Google email: " + userMail + " userName:" + userName);

            string idToken = task.Result.IdToken;

            analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GoogleSignInSucceed, operationType.ToString());

            if (operationType == OPERATION_TYPE.Link)
            {
                LinkProvider(authFlowCallback, idToken);
            }
            else
            {
                SignInWithFirebase(authFlowCallback, idToken);
            }
        });
    }

    private void LinkProvider(Action<GenericAuthFlowResult> authFlowCallback, string idToken)
    {
        try
        {
            // AccessToken class will have session details
            Credential firebaseCredential = GoogleAuthProvider.GetCredential(idToken, null);

            analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GoogleLinkProvider, operationType.ToString());

            FirebaseAuth.DefaultInstance.CurrentUser.LinkWithCredentialAsync(firebaseCredential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GoogleLinkProviderError, operationType.ToString());

                    if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "LinkProviderWithGoogle Error: " + task.Exception.InnerExceptions[0].Message);

                    LogOut();

                    FirebaseException fbEx = task.Exception.InnerExceptions[0].GetBaseException() as FirebaseException;
                    if (fbEx != null && fbEx.ErrorCode == 10)
                    {
                        authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.DIFFERENT_PROVIDER));
                        return;
                    }

                    authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.ERROR_SIGNING_WITH_CREDENTIAL));
                    return;
                }

                analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GoogleLinkProviderSucceed, operationType.ToString());

                FirebaseUser firebaseUser = task.Result;
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "LinkProviderWithGoogle Succeed, DisplayName: " + firebaseUser.DisplayName + " UserId: " + firebaseUser.UserId);

                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.SUCCEED, firebaseUser));
            });
        }
        catch (Exception e)
        {
            authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.ERROR_SIGNING_WITH_CREDENTIAL));
        }
    }

    private void SignInWithFirebase(Action<GenericAuthFlowResult> authFlowCallback, string idToken)
    {
        // AccessToken class will have session details
        Credential firebaseCredential = GoogleAuthProvider.GetCredential(idToken, null);

        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Google SignInWithCredentialAsync");

        analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GoogleFirebaseLogin, operationType.ToString());

        FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(firebaseCredential).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GoogleFirebaseLoginError, operationType.ToString());

                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "SignInUserWithGoogle Error: " + task.Exception.InnerExceptions[0].Message);
                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.ERROR_SIGNING_WITH_CREDENTIAL));
                return;
            }

            analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GoogleFirebaseLoginSucceed, operationType.ToString());

            FirebaseUser firebaseUser = task.Result;
            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "SignInUserWithGoogle Succeed, DisplayName: " + firebaseUser.DisplayName + " UserId: " + firebaseUser.UserId);

            authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.SUCCEED, firebaseUser));
        });
    }
}

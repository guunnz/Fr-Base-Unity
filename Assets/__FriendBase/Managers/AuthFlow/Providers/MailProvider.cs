using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using UnityEngine;
using Firebase.Auth;
using UniRx;
using DebugConsole;
using AuthFlow.EndAuth.Repo;
using Firebase;

public class MailProvider : AbstractProvider
{
    public override ProviderType TypeProvider => ProviderType.EMAIL;
    private IDebugConsole debugConsole;
    private IAnalyticsSender analyticsSender;

    public MailProvider()
    {
        analyticsSender = Injection.Get<IAnalyticsSender>();
        debugConsole = Injection.Get<IDebugConsole>();
    }

    public override async void LoginWithMailAndPassword(string email, string password, Action<GenericAuthFlowResult> authFlowCallback)
    {
        analyticsSender.SendAnalytics(AnalyticsEvent.SSO_MailLogin);

        try
        {
            FirebaseUser firebaseUser = await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);

            if (firebaseUser == null)
            {
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "firebaseUser==null - Login with mail and password:");

                analyticsSender.SendAnalytics(AnalyticsEvent.SSO_MailLoginIncorrectPassword);

                if (authFlowCallback != null)
                {
                    authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.INCORRECT_PASSWORD));
                }
                return;
            }

            analyticsSender.SendAnalytics(AnalyticsEvent.SSO_MailLoginSucceed);

            string loginToken = await firebaseUser.TokenAsync(true);
            ILocalUserInfo userInfo = Injection.Get<ILocalUserInfo>();
            userInfo["firebase-login-token"] = loginToken;

            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Login with mail and password, name:" + firebaseUser.DisplayName + " UserId:" + firebaseUser.UserId);

            if (authFlowCallback != null)
            {
                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.SUCCEED, firebaseUser));
            }
        }
        catch (Exception e)
        {
            FirebaseException fbEx = e.GetBaseException() as FirebaseException;

            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Error - Login with mail and password:");
           
            if (authFlowCallback != null)
            {
                if (fbEx.ErrorCode == 14)
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.SSO_MailLoginInvalidMail);
                    authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.ACCOUNT_NOT_FOUND));
                }
                else if (fbEx.ErrorCode == 12)
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.SSO_MailLoginIncorrectPassword);
                    authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.INCORRECT_PASSWORD));
                }
                else if (fbEx.ErrorCode == 6)
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.SSO_MailLoginMailWithDifferentProvider);
                    authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.DIFFERENT_PROVIDER));
                }
                else
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.SSO_MailLoginInvalidMail);
                    authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.ACCOUNT_NOT_FOUND));
                }
            }
        }
    }

    public override void LoginSSO(Action<GenericAuthFlowResult> authFlowCallback, OPERATION_TYPE operationType)
    {
    }

    public override void LogOut()
    {
        try
        {
            FirebaseAuth.DefaultInstance.SignOut();
        }
        catch (Exception e)
        {
            // Is not necessary to do anything here
        }
    }
}

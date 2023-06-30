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
using Firebase.Extensions;

public class GuestProvider : AbstractProvider
{
    public override ProviderType TypeProvider => ProviderType.GUEST;
    private IDebugConsole debugConsole;
    private IAnalyticsSender analyticsSender;

    public GuestProvider()
    {
        analyticsSender = Injection.Get<IAnalyticsSender>();
        debugConsole = Injection.Get<IDebugConsole>();
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

    public override void LoginWithMailAndPassword(string email, string password, Action<GenericAuthFlowResult> authFlowCallback)
    {

    }

    public override void LoginSSO(Action<GenericAuthFlowResult> authFlowCallback, OPERATION_TYPE operationType)
    {
        analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GuestFirebaseLogin);

        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GuestFirebaseLoginCancelled);

                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "GuestProvider:LoginSSO 01: " + task.Exception.InnerExceptions[0].Message);
                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.ERROR_SIGNING_WITH_CREDENTIAL));
                return;
            }
            if (task.IsFaulted)
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GuestFirebaseLoginError);

                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "GuestProvider:LoginSSO 02: " + task.Exception.InnerExceptions[0].Message);
                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.ERROR_SIGNING_WITH_CREDENTIAL));
                return;
            }

            if (task.IsCompleted)
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.SSO_GuestFirebaseLoginSucceed);

                FirebaseUser firebaseUser = task.Result;

                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "GuestProvider Succeed, DisplayName: " + firebaseUser.DisplayName + " UserId: " + firebaseUser.UserId);

                //Deliver Trama
                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.SUCCEED, firebaseUser));
            }
        });
    }
}

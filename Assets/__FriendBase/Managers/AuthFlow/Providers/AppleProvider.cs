using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DebugConsole;
using Architecture.Injector.Core;
using System;
using AuthFlow.AppleLogin.Infrastructure;
using AppleAuth;
using AppleAuth.Native;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using AppleAuth.Extensions;
using System.Text;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase;

public class AppleProvider : AbstractProvider
{
    public override ProviderType TypeProvider => ProviderType.APPLE;

    private IDebugConsole debugConsole;
    private IAnalyticsSender analyticsSender;

    private NonceGenerator nonceGenerator;
    private IAppleAuthManager appleAuthManager;

    private Credential firebaseCredential;
    private OPERATION_TYPE operationType;

    public AppleProvider()
    {
        debugConsole = Injection.Get<IDebugConsole>();
        analyticsSender = Injection.Get<IAnalyticsSender>();

        nonceGenerator = new NonceGenerator();
    }

    public void AppleWebSignIn(Action<GenericAuthFlowResult> authFlowCallback, OPERATION_TYPE operationType)
    {
        Firebase.Auth.FederatedOAuthProviderData providerData = new Firebase.Auth.FederatedOAuthProviderData();
        providerData.ProviderId = AuthProvidersFirebase.APPLE;
        providerData.Scopes = new List<string>();
        Firebase.Auth.FederatedOAuthProvider provider = new Firebase.Auth.FederatedOAuthProvider();
        provider.SetProviderData(providerData);

        analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleWebSignIn, operationType.ToString());

        FirebaseAuth.DefaultInstance.SignInWithProviderAsync(provider).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleWebSignInError, operationType.ToString());
                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.CANCELED));
                return;
            }

            Firebase.Auth.SignInResult signInResult = task.Result;
            Firebase.Auth.FirebaseUser firebaseUser = signInResult.User;
            //Debug.LogFormat("User signed in successfully: {0} ({1})", firebaseUser.DisplayName, firebaseUser.UserId);
            analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleWebSignInSucceed, operationType.ToString());
            if (operationType == OPERATION_TYPE.Link)
            {
                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.CANCELED));
            }
            else
            {
                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.SUCCEED, firebaseUser));
            }
        });
    }

    public override void LogOut()
    {
        try
        {
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
        analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleSignIn, operationType.ToString());

        if (!AppleAuthManager.IsCurrentPlatformSupported)
        {
            analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleNotSupport, operationType.ToString());
            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Apple is not supported in the platform");
            AppleWebSignIn(authFlowCallback, operationType);
            return;
        }

        // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
        PayloadDeserializer deserializer = new PayloadDeserializer();
        // Creates an Apple Authentication manager with the deserializer
        appleAuthManager = new AppleAuthManager(deserializer);
        AppleAuthLoginArgs loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
        appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleSignInSucceed, operationType.ToString());

                    if (operationType == OPERATION_TYPE.Link)
                    {
                        LinkProvider(authFlowCallback, credential);
                    }
                    else
                    {
                        SignInWithFirebase(authFlowCallback, credential);
                    }
                },
                error =>
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleSignInCancelled, operationType.ToString());

                    AuthorizationErrorCode authorizationErrorCode = error.GetAuthorizationErrorCode();
                    if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Sign in with Apple failed " + authorizationErrorCode + " " + error);
                    authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.CANCELED));
                }
            );
    }

    private void LinkProvider(Action<GenericAuthFlowResult> authFlowCallback, ICredential credential)
    {
        try
        {
            string nonce = nonceGenerator.GenerateAppleNonce();
            IAppleIDCredential appleIdCredential = credential as IAppleIDCredential;

            if (appleIdCredential != null)
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleLinkProvider, operationType.ToString());

                string userId = appleIdCredential.User;

                string identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                string authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);

                firebaseCredential = OAuthProvider.GetCredential(
                    AuthProvidersFirebase.APPLE,
                    identityToken,
                    nonce,
                    authorizationCode
                );

                FirebaseAuth.DefaultInstance.CurrentUser.LinkWithCredentialAsync(firebaseCredential).ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        if (task.IsCanceled)
                        {
                            analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleLinkProviderCancelled, operationType.ToString());
                        }
                        else
                        {
                            analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleLinkProviderError, operationType.ToString());
                        }

                        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "LinkProviderWithApple Error: " + task.Exception.InnerExceptions[0].Message);

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

                    analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleLinkProviderSucceed, operationType.ToString());

                    FirebaseUser firebaseUser = task.Result;
                    if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "LinkProviderWithApple Succeed, DisplayName: " + firebaseUser.DisplayName + " UserId: " + firebaseUser.UserId);

                    authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.SUCCEED, firebaseUser));
                });
            }
            else
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleLinkProviderError, operationType.ToString());
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Link provider with Apple on Firebase ");
                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.CANCELED));
            }
        }
        catch (Exception e)
        {
            authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.ERROR_SIGNING_WITH_CREDENTIAL));
        }
    }

    private void SignInWithFirebase(Action<GenericAuthFlowResult> authFlowCallback, ICredential credential)
    {
        string nonce = nonceGenerator.GenerateAppleNonce();
        IAppleIDCredential appleIdCredential = credential as IAppleIDCredential;

        analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleFirebaseLogin, operationType.ToString());

        if (appleIdCredential != null)
        {
            string userId = appleIdCredential.User;

            string identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
            string authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);

            firebaseCredential = OAuthProvider.GetCredential(
                AuthProvidersFirebase.APPLE,
                identityToken,
                nonce,
                authorizationCode
            );
            FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(firebaseCredential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    if (task.IsCanceled)
                    {
                        analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleFirebaseLoginCancelled, operationType.ToString());
                    }
                    else
                    {
                        analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleFirebaseLoginError, operationType.ToString());
                    }

                    if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "SignInUserWithApple Error: " + task.Exception.InnerExceptions[0].Message);
                    authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.ERROR_SIGNING_WITH_CREDENTIAL));
                    return;
                }

                analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleFirebaseLoginSucceed, operationType.ToString());

                FirebaseUser firebaseUser = task.Result;
                if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "SignInUserWithApple Succeed, DisplayName: " + firebaseUser.DisplayName + " UserId: " + firebaseUser.UserId);

                authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.SUCCEED, firebaseUser));
            });
        }
        else
        {
            analyticsSender.SendAnalytics(AnalyticsEvent.SSO_AppleFirebaseLoginError, operationType.ToString());

            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Sign in with Apple on Firebase ");
            authFlowCallback(new GenericAuthFlowResult(GenericAuthFlowResult.STATE.ERROR, GenericAuthFlowResult.ERROR_TYPE.CANCELED));
        }
    }
}

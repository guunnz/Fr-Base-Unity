using System;
using System.Text;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using AuthFlow.AppleLogin.Core.Services;
using Functional.Maybe;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace AuthFlow.AppleLogin.Infrastructure
{
    /// <summary>
    /// Documentation: 
    /// <value>https://github.com/lupidan/apple-signin-unity#implement-sign-in-with-apple</value>
    /// </summary>
    [UsedImplicitly]
    public class AppleLoginService : IAppleLoginService
    {
        readonly IAppleLoginRepository repo;


        readonly CompositeDisposable disposables = new CompositeDisposable();
        AppleAuthManager appleAuthManager;

        public AppleLoginService(IAppleLoginRepository repo)
        {
            this.repo = repo;
        }


        public void Init()
        {
            if (appleAuthManager == null && AppleAuthManager.IsCurrentPlatformSupported)
            {
                disposables.Clear(); // to prevent double init and race-conds

                // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
                var deserializer = new PayloadDeserializer();
                // Creates an Apple Authentication manager with the deserializer
                appleAuthManager = new AppleAuthManager(deserializer);
                Observable
                    .EveryUpdate()
                    .Subscribe(_ => appleAuthManager.Update())
                    .AddTo(disposables);
            }
        }


        /*
         * You will receive user email and name ONLY THE FIRST TIME THE USER LOGINS.
         * Any further login attempts will have a NULL Email and FullName, unless you revoke the credentials
         * https://github.com/lupidan/apple-signin-unity#how-can-i-logout-does-the-plugin-provide-any-logout-option
         */
        public IObservable<AppleData> SignInAndRequestUserData()
        {
            return Observable.Create<AppleData>(obs =>
            {
                var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
                appleAuthManager.LoginWithAppleId(loginArgs, HandleCredentials(obs), HandleError(obs));
                return Disposable.Empty;
            });
        }

        Action<ICredential> HandleCredentials(IObserver<AppleData> obs) =>
            credential => ReadCredential(credential).EmitOnly(obs, true);

        Action<IAppleError> HandleError(IObserver<AppleData> obs) => error =>
        {
            var exception = BuildException(error);
            obs.OnError(exception);
        };

        Exception BuildException(IAppleError error)
        {
            var errorCode = error.GetAuthorizationErrorCode();
            var description = error.LocalizedDescription;
            var failureReason = error.LocalizedFailureReason;
            var exceptionMessage = $"Apple Error {description} {failureReason} Authorization Error Code : {errorCode}";
            var exception = new Exception(exceptionMessage);
            return exception;
        }

        Maybe<AppleData> ReadCredential(ICredential credential)
        {
            // Obtained credential, cast it to IAppleIDCredential
            if (!(credential is IAppleIDCredential appleIdCredential))
            {
                Debug.LogError(
                    $"Fail to cast {credential} as {nameof(IAppleIDCredential)} cause it is {credential?.GetType()?.Name}");
                return Maybe<AppleData>.Nothing;
            }
            
            // Apple User ID
            // You should save the user ID somewhere in the device
            var userId = appleIdCredential.User;
            repo.AppleID = userId;


            // Email (Received ONLY in the first login)
            var email = appleIdCredential.Email;

            // Full name (Received ONLY in the first login)
            var fullName = appleIdCredential.FullName;

            // Identity token
            var identityToken = Encoding.UTF8.GetString(
                appleIdCredential.IdentityToken, 0,
                appleIdCredential.IdentityToken.Length);

            // Authorization code
            var authorizationCode = Encoding.UTF8.GetString(
                appleIdCredential.AuthorizationCode,
                0,
                appleIdCredential.AuthorizationCode.Length);

            // And now you have all the information to create/login a user in your system
            return new AppleData(userId, email, fullName, identityToken, authorizationCode);
        }
    }
}
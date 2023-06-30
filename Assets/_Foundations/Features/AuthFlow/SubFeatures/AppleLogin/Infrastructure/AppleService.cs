using System;
using System.Text;
using Architecture.Injector.Core;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.Actions;
using AuthFlow.AppleLogin.Core.Services;
using AuthFlow.Domain;
using Firebase.Auth;
using Functional.Maybe;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using User.Core;
using User.Core.Domain;

namespace AuthFlow.AppleLogin.Infrastructure
{
    [UsedImplicitly]
    public class AppleService : IAppleService
    {
        readonly IAboutYouWebClient aboutYouClient;
        readonly MarkAsLoggedIn markAsLoggedIn;
        readonly SaveUserInfo saveUserInfo;
        readonly IAuthWebService authWebService;
        readonly IAppleLoginService loginService;
        readonly IAppleNonceGenerator nonceGenerator;

        public AppleService(IAboutYouWebClient aboutYouClient,
            MarkAsLoggedIn markAsLoggedIn, SaveUserInfo saveUserInfo, IAuthWebService authWebService,
            IAppleLoginService loginService, IAppleNonceGenerator nonceGenerator)
        {
            this.aboutYouClient = aboutYouClient;
            this.markAsLoggedIn = markAsLoggedIn;
            this.saveUserInfo = saveUserInfo;
            this.authWebService = authWebService;
            this.loginService = loginService;
            this.nonceGenerator = nonceGenerator;
        }

        IObservable<Unit> SaveInfo(UserAuthInfo user)
        {
            //todo save user info
            var displayName = user.displayName;
            var firstName = displayName;
            var lastName = Maybe<string>.Nothing;

            // ReSharper disable once InvertIf
            if (displayName.Contains(" "))
            {
                var split = displayName.Split(' ');
                firstName = split[0];
                lastName = split[1];
            }

            return saveUserInfo
                .Execute(ToUserInfo(user))
                .Do(() => {
                    markAsLoggedIn.Execute();
                    // TODO find if it's a login or a sign up
                    Injection.Get<IAnalyticsService>().SendLoginEvent();
                })
                .SelectMany(aboutYouClient.UpdateUserData(firstName, lastName));
        }

        IObservable<FirebaseUser> LoginWithApple()
        {
            return loginService.SignInAndRequestUserData().SelectMany(appleData =>
            {
                var auth = FirebaseAuth.DefaultInstance;
                var nonce = nonceGenerator.GenerateAppleNonce();
                var credential =
                    OAuthProvider.GetCredential(AuthProvidersFirebase.APPLE, appleData.IdentityToken, nonce, appleData.AuthorizationCode);
                return auth.SignInWithCredentialAsync(credential).ToObservable();
            });
        }

        public IObservable<Unit> Login()
        {
            return LoginWithApple()
                .SelectMany(ToDomain)
                .SelectMany(SaveInfo);
        }

        IObservable<string> GetToken()
        {
            //This may be needed for android, at the moment breaks iOs.
            return Observable.Empty<Unit>()
                .SelectMany(_ => FirebaseAuth.DefaultInstance.CurrentUser.TokenAsync(true).ToObservable());
        }

        static UserInfo ToUserInfo(UserAuthInfo auth)
        {
            var userInfo = new UserInfo(
                auth.displayName,
                auth.email,
                auth.photoUrl,
                auth.providerId,
                auth.userId,
                auth.token
            );
            Debug.Log("create user info " + userInfo);
            return userInfo;
        }

        static IObservable<UserAuthInfo> ToDomain(FirebaseUser firebaseUserInfo)
        {
            Debug.Log(firebaseUserInfo);
            return firebaseUserInfo.TokenAsync(true).ToObservable().Select(token =>
                new UserAuthInfo(
                    firebaseUserInfo.DisplayName,
                    firebaseUserInfo.Email,
                    firebaseUserInfo.PhotoUrl,
                    firebaseUserInfo.ProviderId,
                    firebaseUserInfo.UserId,
                    token));
        }
    }
}
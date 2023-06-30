using System;
using System.Collections.Generic;
using Architecture.Injector.Core;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.Actions;
using AuthFlow.Domain;
using AuthFlow.FacebookLogin.Core.Services;
using Firebase.Auth;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Facebook.Unity;
using Functional.Maybe;
using User.Core;
using User.Core.Domain;

namespace AuthFlow.FacebookLogin.Infrastructure
{
    [UsedImplicitly]
    public class FacebookService : IFacebookService
    {
        readonly SaveUserInfo saveUserInfo;
        readonly MarkAsLoggedIn markAsLoggedIn;
        readonly IAboutYouWebClient aboutYouWebClient;
        readonly IAuthWebService authWebService;

        public FacebookService(IAboutYouWebClient aboutYouClient,
            MarkAsLoggedIn markAsLoggedIn, SaveUserInfo saveUserInfo, IAuthWebService authWebService)
        {
            this.aboutYouWebClient = aboutYouClient;
            this.markAsLoggedIn = markAsLoggedIn;
            this.saveUserInfo = saveUserInfo;
            this.authWebService = authWebService;
        }

        public IObservable<Unit> Init()
        {
// #if UNITY_STANDALONE
//             return Observable.ReturnUnit();
// #else
            return Observable.Create<Unit>(obs =>
            {
                if (!FB.IsInitialized)
                {
                    FB.Init(() =>
                    {
                        if (FB.IsInitialized)
                        {
                            obs.OnNext(Unit.Default);
                        }
                        else
                        {
                            obs.OnError(new Exception("Something went wrong to Initialize the Facebook SDK"));
                        }
                    }, ONHideUnity);
                }
                else
                {
                    FB.ActivateApp();
                    obs.OnNext(Unit.Default);
                }

                return Disposable.Empty;
            }).First();
// #endif
        }

        private static void ONHideUnity(bool isGameScreenShown)
        {
            Time.timeScale = !isGameScreenShown ? 0 : 1;
        }

        public IObservable<Unit> Login()
        {
            return LoginWithFacebook()
                .SelectMany(ToDomain)
                .SelectMany(SaveInfo);
        }

        private static IObservable<FirebaseUser> LoginWithFacebook()
        {
            return Observable.Create<FirebaseUser>(obs =>
            {
                Debug.Log("Logging in with facebook");
                var permissions = new List<string>() {"email", "user_birthday", "user_friends", "public_profile"};
                FB.LogInWithReadPermissions(permissions, (result =>
                {
                    if (FB.IsLoggedIn)
                    {
                        // AccessToken class will have session details
                        var fullAccessToken = AccessToken.CurrentAccessToken;
                        var accessToken = fullAccessToken.TokenString;

                        var auth = FirebaseAuth.DefaultInstance;
                        var credential = FacebookAuthProvider.GetCredential(accessToken);

                        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                        {
                            if (task.IsCanceled)
                            {
                                obs.OnError(new Exception("SignInWithCredentialAsync was canceled."));
                                return;
                            }

                            if (task.IsFaulted && task.Exception != null)
                            {
                                obs.OnError(task.Exception);
                                return;
                            }

                            var newUser = task.Result;
                            Debug.LogFormat("User signed in successfully: {0} ({1})",
                                newUser.DisplayName, newUser.UserId);

                            var user = FirebaseAuth.DefaultInstance.CurrentUser;
                            obs.OnNext(user);
                        });
                    }
                    else
                    {
                        Debug.Log("User cancelled login");
                    }
                }));
                return Disposable.Empty;
            });
        }

        private static UserInfo ToUserInfo(UserAuthInfo auth)
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

        private static IObservable<UserAuthInfo> ToDomain(FirebaseUser firebaseUserInfo)
        {
            return firebaseUserInfo.TokenAsync(true).ToObservable().Select(token =>
                new UserAuthInfo(
                    firebaseUserInfo.DisplayName,
                    firebaseUserInfo.Email,
                    firebaseUserInfo.PhotoUrl,
                    firebaseUserInfo.ProviderId,
                    firebaseUserInfo.UserId,
                    token));
        }

        private IObservable<Unit> SaveInfo(UserAuthInfo user)
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
                .SelectMany(aboutYouWebClient.UpdateUserData(firstName, lastName));
        }
    }
}
using Architecture.ViewManager;
using Architecture.Injector.Core;
using AuthFlow.Actions;
using AuthFlow.AppleLogin.Core.Actions;
using AuthFlow.FacebookLogin.Core.Actions;
using JetBrains.Annotations;
using Firebase.Auth;
using UniRx;
using UnityEngine;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.EndAuth.Repo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Data;
using Data.Catalog;
using Data.Users;
using System.Linq;
using System.Threading.Tasks;

namespace AuthFlow.Landing
{
    [UsedImplicitly]
    public class LandingPagePresenter
    {
        readonly ILandingPageScreen screen;
        readonly IViewManager viewManager;
        readonly LoginWithFacebook loginWithFacebook;
        readonly LoginUsingAppleId loginWithApple;

        readonly CompositeDisposable disposables = new CompositeDisposable();

        readonly CompositeDisposable buttonsDisposables = new CompositeDisposable();

        public LandingPagePresenter(ILandingPageScreen screen, IViewManager viewManager,
            CheckCookieCredentials checkCredentials, LoginWithFacebook loginWithFacebook,
            LoginUsingAppleId loginWithApple,
            IAboutYouWebClient ayWebClient
        )
        {
            this.screen = screen;
            this.viewManager = viewManager;
            this.loginWithFacebook = loginWithFacebook;
            this.loginWithApple = loginWithApple;

            this.screen.OnShowView.Subscribe(DoPresent).AddTo(disposables);

            this.screen.OnHideView.Subscribe(_ => Hide()).AddTo(disposables);
            this.screen.OnDisposeView.Subscribe(_ => CleanUp()).AddTo(disposables);
            buttonsDisposables.AddTo(disposables);
        }

        void SetInteractionsEnabled(bool state)
        {
            screen.loadingSpinner.SetActive(!state);
            if (screen.emailLoginButton)
                screen.emailLoginButton.gameObject.SetActive(state);
            if (screen.googleLoginButton)
                screen.googleLoginButton.gameObject.SetActive(state);
            if (screen.facebookLoginButton)
                screen.facebookLoginButton.gameObject.SetActive(state);
            if (screen.appleLoginButton)
                screen.appleLoginButton.gameObject.SetActive(state);
        }

        void DoPresent()
        {
            Present().ToObservable().ObserveOnMainThread().Subscribe().AddTo(disposables);
        }

        async Task Present()
        {
            SetInteractionsEnabled(false);

            var loginToken = await JesseUtils.IsUserLoggedIn();
            if (string.IsNullOrEmpty(loginToken))
            {
                SetInteractionsEnabled(true);

                screen.OnGoToEmail.Subscribe(_ => viewManager.GetOut("email")).AddTo(buttonsDisposables);

                screen.OnFacebookLogin
                    .Do(() => Debug.Log("attempting to login with facebook"))
                    .SelectMany(loginWithFacebook.Execute())
                    .Do(() => Debug.Log("logged in with facebook"))
                    .Do(EndAuth)
                    .Subscribe()
                    .AddTo(buttonsDisposables);

                screen.OnAppleLogin
                    .SelectMany(loginWithApple.Execute())
                    .Do(() => Debug.Log("logged in with apple"))
                    .Do(EndAuth)
                    .Subscribe()
                    .AddTo(buttonsDisposables);
            }
            else
            {
                viewManager.DebugGetOut("end-auth");
            }
        }

        void EndAuth()
        {
            viewManager.DebugGetOut("end-auth");
        }

        void Hide()
        {
            buttonsDisposables.Clear();
        }

        void CleanUp()
        {
            disposables.Clear();
        }
    }
}
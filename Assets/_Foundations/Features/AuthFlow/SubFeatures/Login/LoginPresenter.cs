using System;
using Architecture.ViewManager;
using AuthFlow.EndAuth.Repo;
using AuthFlow.Actions;
using AuthFlow.Core.Services;
using AuthFlow.Domain;
using AuthFlow.VerifyEmail.Core.Actions;
using AuthFlow.AboutYou.Infrastructure;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using User.Core;
using User.Core.Domain;
using Architecture.Injector.Core;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.AboutYou.Core.Infrastructure;
using Newtonsoft.Json.Linq;
using Firebase.Auth;

namespace AuthFlow.Login
{
    [UsedImplicitly]
    public class LoginPresenter
    {
        readonly ILoginScreen view;
        readonly IViewManager viewManager;

        readonly IAuthStateManager authState;
        readonly IFirebaseAccess firebaseAccess;
        readonly ISignWithEmail signWithEmail;
        readonly IPasswordValidator passwordValidator;
        readonly CompositeDisposable disposables = new CompositeDisposable();
        readonly CompositeDisposable sessionDisposables = new CompositeDisposable();
        readonly SaveUserInfo saveUserInfo;
        readonly MarkAsLoggedIn markAsLoggedIn;
        readonly IsEmailVerified isEmailVerified;
        readonly IAboutYouWebClient ayWebClient;

        public LoginPresenter(ILoginScreen view, IViewManager viewManager,
            IPasswordValidator passwordValidator, IFirebaseAccess firebaseAccess,
            IAuthStateManager authState, ISignWithEmail signWithEmail,
            SaveUserInfo saveUserInfo, MarkAsLoggedIn markAsLoggedIn,
            IAboutYouWebClient ayWebClient,
            IsEmailVerified isEmailVerified)
        {
            this.view = view;
            this.viewManager = viewManager;
            this.passwordValidator = passwordValidator;
            this.firebaseAccess = firebaseAccess;
            this.authState = authState;
            this.signWithEmail = signWithEmail;
            this.saveUserInfo = saveUserInfo;
            this.markAsLoggedIn = markAsLoggedIn;
            this.isEmailVerified = isEmailVerified;
            this.ayWebClient = ayWebClient;

            view.OnShowView.Subscribe(_ => Present()).AddTo(disposables);
            view.OnHideView.Subscribe(_ => Hide()).AddTo(disposables);
            view.OnDisposeView.Subscribe(_ => CleanUp()).AddTo(disposables);
            sessionDisposables.AddTo(disposables);
        }

        void Present()
        {
            view.SetError(null);
            firebaseAccess.App.Subscribe(app => { Debug.Log("app name : " + app.Name); }).AddTo(disposables);
            view.SetEmail(authState.Email);
            view.OnMoveNext
                .Subscribe(_ => OnSubmit()).AddTo(sessionDisposables);
            view.OnGoToBack
                .Do(view.ClearPasswordField)
                .Subscribe(_ => MoveToBackView()).AddTo(sessionDisposables);
            view.OnGoToForgot
                .Do(view.ClearPasswordField)
                .Subscribe(_ => MoveToForgot()).AddTo(sessionDisposables);
        }

        void MoveToForgot()
        {
            viewManager.GetOut("forgot-pass");
        }

        async void OnSubmit()
        {
            view.SetError(null);
            view.SetLoadingPanelActive(true);

            (bool passwordIsValid, string validationError) = passwordValidator.Validate(view.GetPassword());
            if (passwordIsValid)
            {
                authState.Password = view.GetPassword();

                (FirebaseUser firebaseUser, string loginToken, string error) = await JesseUtils.SignInUser(authState.Email, authState.Password);

                if (error != null)
                {
                  view.SetError(error);
                }
                else
                {
                  view.ClearPasswordField();
                  viewManager.DebugGetOut("end-auth");
                }
            }
            else
            {
                view.SetError(validationError);
            }

            view.SetLoadingPanelActive(false);
        }

#if false
        void HandleException(Exception ex)
        {
            var fullMessage = ex.Message + ex.StackTrace + ex.InnerException?.Message;
            if (fullMessage.Contains("password"))
            {
                view.SetError("Incorrect Password");
            }
            else
            {
                Debug.LogError(ex);
                throw ex;
            }
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
#endif

        void MoveToBackView()
        {
            viewManager.GetOut("back-view");
        }

        void Hide()
        {
            sessionDisposables.Clear();
        }

        void CleanUp()
        {
            disposables.Clear();
        }
    }
}

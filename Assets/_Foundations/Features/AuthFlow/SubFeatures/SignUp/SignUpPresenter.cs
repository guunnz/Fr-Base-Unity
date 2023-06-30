using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.Actions;
using AuthFlow.Core.Services;
using AuthFlow.Domain;
using AuthFlow.EndAuth.Repo;
using Firebase.Auth;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
using User.Core;
using User.Core.Domain;

namespace AuthFlow.SignUp
{
    [UsedImplicitly]
    public class SignUpPresenter
    {
        readonly ISignUpScreen view;
        readonly IViewManager viewManager;
        readonly IPasswordValidator passwordValidator;
        readonly IAuthStateManager authState;
        readonly CompositeDisposable disposables = new CompositeDisposable();
        readonly CompositeDisposable sessionDisposables = new CompositeDisposable();
        readonly IAboutYouWebClient ayWebClient;

        public SignUpPresenter(ISignUpScreen view,
            IViewManager viewManager,
            IPasswordValidator passwordValidator,
            IAuthStateManager authState,
            IAboutYouWebClient ayWebClient
          )
        {
            this.view = view;
            this.viewManager = viewManager;
            this.passwordValidator = passwordValidator;
            this.authState = authState;
            this.ayWebClient = ayWebClient;

            view.OnShowView.Subscribe(_ => Present()).AddTo(disposables);
            view.OnHideView.Subscribe(_ => Hide()).AddTo(disposables);
            view.OnDisposeView.Subscribe(_ => CleanUp()).AddTo(disposables);
            sessionDisposables.AddTo(disposables);
        }

        void Present()
        {
            view.SetError(null);
            view.SetEmail(authState.Email);
            view.OnMoveNext.Subscribe(_ => OnSubmit()).AddTo(sessionDisposables);
            view.OnGoToBack.Subscribe(_ => MoveToBackView()).AddTo(sessionDisposables);
        }

        async void OnSubmit()
        {
            view.SetError(null);
            view.SetLoadingPanelActive(true);

            string password = view.GetPassword();

            (bool passwordIsValid, string pwValidationError) = passwordValidator.Validate(password);

            if (passwordIsValid)
            {
                authState.Password = password;

                (FirebaseUser firebaseUser, string loginToken, string error) = await JesseUtils.SignUpUser(authState.Email, authState.Password);

                if (error == null)
                {
                  viewManager.DebugGetOut("next-view");
                  view.ClearPasswordField();
                }
                else
                {
                  view.SetError(error);
                }
            }
            else
            {
                view.SetError(pwValidationError);
            }

            view.SetLoadingPanelActive(false);
        }

        void MoveToBackView()
        {
            viewManager.DebugGetOut("back-view");
            view.ClearPasswordField();
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

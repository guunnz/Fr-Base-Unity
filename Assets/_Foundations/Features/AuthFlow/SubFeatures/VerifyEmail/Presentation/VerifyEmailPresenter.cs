using System;
using System.Collections;
using Architecture.ViewManager;
using AuthFlow.VerifyEmail.Core.Actions;
using Firebase.Auth;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace AuthFlow.VerifyEmail.Presentation
{
    [UsedImplicitly]
    public class VerifyEmailPresenter
    {
        readonly IVerifyEmailView view;
        readonly CompositeDisposable disposables = new CompositeDisposable();
        readonly IAuthStateManager authState;
        readonly SendVerifyEmail sendVerifyEmail;
        readonly IViewManager viewManager;

        public VerifyEmailPresenter(IVerifyEmailView view, IAuthStateManager authState, SendVerifyEmail sendVerifyEmail, IViewManager viewManager)
        {
            this.view = view;
            this.authState = authState;
            this.sendVerifyEmail = sendVerifyEmail;
            this.viewManager = viewManager;
            this.view.OnShowView.Subscribe(Present).AddTo(disposables);
            this.view.OnHideView.Subscribe(Hide).AddTo(disposables);
            this.view.OnDisposeView.Subscribe(CleanUp).AddTo(disposables);
        }

        void Present()
        {
            view.SetEmail(authState.Email);
            sendVerifyEmail.Execute().Subscribe().AddTo(disposables);
            view.OnSendAgain.SelectMany(sendVerifyEmail.Execute()).Subscribe().AddTo(disposables);
            CheckVerifyEmail().ToObservable().ObserveOnMainThread().Subscribe().AddTo(disposables);
            view.OnGoToBack.Subscribe(_ => MoveToBackView()).AddTo(disposables);
        }

        IEnumerator CheckVerifyEmail()
        {
            yield return null;

            while (!FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified)
            {
                yield return new WaitForSeconds(1);
                yield return FirebaseAuth.DefaultInstance.CurrentUser
                    .ReloadAsync()
                    .ToObservable()
                    .ObserveOnMainThread()
                    .ToYieldInstruction();
            }

            viewManager.GetOut("continue");
        }

        void MoveToBackView()
        {
            JesseUtils.Logout();
            viewManager.GetOut("back");
        }

        void Hide() => disposables.Clear();
        void CleanUp() => disposables.Clear();
    }
}

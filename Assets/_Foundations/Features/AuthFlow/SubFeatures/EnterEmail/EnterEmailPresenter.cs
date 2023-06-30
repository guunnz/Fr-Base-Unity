using System.Text.RegularExpressions;
using Architecture.ViewManager;
using AuthFlow.Actions;
using UniRx;
using UnityEngine;

namespace AuthFlow.EnterEmail
{
    public class EnterEmailPresenter
    {
        const string FieldIsRequired = "Field is required";
        const string EnterValidEmail = "Enter valid email";
        

        readonly IEnterEmailScreen screen;
        readonly IViewManager viewManager;
        readonly CompositeDisposable disposables = new CompositeDisposable();
        readonly CompositeDisposable buttonSubscription = new CompositeDisposable();
        readonly IAuthStateManager state;
        readonly MailVerification verifyEmail;

        public EnterEmailPresenter(IEnterEmailScreen screen, IViewManager viewManager, IAuthStateManager state,
            MailVerification verifyEmail)
        {
            this.screen = screen;
            this.viewManager = viewManager;
            this.state = state;
            this.verifyEmail = verifyEmail;
            this.screen.OnShowView.Subscribe(_ => Present()).AddTo(disposables);
            this.screen.OnHideView.Subscribe(_ => Hide()).AddTo(disposables);
            this.screen.OnDisposeView.Subscribe(_ => CleanUp()).AddTo(disposables);
            buttonSubscription
                .AddTo(disposables);
        }

        void Present()
        {
            screen.SetInputText(state.Email);
            screen.OnGoToNext.Subscribe(_ => OnSubmit()).AddTo(buttonSubscription);
            screen.OnGoToBack.Subscribe(_ => MoveToBackView()).AddTo(buttonSubscription);
        }

        void OnSubmit()
        {
            var regexEmail = new Regex(@"^([\w\.\-\+]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var inputText = screen.GetInputText();

            if (string.IsNullOrEmpty(inputText))
            {
                screen.ShowError(true);
                screen.SetErrorText(FieldIsRequired);
                screen.SetOutlineError();
            }
            else if (regexEmail.Match(inputText).Success)
            {
                screen.ShowError(false);
                state.Email = inputText;
                screen.SetOutlineRegular();
                CheckEmail();
            }
            else
            {
                screen.ShowError(true);
                screen.SetErrorText(EnterValidEmail);
                screen.SetOutlineError();
            }
        }

        void CheckEmail()
        {
            buttonSubscription.Clear();
            verifyEmail.ExecuteValidEmail()
                .Do(WhenKnowIfEmailAlreadyExists)
                .Subscribe().AddTo(disposables);
        }

        void WhenKnowIfEmailAlreadyExists(bool emailAlreadyExists)
        {
            var outPort = emailAlreadyExists ? "log-in" : "sign-up";
            viewManager.DebugGetOut(outPort);
        }

        void MoveToBackView()
        {
            viewManager.GetOut("back-view");
        }

        void Hide()
        {
            buttonSubscription.Clear();
        }

        void CleanUp()
        {
            disposables.Clear();
        }
    }
}
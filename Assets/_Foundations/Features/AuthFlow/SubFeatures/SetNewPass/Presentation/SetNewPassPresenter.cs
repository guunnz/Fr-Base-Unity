using System;
using Architecture.ViewManager;
using AuthFlow.SetNewPass.Core.Actions;
using JetBrains.Annotations;
using UniRx;
using UniRx.Diagnostics;

namespace AuthFlow.SetNewPass.Presentation
{
    [UsedImplicitly]
    public class SetNewPassPresenter
    {
        readonly ISetNewPassView view;
        readonly CompositeDisposable disposables = new CompositeDisposable();
        readonly IPasswordValidator passValidator;
        readonly SetNewPassword setNewPassword;
        readonly IViewManager viewManager;
        readonly LoginWithNewPass loginWithNewPass;

        public SetNewPassPresenter(ISetNewPassView view, IPasswordValidator passValidator,
            SetNewPassword setNewPassword, IViewManager viewManager, LoginWithNewPass loginWithNewPass)
        {
            this.view = view;
            this.passValidator = passValidator;
            this.setNewPassword = setNewPassword;
            this.viewManager = viewManager;
            this.loginWithNewPass = loginWithNewPass;
            this.view.OnShowView.Subscribe(Present).AddTo(disposables);
            this.view.OnHideView.Subscribe(Hide).AddTo(disposables);
            this.view.OnDisposeView.Subscribe(CleanUp).AddTo(disposables);
        }

        void Present()
        {
            view.OnSubmit
                .Select(ValidateInput)
                .WhereTrue()
                .SelectMany(SetNewPass())
                .SelectMany(UpdateNewPass())
                .Do(() => viewManager.GetOut("next-view"))
                .Subscribe()
                .AddTo(disposables);
        }

        IObservable<Unit> UpdateNewPass() =>
            loginWithNewPass.Execute(view.NewPass.Value)
                .Debug("Successfully Relogin with new pass");

        IObservable<Unit> SetNewPass() => setNewPassword.Execute(view.NewPass.Value);

        bool ValidateInput()
        {
            (bool passwordIsValid, string validationError) = passValidator.Validate(view.NewPass.Value);

            if (!passwordIsValid)
            {
                view.NewPass.ShowError(validationError);
                return false;
            }

            if (view.ConfirmPass.Value != view.NewPass.Value)
            {
                view.ConfirmPass.ShowError("Both fields should match");
                return false;
            }

            return true;
        }

        void Hide() => disposables.Clear();
        void CleanUp() => disposables.Clear();
    }
}

using Architecture.ViewManager;
using JetBrains.Annotations;
using UniRx;

namespace AuthFlow.ForgotPass.Presentation
{
    [UsedImplicitly]
    public class ForgotPassPresenter
    {
        readonly IForgotPassView view;
        readonly CompositeDisposable disposables = new CompositeDisposable();
        readonly IAuthStateManager authState;
        readonly SendForgotPassword sendForgotPassword;
        readonly IViewManager viewManager;


        public ForgotPassPresenter(IForgotPassView view, IAuthStateManager authState,
            SendForgotPassword sendForgotPassword, IViewManager viewManager)
        {
            this.view = view;
            this.authState = authState;
            this.sendForgotPassword = sendForgotPassword;
            this.viewManager = viewManager;
            this.view.OnShowView.Subscribe(Present).AddTo(disposables);
            this.view.OnDisposeView.Subscribe(CleanUp).AddTo(disposables);
            this.view.OnHideView.Subscribe(Hide).AddTo(disposables);
        }


        void Present()
        {
            view.SetEmail(authState.Email);
            sendForgotPassword.Execute().Subscribe().AddTo(disposables);
            view.OnGoToBack.Subscribe(_ => MoveToBackView()).AddTo(disposables);
            view.OnSendAgain.Subscribe(_ => Resend()).AddTo(disposables);
        }

        private void Resend()
        {
            sendForgotPassword.Execute().Subscribe().AddTo(disposables);
            view.OnResend();
        }

        private void MoveToBackView()
        {
            viewManager.GetOut("back");
        }

        void Hide() => disposables.Clear();
        void CleanUp() => disposables.Clear();
    }
}
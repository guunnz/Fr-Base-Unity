using System;
using Architecture.MVP;
using UniRx;

namespace AuthFlow.ForgotPass.Presentation
{
    public interface IForgotPassView : IPresentable
    {
        void SetEmail(string infoEmail);
        IObservable<Unit> OnSendAgain { get; }
        IObservable<Unit> OnGoToBack { get; }
        public void OnResend();
    }
}
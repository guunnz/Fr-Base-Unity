using System;
using Architecture.MVP;
using UniRx;

namespace AuthFlow.VerifyEmail.Presentation
{
    public interface IVerifyEmailView : IPresentable
    {
        void SetEmail(string mail);
        IObservable<Unit> OnSendAgain { get; }
        IObservable<Unit> OnGoToBack { get; }
    }
}
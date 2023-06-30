using System;
using Architecture.MVP;
using UniRx;

namespace AuthFlow.Login
{
    public interface ILoginScreen : IPresentable
    {
        IObservable<Unit> OnMoveNext { get; }
        IObservable<Unit> OnGoToBack { get; }
        IObservable<Unit> OnGoToForgot { get; }

        void SetEmail(string userEmail);
        string GetPassword();
        void SetError(string errorMessage);
        void ClearPasswordField();
        void SetLoadingPanelActive(bool state);
    }
}

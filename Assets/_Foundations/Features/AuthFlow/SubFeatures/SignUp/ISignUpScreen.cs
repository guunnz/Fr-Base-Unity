using System;
using Architecture.MVP;
using UniRx;

namespace AuthFlow.SignUp
{
    public interface ISignUpScreen : IPresentable
    {
        IObservable<Unit> OnMoveNext { get; }
        IObservable<Unit> OnGoToBack { get; }

        void SetEmail(string userEmail);
        string GetPassword();
        void SetError(string err);
        void ClearPasswordField();
        void SetLoadingPanelActive(bool state);
    }
}

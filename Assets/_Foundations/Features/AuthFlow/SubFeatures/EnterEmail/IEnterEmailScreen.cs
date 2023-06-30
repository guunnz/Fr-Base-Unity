using System;
using Architecture.MVP;
using UniRx;

namespace AuthFlow.EnterEmail
{
    public interface IEnterEmailScreen : IPresentable
    {
        IObservable<Unit> OnGoToBack { get; }
        IObservable<Unit> OnGoToNext { get; }
        
        string GetInputText();
        void ShowError(bool show);
        void SetErrorText(string errorText);
        void SetOutlineRegular();
        void SetOutlineError();
        void SetInputText(string email);
    }
    
}
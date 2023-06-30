using System;
using UniRx;

namespace AuthFlow.Presentation
{
    public interface IAuthFlowScreen
    {
        
        IObservable<Unit> OnSubmit { get; }
        string UserFieldText { get; set; }
        string PassFieldText { get; }
        IObservable<Unit> OnEnabled { get; }
        IObservable<Unit> OnDisabled { get; }
        IObservable<Unit> OnDisposed { get; }
    }
}
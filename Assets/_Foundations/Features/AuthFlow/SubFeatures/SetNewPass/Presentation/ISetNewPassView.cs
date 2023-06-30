using System;
using Architecture.MVP;
using UI.DataField;
using UniRx;

namespace AuthFlow.SetNewPass.Presentation
{
    public interface ISetNewPassView : IPresentable
    {
        IDataField<string> NewPass { get; }
        IDataField<string> ConfirmPass { get; }
        IObservable<Unit> OnSubmit { get; }
    }
}
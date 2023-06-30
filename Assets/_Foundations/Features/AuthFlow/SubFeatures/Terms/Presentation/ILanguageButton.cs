using System;
using UniRx;

namespace AuthFlow.Terms.Presentation
{
    public interface ILanguageButton
    {
        IObservable<Unit> OnCLick { get; }
        string Key { get; }
    }
}
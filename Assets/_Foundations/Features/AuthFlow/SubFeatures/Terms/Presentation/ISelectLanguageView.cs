using System;
using UniRx;

namespace AuthFlow.Terms.Presentation
{
    public interface ISelectLanguageView
    {
        IObservable<Unit> ViewClosed { get; }
        void Show();
    }
}
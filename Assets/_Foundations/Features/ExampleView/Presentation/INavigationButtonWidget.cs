using System;
using Architecture.MVP;
using UniRx;

namespace ExampleView.Presentation
{
    public interface INavigationButtonWidget : IPresentable
    {
        IObservable<Unit> OnClick { get; }
        string OutPort { get; }
    }
}
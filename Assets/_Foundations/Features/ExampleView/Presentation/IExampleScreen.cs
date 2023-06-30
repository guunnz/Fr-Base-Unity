// the view interface must live on the presentation context
// cause present shouldn't know directly to the view

using System;
using Architecture.MVP;
using UniRx;

namespace ExampleView.Presentation
{
    public interface IExampleScreen : IPresentable
    {
        IObservable<Unit> OnGoToNextView { get; }
    }
}
using System;
using UniRx;

namespace Architecture.MVP
{
    /// <summary>
    /// interface to be implemented for views
    /// if you inherit from <see cref="ViewManager.ViewNode"/> those are already implemented
    /// and you can use those as implementation
    /// the idea is to create your own view interface on the presentation part, and make that interface inherit from
    /// <see cref="IPresentable"/>
    /// so you can use those methods on a presenter just by knowing the interface
    /// </summary>
    public interface IPresentable
    {
        IObservable<Unit> OnFocusView { get; }
        IObservable<Unit> OnUnfocusView { get; }
        IObservable<Unit> OnShowView { get; }
        IObservable<Unit> OnHideView { get; }
        IObservable<Unit> OnDisposeView { get; }
    }
}
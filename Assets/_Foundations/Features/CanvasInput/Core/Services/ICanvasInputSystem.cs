using System;

namespace CanvasInput.Core.Services
{
    public interface ICanvasInputSystem
    {
        void Enable();
        void Disable();
        IObservable<InputInfo> DragBegin();
        IObservable<InputInfo> DragUpdate();
        IObservable<InputInfo> DragEnd();
        IObservable<InputInfo> Tap();
    }
}
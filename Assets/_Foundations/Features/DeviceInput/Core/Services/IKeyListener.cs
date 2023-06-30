using System;
using UniRx;
using UnityEngine;

namespace DeviceInput.Core.Services
{
    public interface IKeyListener
    {
        IObservable<Unit> OnKeyPress(KeyCode keyCode);
        IObservable<Unit> OnKeyRelease(KeyCode keyCode);
        IObservable<Unit> OnKey(KeyCode keyCode);//emits every update the key is pressed
        
    }
}
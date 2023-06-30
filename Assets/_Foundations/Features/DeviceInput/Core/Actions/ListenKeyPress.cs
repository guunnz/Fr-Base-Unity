using System;
using DeviceInput.Core.Services;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace DeviceInput.Core.Actions
{
    [UsedImplicitly]
    public class ListenKeyPress
    {
        readonly IKeyListener keyListener;

        public ListenKeyPress(IKeyListener keyListener)
        {
            this.keyListener = keyListener;
        }

        public IObservable<Unit> Execute(KeyCode keyCode)
        {
            return keyListener.OnKeyPress(keyCode);
        }
    }
}
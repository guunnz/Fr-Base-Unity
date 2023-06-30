using System;
using DeviceInput.Core.Services;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace DeviceInput.Infrastructure
{
    [UsedImplicitly]
    public class KeyListener : IKeyListener
    {
        readonly IObservable<Unit> everyUpdate;

        public KeyListener()
        {
            everyUpdate = Observable.EveryUpdate().AsUnitObservable();
        }

        public IObservable<Unit> OnKeyPress(KeyCode keyCode)
        {
            return everyUpdate.Where(_ => Input.GetKeyDown(keyCode));
        }

        public IObservable<Unit> OnKeyRelease(KeyCode keyCode)
        {
            return everyUpdate.Where(_ => Input.GetKeyUp(keyCode));
        }

        public IObservable<Unit> OnKey(KeyCode keyCode)
        {
            return everyUpdate.Where(_ => Input.GetKey(keyCode));
        }
    }
}
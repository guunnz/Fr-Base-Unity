using System;
using UniRx;

namespace AuthFlow.Core.Services
{
    public interface IFirebaseEventBus
    {
        void SafeInitializeFirebase();
        IObservable<Unit> OnUserAuthChange();
    }
}
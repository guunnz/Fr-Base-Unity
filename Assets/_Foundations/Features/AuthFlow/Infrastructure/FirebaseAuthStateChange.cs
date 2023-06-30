using System;
using AuthFlow.Core.Services;
using Firebase.Auth;
using UniRx;

namespace AuthFlow.Infrastructure
{
    public class FirebaseAuthStateChange : IFirebaseEventBus
    {
        private readonly ISubject<Unit> firebaseAuthChange = new Subject<Unit>();

        public void SafeInitializeFirebase()
        {
            FirebaseAuth.DefaultInstance.StateChanged -= AuthStateChanged;
            FirebaseAuth.DefaultInstance.StateChanged += AuthStateChanged;
        }

        private void AuthStateChanged(object sender, EventArgs eventArgs)
        {
            firebaseAuthChange.OnNext(Unit.Default);
        }

        public IObservable<Unit> OnUserAuthChange()
        {
            SafeInitializeFirebase();
            return firebaseAuthChange;
        }
    }

    
}
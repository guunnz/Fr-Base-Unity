using System;
using Firebase.Auth;
using JetBrains.Annotations;
using UniRx;

namespace AuthFlow.VerifyEmail.Core.Actions
{
    [UsedImplicitly]
    public class IsEmailVerified
    {
        public IObservable<bool> Execute()
        {
            return Observable.Return(FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified);
        }
    }
}
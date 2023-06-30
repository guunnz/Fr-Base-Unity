using System;
using Firebase.Auth;
using JetBrains.Annotations;
using UniRx;

namespace AuthFlow.VerifyEmail.Core.Actions
{
    [UsedImplicitly]
    public class SendVerifyEmail
    {
        public IObservable<Unit> Execute()
        {
            //todo: refactor with service/infra
            return FirebaseAuth.DefaultInstance.CurrentUser.SendEmailVerificationAsync().ToObservable().ObserveOnMainThread();
        }
    }
}
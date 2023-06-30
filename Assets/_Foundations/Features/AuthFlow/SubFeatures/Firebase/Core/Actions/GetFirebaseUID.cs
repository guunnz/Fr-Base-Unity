// TODO(Jesse): Delete this
using System;
using Firebase.Auth;
using JetBrains.Annotations;
using UniRx;
using User.Core.Actions;

namespace AuthFlow.Firebase.Core.Actions
{
    [UsedImplicitly]
    public class GetFirebaseUid
    {
        public GetFirebaseUid()
        {
        }

        public IObservable<string> Execute()
        {
            return Observable.Create<string>(obs =>
            {
                obs.OnNext(FirebaseAuth.DefaultInstance.CurrentUser.UserId);
                obs.OnCompleted();
                return Disposable.Empty;
            }).SubscribeOnMainThread().ObserveOnMainThread();
        }
    }
}

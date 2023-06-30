#if false
using System;
using Firebase;
using Firebase.Auth;
using JetBrains.Annotations;
using UniRx;
using User.Core.Actions;

namespace AuthFlow.Firebase.Core.Actions
{
    [UsedImplicitly]
    public class GetUserSessionToken
    {
        readonly GetUserInfo getUserInfo;

        public GetUserSessionToken(GetUserInfo getUserInfo)
        {
            this.getUserInfo = getUserInfo;
        }

        public IObservable<string> Execute()
        {
            return getUserInfo.Execute().Select(info => info.token);
        }
    }
}
#endif
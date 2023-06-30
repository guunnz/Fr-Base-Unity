using System;
using AuthFlow.Firebase.Core.Actions;
using Firebase.Auth;
using JetBrains.Annotations;
using UniRx;
using WebClientTools.Core.Services;

namespace WebClientTools.Infrastructure
{
    [UsedImplicitly]
    public class WebHeadersBuilder : IWebHeadersBuilder
    {
        public WebHeadersBuilder()
        {
        }

        static IObservable<string> GetBearerToken()
        {
            return FirebaseAuth.DefaultInstance.CurrentUser.TokenAsync(true).ToObservable().ObserveOnMainThread();
        }

        public IObservable<(string, string)> BearerToken
        {
            get { return GetBearerToken().Select(token => ("Authorization", "Bearer " + token)); }
        }


        //todo: endpoints not working without the bearer prefix.
        //Discuss it with Marcos needed.
    }
}

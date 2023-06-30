using System;
using AuthFlow.Terms.Core.Services;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
using Web;

namespace AuthFlow.Terms.Infrastructure
{
    [UsedImplicitly]
    public class TermsWebClient : ITermsWebClient
    {
        const string GETTerms = "https://us-central1-friendbase-dev.cloudfunctions.net/api/";

        public IObservable<JObject> GetTerms(string langKey)
        {
            return WebClient.Get($"{GETTerms}terms-and-conds?lang={langKey}")
                .Do(Debug.Log)
                .Select(ri => ri?.json["termsAndConds"]?.Value<JObject>())
                .ObserveOnMainThread();
        }
    }
}
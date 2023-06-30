using System;
using System.Collections.Generic;
using AuthFlow.Terms.Core.Services;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UniRx;


namespace AuthFlow.Terms.Infrastructure
{
    [UsedImplicitly]
    public class TermsService : ITermsService
    {
        readonly ITermsWebClient webClient;
        readonly IDictionary<string, JObject> cache = new Dictionary<string, JObject>();

        readonly IDictionary<string, IObservable<JObject>> runningObservables =
            new Dictionary<string, IObservable<JObject>>();

        public TermsService(ITermsWebClient webClient)
        {
            this.webClient = webClient;
        }

        private (string title, string content) ParseJson(JObject json)
        {
            return (json["title"].Value<string>(), json["content"].Value<string>());
        }

        public IObservable<(string title, string content)> GetTerms(string langKey)
        {
            if (cache.TryGetValue(langKey, out var json))
            {
                return Observable.Empty(ParseJson(json));
            }

            if (runningObservables.TryGetValue(langKey, out var observable))
            {
                return observable.Select(ParseJson);
            }

            var newObservable = webClient.GetTerms(langKey);

            runningObservables[langKey] = newObservable;

            return newObservable.Select(ParseJson);
        }
    }
}

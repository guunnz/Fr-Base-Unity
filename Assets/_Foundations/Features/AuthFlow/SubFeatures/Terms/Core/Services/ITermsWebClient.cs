using System;
using Newtonsoft.Json.Linq;

namespace AuthFlow.Terms.Core.Services
{
    public interface ITermsWebClient
    {
        IObservable<JObject> GetTerms(string langKey);
    }
}
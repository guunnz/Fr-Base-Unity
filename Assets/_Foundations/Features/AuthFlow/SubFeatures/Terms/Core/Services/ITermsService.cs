using System;

namespace AuthFlow.Terms.Core.Services
{
    public interface ITermsService
    {
        public IObservable<(string title, string content)> GetTerms(string langKey);
        
    }
}
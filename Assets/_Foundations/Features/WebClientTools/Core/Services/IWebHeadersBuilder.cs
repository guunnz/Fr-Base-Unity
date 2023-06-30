using System;

namespace WebClientTools.Core.Services
{
    public interface IWebHeadersBuilder
    {
        IObservable<(string, string)> BearerToken { get; }
    }
}
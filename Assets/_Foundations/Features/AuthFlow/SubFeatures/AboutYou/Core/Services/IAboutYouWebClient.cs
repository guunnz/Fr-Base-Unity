using System;
using System.Collections.Generic;
using Functional.Maybe;
using UniRx;

namespace AuthFlow.AboutYou.Core.Services
{
    public interface IAboutYouWebClient
    {
        IObservable<List<string>> GetGendersOptions();

        IObservable<Web.RequestInfo> GetUserData();

        IObservable<Unit> UpdateUserData(
            Maybe<string> firstName = default, 
            Maybe<string> lastName = default,
            Maybe<DateTime> birthDate = default,
            Maybe<string> gender = default, 
            Maybe<string> userName = default);

        IObservable<bool> IsAvailableUserName(string userName);
    }
}

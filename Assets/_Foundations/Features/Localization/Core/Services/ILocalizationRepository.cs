using System;
using System.Collections.Generic;
using Localization.Model;

namespace Localization
{
    internal interface ILocalizationRepository
    {
        bool IsUsingDefaultLanguage();
        void SetCurrentLanguage(Language language);
        Language CurrentLanguage();

        //retrieves dictionary for current language
        IObservable<IReadOnlyDictionary<string, string>> GetDict();
    }
}
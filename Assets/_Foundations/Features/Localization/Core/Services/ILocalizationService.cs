using System;
using Localization.Model;

namespace Localization
{
    public interface ILocalizationService
    {
        Language CurrentLanguage { get; set; }
        bool LanguageHasBeenSetup();
        IObservable<string> Translate(string key);
    }
}
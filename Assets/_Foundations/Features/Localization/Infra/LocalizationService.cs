using System;
using JetBrains.Annotations;
using Localization.Model;
using UniRx;

namespace Localization
{
    [UsedImplicitly]
    internal class LocalizationService : ILocalizationService
    {
        private readonly ILocalizationRepository repository;

        public LocalizationService(ILocalizationRepository repository)
        {
            this.repository = repository;
        }

        public bool LanguageHasBeenSetup()
        {
            return !repository.IsUsingDefaultLanguage();
        }

        public Language CurrentLanguage
        {
            get => repository.CurrentLanguage();
            set => repository.SetCurrentLanguage(value);
        }

        public IObservable<string> Translate(string key)
        {
            return repository.GetDict().Select(d => d[key]);
        }
    }
}
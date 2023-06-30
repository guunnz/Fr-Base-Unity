using System;
using System.Collections.Generic;
using System.Linq;
using CloudStorage.Core;
using JetBrains.Annotations;
using Localization.DTOs;
using Localization.Model;
using LocalStorage.Core;
using Tools.Enums;
using UniRx;
using UnityEngine;

namespace Localization
{
    [UsedImplicitly]
    internal class LocalLocalizationRepository : ILocalizationRepository
    {
        private const Language DefaultLanguage = Language.English;
        private const string LocalizationCurrentLanguageKey = "localization-current-language";
        private const string LocalizationDicts = "localization-dictionaries";
        private readonly ICloudStorage cloudStorage;

        private readonly ILocalStorage localStorage;

        private IReadOnlyDictionary<string, string> cacheDictionary;


        public LocalLocalizationRepository(ILocalStorage localStorage, ICloudStorage cloudStorage)
        {
            this.localStorage = localStorage;
            this.cloudStorage = cloudStorage;
        }

        public void SetCurrentLanguage(Language language)
        {
            if (language != CurrentLanguage()) cacheDictionary = null;

            localStorage.SetString(LocalizationCurrentLanguageKey, language.ToString());
        }

        public Language CurrentLanguage()
        {
            var storedLanguage = localStorage.GetString(LocalizationCurrentLanguageKey);
            return storedLanguage.ParseEnum<Language>() ?? GetDefaultLanguage();
        }

        public IObservable<IReadOnlyDictionary<string, string>> GetDict()
        {
            /*
             * if we have a dictionary on memory-cache (then it's a valid one)
             * return the cached one
             */
            if (cacheDictionary != null) return Observable.Return(cacheDictionary);

            /*
             * if we have local stored an json, then avoid call the cloud
             * parse and cache current language dictionary, to return it
             */
            var dictsJson = localStorage.GetString(LocalizationDicts);
            if (!string.IsNullOrEmpty(dictsJson))
                return Observable.Return(RetrieveAndCacheCorrectDictionary(ParseJson(dictsJson)));

            /*
             * if cache is empty, then, search for the dictionaries json at the cloud
             * then store those, parse the information, retrieve the correct dictionary, memory-cache and return it
             */
            return cloudStorage
                .Fetch<LocalizationsDTO>(LocalizationDicts)
                .Do(CacheJson)
                .Select(DtoToDomain)
                .Select(RetrieveAndCacheCorrectDictionary);
        }

        public bool IsUsingDefaultLanguage()
        {
            var storedLanguage = localStorage.GetString(LocalizationCurrentLanguageKey);
            return !storedLanguage.ParseEnum<Language>().HasValue;
        }

        private IReadOnlyDictionary<string, string> RetrieveAndCacheCorrectDictionary(
            List<(Language language, IReadOnlyDictionary<string, string> dict)> dicts)
        {
            var dict = dicts
                .Where(pair => pair.language == CurrentLanguage())
                .Select(pair => pair.dict)
                .FirstOrDefault() ?? new Dictionary<string, string>();
            return cacheDictionary = dict;
        }

        private void CacheJson(LocalizationsDTO dto)
        {
            localStorage.SetString(LocalizationDicts, JsonUtility.ToJson(dto));
        }

        private List<(Language language, IReadOnlyDictionary<string, string> dict)> ParseJson(string dictsJson)
        {
            var infos = JsonUtility.FromJson<LocalizationsDTO>(dictsJson);
            return DtoToDomain(infos);
        }

        private List<(Language language, IReadOnlyDictionary<string, string> dict)> DtoToDomain(LocalizationsDTO infos)
        {
            var validEntries = infos.infos.Where(info => ValidateLanguage(info.language));
            var domainInfos = validEntries.Select(info =>
            {
                var domainLanguage = info.language.ParseEnum<Language>() ?? DefaultLanguage;
                return (domainLanguage, CreateDictionary(info.pairs));
            });
            return domainInfos.ToList();
        }

        private static IReadOnlyDictionary<string, string> CreateDictionary(IEnumerable<StringKVDTO> kvs)
        {
            var dict = new Dictionary<string, string>();
            foreach (var pair in kvs) dict[pair.k] = pair.v;
            return dict;
        }

        private bool ValidateLanguage(string language)
        {
            if (!language.ParseEnum<Language>().HasValue)
            {
                Debug.LogWarning("wrong language : " + language);
                return false;
            }

            return true;
        }

        private static Language GetDefaultLanguage()
        {
            var defaultLanguage = Application.systemLanguage.ToDomain() ?? DefaultLanguage;
            Debug.Log("get default language "+defaultLanguage);
            return defaultLanguage;
        }
    }
}
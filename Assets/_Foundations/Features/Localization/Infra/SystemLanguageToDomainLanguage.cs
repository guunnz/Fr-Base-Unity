using Localization.Model;
using UnityEngine;

namespace Localization
{
    public static class SystemLanguageToDomainLanguage
    {
        public static Language? ToDomain(this SystemLanguage language)
        {
            return language switch
            {
                SystemLanguage.Spanish => Language.Spanish,
                SystemLanguage.English => Language.English,
                _ => null
            };
        }
    }
}
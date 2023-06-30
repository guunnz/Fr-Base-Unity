using JetBrains.Annotations;
using Localization.Model;

namespace Localization.Actions
{
    [UsedImplicitly]
    public class SetLanguageKey
    {
        readonly ILocalizationService localizationService;


        public SetLanguageKey(ILocalizationService localizationService)
        {
            this.localizationService = localizationService;
        }

        public void Execute(string langKey) =>
            localizationService.CurrentLanguage = langKey switch
            {
                "es" => Language.Spanish,
                "en" => Language.English,
                _ => Language.English
            };
    }

    [UsedImplicitly]
    public class GetLanguageKey
    {
        readonly ILocalizationService localizationService;


        public GetLanguageKey(ILocalizationService localizationService)
        {
            this.localizationService = localizationService;
        }

        public string Execute() =>
            localizationService.CurrentLanguage switch
            {
                Language.English => "en",
                Language.Spanish => "es",
                _ => "en"
            };
    }
}
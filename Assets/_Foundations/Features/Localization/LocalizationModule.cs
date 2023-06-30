using Architecture.Context;
using Architecture.Injector.Core;
using Localization.Actions;

namespace Localization
{
    public class LocalizationModule : IModule
    {
        public void Init()
        {
            Injection.Register<ILocalizationRepository, LocalLocalizationRepository>();
            Injection.Register<ILocalizationService, LocalizationService>();
            Injection.Register<SetLanguageKey>();
            Injection.Register<GetLanguageKey>();
        }
    }
}
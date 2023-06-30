using Architecture.Context;
using Architecture.Injector.Core;

namespace LocalizationSystem
{
    public class LanguageModule : IModule
    {
        public void Init()
        {
            Injection.Register<ILanguage, Language>();
        }
    }
}
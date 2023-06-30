using Architecture.Context;
using Architecture.Injector.Core;
using Settings.Services;

namespace Settings
{
    public class SettingsModule : IModule
    {
        public void Init()
        {
            Injection.Register<ISettingsRepository, LocalSettingsRepository>();
        }
    }
}
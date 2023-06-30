using Architecture.Context;
using Architecture.Injector.Core;

namespace FeatureToggle
{
    public class FeatureToggleModule : IModule
    {
        public void Init()
        {
            Injection.Register<IFeatureService, LocalFeatureService>();
        }
    }
}
using Architecture.Context;
using Architecture.Injector.Core;

namespace AddressablesSystem
{
    public class LoaderModule : IModule
    {
        public void Init()
        {
            Injection.Register<ILoader, Loader>();
        }
    }
}

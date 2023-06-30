using Architecture.Context;
using Architecture.Injector.Core;

namespace Multiuser
{
    public class MultiuserModule : IModule
    {
        public void Init()
        {
            Injection.Register<IMultiuserInstance, LocalMultiuser>();
            Injection.Register<IMultiuser, Multiuser>();
        }
    }
}



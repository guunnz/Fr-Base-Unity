using Architecture.Context;
using Architecture.Injector.Core;

namespace DebugConsole
{
    public class DebugConsoleModule : IModule
    {
        public void Init()
        {
            Injection.Register<IDebugConsole, DebugConsole>();
        }
    }
}
using Architecture.Context;
using Architecture.Injector.Core;
using MemoryStorage.Core;
using MemoryStorage.Infrastructure;

namespace MemoryStorage
{
    public class MemoryStorageModule : IModule
    {
        public void Init()
        {
            Injection.Register<IMemoryStorage, MemoryStorageImpl>();
        }
    }
}
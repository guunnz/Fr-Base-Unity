using Architecture.Context;
using Architecture.Injector.Core;
using LocalStorage.Core;
using LocalStorage.Delivery;

namespace LocalStorage
{
    public class LocalStorageService : IModule
    {
        public void Init()
        {
            Injection.Register<ILocalStorage, UnityLocalStorage>();
        }
    }
}
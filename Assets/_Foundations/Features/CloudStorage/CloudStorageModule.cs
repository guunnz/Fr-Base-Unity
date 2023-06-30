using Architecture.Context;
using Architecture.Injector.Core;
using CloudStorage.Core;
using CloudStorage.Infrastructure;

namespace CloudStorage
{
    public class CloudStorageModule : IModule
    {
        public void Init()
        {
            Injection.Register<ICloudStorage, CloudStorageService>();
        }
    }
}
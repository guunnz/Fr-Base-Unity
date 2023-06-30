using Architecture.Context;
using Architecture.Injector.Core;

namespace Snapshots
{
    public class SnapshotModule : IModule
    {
        public void Init()
        {
            Injection.Register<ISnapshot, SnapshotManager>();
        }
    }
}


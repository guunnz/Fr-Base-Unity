using Architecture.Context;
using Architecture.Injector.Core;
using DeviceInput.Core.Actions;
using DeviceInput.Core.Services;
using DeviceInput.Infrastructure;

namespace DeviceInput
{
    public class DeviceInputModule : IModule
    {
        public void Init()
        {
            Injection.Register<ListenKeyPress>();
            Injection.Register<IKeyListener, KeyListener>();
        }
    }
}
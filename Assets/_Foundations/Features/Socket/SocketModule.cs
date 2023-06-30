using System;
using Architecture.Context;
using Architecture.Injector.Core;
using UniRx;

namespace Socket
{
    public class SocketModule : IModule
    {
        public void Init()
        {
            Injection.Register<ISocketManager, SocketManager>();
        }
    }
}
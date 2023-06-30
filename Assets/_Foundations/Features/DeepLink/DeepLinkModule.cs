using System;
using Architecture.Context;
using Architecture.Injector.Core;
using DeepLink.Core;
using DeepLink.Infrastructure;
using UniRx;

namespace DeepLink
{
    public class DeepLinkModule : IBlockerModule
    {
        public IObservable<Unit> Init()
        {
            Injection.Register<IDeepLinkProcess, DeepLinkProcess>();
            Injection.Register<IDeepLinkService, DeepLinkService>();
            return Observable.Return(Unit.Default);
            
        }
    }
}
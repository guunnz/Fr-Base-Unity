using System;
using Architecture.Context;
using Architecture.Injector.Core;
using AuthFlow.FacebookLogin.Core.Actions;
using AuthFlow.FacebookLogin.Core.Services;
using AuthFlow.FacebookLogin.Infrastructure;
using UniRx;

namespace AuthFlow.FacebookLogin
{
    public class FacebookLoginModule : IBlockerModule
    {
        public IObservable<Unit> Init()
        {
            Injection.Register<IFacebookService, FacebookService>();
            Injection.Register<LoginWithFacebook>();

            return Injection.Get<IFacebookService>().Init();
        }
    }
}
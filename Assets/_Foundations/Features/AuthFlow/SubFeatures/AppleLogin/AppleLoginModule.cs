using Architecture.Context;
using Architecture.Injector.Core;
using AuthFlow.AppleLogin.Core.Actions;
using AuthFlow.AppleLogin.Core.Services;
using AuthFlow.AppleLogin.Infrastructure;

namespace AuthFlow.AppleLogin
{
    public class AppleLoginModule : IModule
    {
        public void Init()
        {
            Injection.Register<IAppleService, AppleService>();
            Injection.Register<IAppleLoginService, AppleLoginService>();
            Injection.Register<IAppleLoginRepository, AppleLoginRepository>();
            Injection.Register<IAppleNonceGenerator, NonceGenerator>();

            Injection.Register<LoginUsingAppleId>();

            Injection.Get<IAppleLoginService>().Init();
        }
    }
}
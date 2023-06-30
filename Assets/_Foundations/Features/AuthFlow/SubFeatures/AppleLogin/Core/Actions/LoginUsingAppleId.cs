using System;
using AuthFlow.AppleLogin.Core.Services;
using AuthFlow.AppleLogin.Infrastructure;
using JetBrains.Annotations;
using UniRx;

namespace AuthFlow.AppleLogin.Core.Actions
{
    [UsedImplicitly]
    public class LoginUsingAppleId
    {
        readonly IAppleLoginService appleLoginService;
        readonly IAppleService appleService;

        public LoginUsingAppleId(IAppleLoginService appleLoginService, IAppleService appleService)
        {
            this.appleLoginService = appleLoginService;
            this.appleService = appleService;
        }


        public IObservable<Unit> Execute()
        {
            return Observable
                .Return(Unit.Default)
                .Do(appleLoginService.Init)
                .SelectMany(appleService.Login());
        }
    }
}
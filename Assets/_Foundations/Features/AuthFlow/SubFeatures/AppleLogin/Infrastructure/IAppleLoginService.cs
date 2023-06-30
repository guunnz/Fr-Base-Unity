using System;

namespace AuthFlow.AppleLogin.Infrastructure
{
    public interface IAppleLoginService
    {
        IObservable<AppleData> SignInAndRequestUserData();
        void Init();
    }
}
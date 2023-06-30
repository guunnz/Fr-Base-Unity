using System;
using UniRx;

namespace AuthFlow.AppleLogin.Core.Services
{
    public interface IAppleService
    {
        IObservable<Unit> Login();
    }
}
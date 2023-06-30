using System;
using UniRx;

namespace AuthFlow.FacebookLogin.Core.Services
{
    public interface IFacebookService
    {
        IObservable<Unit> Init();
        IObservable<Unit> Login();
    }
}
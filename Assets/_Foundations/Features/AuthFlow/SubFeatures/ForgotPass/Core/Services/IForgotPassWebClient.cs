using System;
using UniRx;

namespace AuthFlow.ForgotPass
{
    public interface IForgotPassWebClient
    {
        IObservable<Unit> SendResetPass();
    }
}
using System;
using JetBrains.Annotations;
using UniRx;

namespace AuthFlow.ForgotPass
{
    [UsedImplicitly]
    public class SendForgotPassword
    {
        readonly IForgotPassWebClient webClient;

        public SendForgotPassword(IForgotPassWebClient webClient)
        {
            this.webClient = webClient;
        }

        public IObservable<Unit> Execute()
        {
            return webClient.SendResetPass();
        }
    }
}
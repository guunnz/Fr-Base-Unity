using System;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UniRx;
using Web;

namespace AuthFlow.ForgotPass
{
    [UsedImplicitly]
    public class ForgotPassWebClient : IForgotPassWebClient
    {
        const string ResetPassEndpoint =
            "https://us-central1-friendbase-dev.cloudfunctions.net/api/password-reset-email";

        readonly IAuthStateManager state;

        public ForgotPassWebClient(IAuthStateManager state)
        {
            this.state = state;
        }


        public IObservable<Unit> SendResetPass()
        {
            var json = new JObject
            {
                ["email"] = state.Email
            };
            return WebClient.Post(ResetPassEndpoint, json, true).AsUnitObservable();
        }
    }
}
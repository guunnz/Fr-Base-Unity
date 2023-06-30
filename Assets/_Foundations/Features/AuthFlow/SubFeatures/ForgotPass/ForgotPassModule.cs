using Architecture.Context;
using Architecture.Injector.Core;

namespace AuthFlow.ForgotPass
{
    public class ForgotPassModule : IModule
    {
        public void Init()
        {
            Injection.Register<IForgotPassWebClient, ForgotPassWebClient>();
            Injection.Register<SendForgotPassword>();
        }
    }
}
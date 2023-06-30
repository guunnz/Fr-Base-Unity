using Architecture.Context;
using Architecture.Injector.Core;
using AuthFlow.VerifyEmail.Core.Actions;

namespace AuthFlow.VerifyEmail
{
    public class VerifyEmailModule : IModule
    {
        public void Init()
        {
            Injection.Register<SendVerifyEmail>();
            Injection.Register<IsEmailVerified>();
        }
    }
}
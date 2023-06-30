using Architecture.Context;
using Architecture.Injector.Core;
using AuthFlow.SetNewPass.Core.Actions;

namespace AuthFlow.SetNewPass
{
    public class SetNewPassModule : IModule
    {
        public void Init()
        {
            Injection.Register<SetNewPassword>();
            Injection.Register<LoginWithNewPass>();
        }
    }
}
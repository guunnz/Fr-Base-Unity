using Architecture.Context;
using Architecture.Injector.Core;
using AuthFlow.Firebase.Core.Actions;

namespace AuthFlow.Firebase
{
    public class FirebaseModule : IModule
    {
        public void Init()
        {
            Injection.Register<GetFirebaseUid>();
        }
    }
}
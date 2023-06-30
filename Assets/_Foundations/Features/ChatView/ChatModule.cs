using Architecture.Context;
using Architecture.Injector.Core;
using AuthFlow.EndAuth.Repo;
using ChatView.Core.Services;
using PlayerMovement;

namespace ChatView
{
    public class ChatModule : IModule
    {
        public void Init()
        {
            Injection.Register<ILocalUserInfo>();
            Injection.Register<IChatServices>();
            Injection.Register<IPlayerWorldData>();

        }
    }
}
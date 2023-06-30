using Architecture.Context;
using Architecture.Injector.Core;
using FriendsView.Core.Services;
using FriendsView.Infrastructure;
using PlayerMovement;

namespace FriendsView
{
    public class FriendsModule : IModule
    {
        public void Init()
        {
            Injection.Register<IFriendsNotificationsServices, FriendsNotificationsServices>();
            Injection.Register<IFriendsWebClient, FriendsWebClient>();
        }
    }
}
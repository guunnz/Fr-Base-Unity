using Architecture.Context;
using Architecture.Injector.Core;
using User.Core;
using User.Core.Actions;
using User.Infrastructure;

namespace User
{
    public class UserModule : IModule
    {
        public void Init()
        {
            Injection.Register<UserInfoRepository>();
            Injection.Register<SaveUserInfo>();
            Injection.Register<GetUserInfo>();
        }
    }
}
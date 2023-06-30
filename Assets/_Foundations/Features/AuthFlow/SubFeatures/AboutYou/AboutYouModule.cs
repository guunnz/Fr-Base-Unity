using Architecture.Context;
using Architecture.Injector.Core;
using AuthFlow.AboutYou.Core.Infrastructure;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.AboutYou.Infrastructure;

namespace AuthFlow.AboutYou
{
    public class AboutYouModule : IModule
    {
        public void Init()
        {
            Injection.Register<IAboutYouWebClient,AboutYouWebClient>();
            Injection.Register<IAboutYouStateManager,AboutYouStateManager>();
        }
    }
}
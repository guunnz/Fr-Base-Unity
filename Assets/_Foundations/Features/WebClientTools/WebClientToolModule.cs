using Architecture.Context;
using Architecture.Injector.Core;
using WebClientTools.Core.Services;
using WebClientTools.Infrastructure;

namespace WebClientTools
{
    public class WebClientToolModule : IModule
    {
        public void Init()
        {
            Injection.Register<IWebHeadersBuilder,WebHeadersBuilder>();
        }
    }
}
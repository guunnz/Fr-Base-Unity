using Architecture.Context;
using Architecture.Injector.Core;
using Currency.Infrastructure;
using Currency.Services;

namespace Currency
{
    public class CurrenciesModule : IModule
    {
        public void Init()
        {
            Injection.Register<ICurrencyRepository, LocalCurrencyRepository>();
            Injection.Register<ICurrenciesService, ReactiveCurrenciesService>();
        }
    }
}
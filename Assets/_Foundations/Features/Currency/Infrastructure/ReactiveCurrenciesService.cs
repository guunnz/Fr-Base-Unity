using System;
using System.Collections.Generic;
using Currency.Services;
using JetBrains.Annotations;
using UniRx;

namespace Currency.Infrastructure
{
    [UsedImplicitly]
    public class ReactiveCurrenciesService : ICurrenciesService
    {
        private readonly ICurrencyRepository currencyRepository;

        private readonly Dictionary<string, IReactiveProperty<int>> properties =
            new Dictionary<string, IReactiveProperty<int>>();

        public ReactiveCurrenciesService(ICurrencyRepository currencyRepository)
        {
            this.currencyRepository = currencyRepository;
        }

        public IObservable<int> OnCurrencyChange(string currencyName)
        {
            return GetProperty(currencyName);
        }

        public int this[string key]
        {
            get => currencyRepository[key];
            set
            {
                currencyRepository[key] = value;
                GetProperty(key).Value = value;
            }
        }

        private IReactiveProperty<int> GetProperty(string key)
        {
            if (properties.TryGetValue(key, out var prop)) return prop;
            prop = new ReactiveProperty<int>();
            properties[key] = prop;
            prop.Value = currencyRepository[key];
            return prop;
        }
    }
}
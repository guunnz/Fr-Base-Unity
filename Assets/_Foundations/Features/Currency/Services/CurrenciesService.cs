using System;

namespace Currency.Services
{
    public interface ICurrenciesService
    {
        int this[string key] { get; set; }
        IObservable<int> OnCurrencyChange(string currencyName);
    }
}
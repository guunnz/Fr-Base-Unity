using Architecture.Injector.Core;
using Currency.Services;
using TMPro;
using UniRx;
using UnityEngine;

namespace Currency.UnityDelivery
{
    public class CurrencyDrawer : MonoBehaviour
    {
        public TextMeshProUGUI label;
        public string currencyName;

        private ICurrenciesService currenciesService;

        private void Start()
        {
            currenciesService = Injection.Get<ICurrenciesService>();
            currenciesService
                .OnCurrencyChange(currencyName)
                .ObserveOnMainThread()
                .Subscribe(OnCurrencyChange);
            OnCurrencyChange(currenciesService[currencyName]);
        }

        private void OnCurrencyChange(int newValue)
        {
            label.text = newValue.ToString();
        }
    }
}
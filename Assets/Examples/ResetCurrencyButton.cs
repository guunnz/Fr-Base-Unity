using Architecture.Injector.Core;
using Currency.Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Examples
{
    [RequireComponent(typeof(Button))]
    public class ResetCurrencyButton : MonoBehaviour
    {
        public string currencyName;

        private void Start()
        {
            var service = Injection.Get<ICurrenciesService>();
            gameObject
                .GetComponent<Button>()
                .OnClickAsObservable()
                .Subscribe(_ => service[currencyName] = 0);
        }
    }
}
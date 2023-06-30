using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.View
{
    public class ReactiveToggle : MonoBehaviour, IReactiveProperty<bool>
    {
        [SerializeField] private bool value;
        [SerializeField] private Button button;
        [SerializeField] private GameObject tick;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private readonly ReactiveProperty<bool> state = new ReactiveProperty<bool>(false);

        private void Awake()
        {
            button
                .OnClickAsObservable()
                .Subscribe(_ => OnTap())
                .AddTo(disposables);
        }

        private void OnDestroy()
        {
            disposables.Clear();
        }

        private void OnValidate()
        {
            state.Value = value;
            OnChange();
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
            return state.Subscribe(observer);
        }

        public bool Value
        {
            get => state.Value;
            set
            {
                state.Value = value;
                OnChange();
            }
        }

        public bool HasValue => state.HasValue;

        private void OnTap()
        {
            state.Value = !state.Value;
            OnChange();
        }

        private void OnChange()
        {
            tick?.SetActive(state.Value);
        }
    }
}
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.View
{
    public class ReactiveSlider : MonoBehaviour, IReactiveProperty<float>
    {
        [SerializeField] private Slider slider;

        [SerializeField] [Range(0, 1)] private float value;
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private readonly ReactiveProperty<float> property = new ReactiveProperty<float>();

        private void Awake()
        {
            slider
                .OnValueChangedAsObservable()
                .Subscribe(Change)
                .AddTo(disposables);
        }

        private void OnDestroy()
        {
            disposables.Clear();
        }

        private void OnValidate()
        {
            Value = value;
        }


        public IDisposable Subscribe(IObserver<float> observer)
        {
            return property.Subscribe(observer);
        }

        public float Value
        {
            get => property.Value;
            set
            {
                if (slider)
                    slider.value = value;
                property.Value = value;
            }
        }

        public bool HasValue => property.HasValue;

        private void Change(float newValue)
        {
            property.Value = newValue;
        }
    }
}
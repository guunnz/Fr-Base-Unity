using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;

namespace Shared.View
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class ReactiveDropdown : MonoBehaviour, IReactiveProperty<int>
    {
        [SerializeField] private List<string> options;
        [SerializeField] private int selected;
        [SerializeField] private TMP_Dropdown dropdown;


        private readonly IReactiveProperty<int> property = new ReactiveProperty<int>();

        public List<string> Options
        {
            get => options;
            set
            {
                options = value;
                DrawOptions();
            }
        }

        private void Awake()
        {
            if (dropdown == null) dropdown = GetComponent<TMP_Dropdown>();
            dropdown.onValueChanged.AddListener(ValueChange);
            property.Value = dropdown.value;
        }

        private void OnValidate()
        {
            if (dropdown == null) dropdown = GetComponent<TMP_Dropdown>();
            selected = Mathf.Clamp(selected, 0, options.Count);
            dropdown.value = selected;
            DrawOptions();
        }

        public bool HasValue => property.HasValue;

        public int Value
        {
            get
            {
                var propertyValue = property.Value;
                if (propertyValue != dropdown.value) property.Value = dropdown.value;
                return propertyValue;
            }
            set
            {
                property.Value = value;
                dropdown.value = value;
            }
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            return property.Subscribe(observer);
        }

        private void DrawOptions()
        {
            dropdown.options = Options
                .Select(option => new TMP_Dropdown.OptionData(option))
                .ToList();
        }

        private void ValueChange(int newValue)
        {
            property.Value = newValue;
        }
    }
}
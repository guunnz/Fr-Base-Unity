using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AuthFlow.AboutYou.View
{
    public class ModalDropdown : MonoBehaviour, IObservable<int>
    {
        readonly ISubject<int> subject = new Subject<int>();
        int selected;
        List<string> options;

        public List<string> Options
        {
            get => options.ToList();
            set
            {
                options = value.ToList();
                UpdateView();
            }
        }

        public int Selected
        {
            get => selected;
            set
            {
                selected = value;
                UpdateView();
            }
        }


        //button section
        [Header("Button")] public Button openDropdownButton;

        public StringWidget openDropdownButtonLabel;

        //options section

        [Space(10), Header("Options")] public RectTransform optionsSection;
        public RectTransform buttonsContent;
        public NamedButton prefabButton;
        readonly List<NamedButton> buttons = new List<NamedButton>();


        void Start()
        {
            openDropdownButton.onClick.AddListener(() =>
            {
                UpdateView();
                optionsSection.gameObject.SetActive(true);
            });
        }

        void Awake()
        {
            optionsSection.gameObject.SetActive(false);
        }


        void UpdateView()
        {
            EnsureAmount(options.Count);
            for (var i = 0; i < options.Count; i++)
            {
                buttons[i].Show(options[i], Select(i));
            }
        }

        Action Select(int index) => () =>
        {
            if (options.Count <= index || buttons.Count <= index) return;
            EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
            openDropdownButtonLabel.Value = options[index];
            selected = index;
            optionsSection.gameObject.SetActive(false);
            Clean();
            subject.OnNext(selected);
        };

        void EnsureAmount(int amount)
        {
            var buttonsLeft = amount - buttons.Count;
            if (buttonsLeft <= 0) return;
            for (int i = 0; i < buttonsLeft; i++)
            {
                var namedButton = Instantiate(prefabButton, buttonsContent);
                namedButton.Hide();
                buttons.Add(namedButton);
            }
        }

        void Clean()
        {
            buttons.ForEach(b => { b.Hide(); });
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            return subject.Subscribe(observer);
        }
    }
}
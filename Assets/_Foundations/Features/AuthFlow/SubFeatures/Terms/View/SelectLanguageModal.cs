using System;
using Architecture.Injector.Core;
using AuthFlow.Terms.Presentation;
using Localization.Actions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace AuthFlow.Terms.View
{
    public class SelectLanguageModal : MonoBehaviour, ISelectLanguageView
    {
        //[SerializeField] Button close;
        ILanguageButton[] langButtons;
        readonly ISubject<Unit> viewClosed = new Subject<Unit>();
        public TermsView termsView;
        public IObservable<Unit> ViewClosed => viewClosed;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        void Awake()
        {
            //close.OnClickAsObservable().Subscribe(CloseModal);
            langButtons = GetComponentsInChildren<ILanguageButton>();
            BindLangButtons();
        }

        void CloseModal()
        {
            if (termsView != null)
            {
                termsView.SetWebView();
            }
                
            gameObject.SetActive(false);
            viewClosed.OnNext(default);
        }

        void BindLangButtons()
        {
            var setLang = Injection.Get<SetLanguageKey>();
            foreach (var langButton in langButtons)
            {
                langButton.OnCLick.Subscribe(() =>
                {
                    var langKey = langButton.Key;
                    setLang.Execute(langKey);
                    Debug.Log("execute key " + langKey, langButton as MonoBehaviour);
                    CloseModal();
                });
            }
        }
    }
}
using System;
using System.Collections.Generic;
using Architecture.Injector.Core;
using AuthFlow.Terms.Presentation;
using UniRx;
using UnityEngine;
using LocalizationSystem;

namespace UI.EditAccount
{
    public class PanelLanguage : AbstractUIPanel
    {
        [SerializeField] private GameObject prefabLanguageSelector;
        [SerializeField] private GameObject scrollContent;
        [SerializeField] private GameObject menu;
        [SerializeField] private EditAccountManager editAccountManager;

        protected override void Start()
        {
            base.Start();
            Refresh();
        }

        public override void OnOpen()
        {
            Refresh();
        }

        private void Refresh()
        {
            // Delete scroll content
            foreach (Transform child in scrollContent.transform)
            {
                Destroy(child.gameObject);
            }

            List<string> languages = language.GetAvailablesLanguages();

            foreach (string lang in languages)
            {
                GameObject languageSelector = Instantiate(prefabLanguageSelector, scrollContent.transform);
                ItemLanguage languageItem = languageSelector.GetComponent<ItemLanguage>();
                switch (lang)
                {
                    case LanguageType.ENGLISH:
                        language.SetText(languageItem.text, lang.ToUpper() + " - " + language.GetTextByKey(LangKeys.AUTH_ENGLISH));
                        break;
                    case LanguageType.SPANISH:
                        language.SetText(languageItem.text, lang.ToUpper() + " - " + language.GetTextByKey(LangKeys.AUTH_SPANISH));
                        break;
                    case LanguageType.PORTUGUESE:
                        language.SetText(languageItem.text, lang.ToUpper() + " - " + language.GetTextByKey(LangKeys.AUTH_BRASILIAN_PORTUGUESE));
                        break;
                    case LanguageType.TURKISH:
                        language.SetText(languageItem.text, lang.ToUpper() + " - " + language.GetTextByKey(LangKeys.AUTH_TURKISH));
                        break;
                }
                //languageItem.text.fontStyle = TMPro.FontStyles.Bold;
                languageItem.button.onClick.AddListener(() =>
                {
                    SetLanguage(lang);
                });
            }
        }

        private void SetLanguage(string code)
        {
            Injection.Get<IAvatarEndpoints>().UpdateLanguage(code);
            language.SetCurrentLanguage(code);
            editAccountManager.gameObject.SetActive(true);
            menu.SetActive(true);
            Close();
            CurrentRoom.Instance.ReloadSameRoom();
        }
    }
}
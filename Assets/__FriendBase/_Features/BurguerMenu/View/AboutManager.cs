using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UI;
using LocalizationSystem;
using Architecture.Injector.Core;

namespace BurguerMenu.View
{
    public class AboutManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI versionText;
        [SerializeField] BurguerView burgerView;
        const string creditsUrl = "https://friendbase.com/credits/";
        private ILanguage language;

        private void Start()
        {
            language = Injection.Get<ILanguage>();
        }

        private void OnEnable()
        {
            if (language == null)
                language = Injection.Get<ILanguage>();
            string text = language.GetTextByKey(LangKeys.MAIN_COPYRIGHT_INFO) + " " + language.GetTextByKey(LangKeys.MAIN_VERSION_INFO) + " " + Application.version; //Should be getting version from Firebase

            string environment = "D";
            if (EnvironmentData.IsOnProduction)
            {
                environment = "P";
            }
            text += " - " + environment + "-" + Application.installMode;
            language.SetText(versionText, text);
        }

        public void OpenCredits()
        {
            burgerView.OpenWebView(creditsUrl);
        }
    }
}
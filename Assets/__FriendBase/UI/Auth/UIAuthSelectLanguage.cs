using System.Collections;
using System.Collections.Generic;
using LocalizationSystem;
using TMPro;
using UnityEngine;
using AuthFlow.Terms.View;
public class UIAuthSelectLanguage : AbstractUIPanel
{
    [SerializeField] private UIAuthLanguageCard prefabLanguageSelector;
    [SerializeField] private GameObject scrollContent;
    [SerializeField] private TextMeshProUGUI TxtTitleSelectLanguage;
    [SerializeField] private TermsView termsView;

    public delegate void SelectLanguage(string langKey);
    public event SelectLanguage OnSelectLanguage;

    protected override void Start()
    {
        base.Start();
        Refresh();
    }

    public override void OnOpen()
    {
        language.SetTextByKey(TxtTitleSelectLanguage, LangKeys.AUTH_SELECT_LANGUAGE);
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
            UIAuthLanguageCard languageSelector = Instantiate(prefabLanguageSelector.gameObject, scrollContent.transform).GetComponent<UIAuthLanguageCard>();
            switch (lang)
            {
                case LanguageType.ENGLISH:
                    languageSelector.SetText(lang.ToUpper() + " - " + language.GetTextByKey(LangKeys.AUTH_ENGLISH));
                    break;
                case LanguageType.SPANISH:
                    languageSelector.SetText(lang.ToUpper() + " - " + language.GetTextByKey(LangKeys.AUTH_SPANISH));
                    break;
                case LanguageType.PORTUGUESE:
                    languageSelector.SetText(lang.ToUpper() + " - " + language.GetTextByKey(LangKeys.AUTH_BRASILIAN_PORTUGUESE));
                    break;
                case LanguageType.TURKISH:
                    languageSelector.SetText(lang.ToUpper() + " - " + language.GetTextByKey(LangKeys.AUTH_TURKISH));
                    break;
            }
            //languageItem.text.fontStyle = TMPro.FontStyles.Bold;//
            languageSelector.GetButton().onClick.AddListener(() =>
            {
                SetLanguage(lang);
                termsView.SetWebView();
            });
        }
    }

    private void SetLanguage(string code)
    {
        language.SetCurrentLanguage(code);
        if (OnSelectLanguage!=null)
        {
            OnSelectLanguage(code);
        }
        Close();
    }
}

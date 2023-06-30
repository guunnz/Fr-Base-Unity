using System.Collections;
using System.Collections.Generic;
using AddressablesSystem;
using Architecture.Injector.Core;
using DebugConsole;
using LocalizationSystem;
using LocalStorage.Core;
using UnityEngine;
using LocalizationSystem;

public class InitLanguages : MonoBehaviour
{
    private ILanguage language;
    private IDebugConsole debug;
    private ILocalStorage localStorage;

    void Awake()
    {
        StartCoroutine(InitLanguage());
    }

    IEnumerator InitLanguage()
    {
        language = Injection.Get<ILanguage>();
        debug = Injection.Get<IDebugConsole>();
        localStorage = Injection.Get<ILocalStorage>();

        while (language == null || debug == null || localStorage == null)
        {
            yield return new WaitForEndOfFrame();
            language = Injection.Get<ILanguage>();
            debug = Injection.Get<IDebugConsole>();
            localStorage = Injection.Get<ILocalStorage>();
        }

        TextAsset asset_en = Resources.Load("language/lang_en") as TextAsset;
        TextAsset asset_es = Resources.Load("language/lang_es") as TextAsset;
        TextAsset asset_tr = Resources.Load("language/lang_tr") as TextAsset;
        TextAsset asset_pt = Resources.Load("language/lang_pt") as TextAsset;

        if (asset_en != null)
        {
            language.AddLanguageFromXML(LanguageType.ENGLISH, asset_en.text);
            debug.TraceLog(LOG_TYPE.GENERAL, "English Language added");
        }
        else
        {
            debug.ErrorLog("InitLanguages", "Error adding english language", "");
        }

        if (asset_es != null)
        {
            language.AddLanguageFromXML(LanguageType.SPANISH, asset_es.text);
            debug.TraceLog(LOG_TYPE.GENERAL, "Spanish Language added");
        }
        else
        {
            debug.ErrorLog("InitLanguages", "Error adding spanish language", "");
        }

        if (asset_tr != null)
        {
            language.AddLanguageFromXML(LanguageType.TURKISH, asset_tr.text);
            debug.TraceLog(LOG_TYPE.GENERAL, "Turkish Language added");
        }
        else
        {
            debug.ErrorLog("InitLanguages", "Error adding turkish language", "");
        }

        if (asset_pt != null)
        {
            language.AddLanguageFromXML(LanguageType.PORTUGUESE, asset_pt.text);
            debug.TraceLog(LOG_TYPE.GENERAL, "Portuguese Language added");
        }
        else
        {
            debug.ErrorLog("InitLanguages", "Error adding portuguese language", "");
        }

        string savedLanguage = localStorage.GetString(Language.CURRENT_LANGUAGE_STORAGE_KEY);
        if (string.IsNullOrEmpty(savedLanguage))
        {
            DetectLanguage();
        }
        else
        {
            language.SetCurrentLanguage(savedLanguage);
        }
    }

    void DetectLanguage()
    {
        language.SetCurrentLanguage(LanguageType.ENGLISH);
        SystemLanguage lang = Application.systemLanguage;
        switch (lang)
        {
            case SystemLanguage.English:
                language.SetCurrentLanguage(LanguageType.ENGLISH);
                localStorage.SetString(Language.CURRENT_LANGUAGE_STORAGE_KEY, LanguageType.ENGLISH);
                break;
            case SystemLanguage.Spanish:
                language.SetCurrentLanguage(LanguageType.SPANISH);
                localStorage.SetString(Language.CURRENT_LANGUAGE_STORAGE_KEY, LanguageType.SPANISH);
                break;
            case SystemLanguage.Turkish:
                language.SetCurrentLanguage(LanguageType.TURKISH);
                localStorage.SetString(Language.CURRENT_LANGUAGE_STORAGE_KEY, LanguageType.TURKISH);
                break;
            case SystemLanguage.Portuguese:
                language.SetCurrentLanguage(LanguageType.PORTUGUESE);
                localStorage.SetString(Language.CURRENT_LANGUAGE_STORAGE_KEY, LanguageType.PORTUGUESE);
                break;
            default:
                language.SetCurrentLanguage(LanguageType.ENGLISH);
                localStorage.SetString(Language.CURRENT_LANGUAGE_STORAGE_KEY, LanguageType.ENGLISH);
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LocalizationSystem;
using Architecture.Injector.Core;

public class SetVersion : MonoBehaviour
{
    private TextMeshProUGUI text;
    private ILanguage language;
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        language = Injection.Get<ILanguage>();
        if (language == null)
            language = Injection.Get<ILanguage>();
        text.text = language.GetTextByKey(LangKeys.MAIN_COPYRIGHT_INFO) + " " + language.GetTextByKey(LangKeys.MAIN_VERSION_INFO) + " " + Application.version; //Should be getting version from Firebase
    }
}
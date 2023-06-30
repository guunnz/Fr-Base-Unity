using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Architecture.Injector.Core;
using LocalizationSystem;

public class UIAuthLanguageCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private Button btnLanguage;

    protected ILanguage language;

    public void SetText(string text)
    {
        language = Injection.Get<ILanguage>();
        language.SetText(txtTitle, text);
    }

    public Button GetButton()
    {
        return btnLanguage;
    }
}

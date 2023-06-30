using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LocalizationSystem;
using Architecture.Injector.Core;

public class UIStoreFurnitureNoItems : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtDesc;

    protected ILanguage language;

    void Start()
    {
        language = Injection.Get<ILanguage>();

        language.SetTextByKey(txtTitle, LangKeys.STORE_YOU_DONT_HAVE_ANY_FURNITURE);
        language.SetTextByKey(txtDesc, LangKeys.STORE_WHEN_YOU_BUY_SOMETHING_IT_SHOWS_HERE);
    }
}

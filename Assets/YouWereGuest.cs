using LocalizationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Architecture.Injector.Core;

public class YouWereGuest : MonoBehaviour
{
    private ILanguage language;

    [SerializeField] private TextMeshProUGUI Title;
    [SerializeField] private TextMeshProUGUI Description;
    [SerializeField] private TextMeshProUGUI Button;

    private void Start()
    {
        language = Injection.Get<ILanguage>();

        Title.text = language.GetTextByKey(LangKeys.GUEST_WELCOME_BACK);
        Description.text = language.GetTextByKey(LangKeys.GUEST_HERE_IS_A_GIFT);
        Button.text = language.GetTextByKey(LangKeys.GUEST_LEARN_WHAT_YOU_HAVE_UNLOCKED);
    }
}

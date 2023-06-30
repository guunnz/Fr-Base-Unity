using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Localization;
using LocalizationSystem;
using Architecture.Injector.Core;
using TMPro;

public class ChatGuest : MonoBehaviour
{
    private ILanguage language;

    [SerializeField] TextMeshProUGUI RegisterToChatText;
    [SerializeField] TextMeshProUGUI RegisterAndGetGemsText;

    private void Awake()
    {
        language = Injection.Get<ILanguage>();
    }

    void Start()
    {
        language.SetTextByKey(RegisterToChatText, LangKeys.GUEST_NEED_REGISTER_TO_CHAT);
        RegisterAndGetGemsText.text = language.GetTextByKey(LangKeys.GUEST_REGISTER_GET_FREE_GEMS).Replace("[GEM ICON]", "<sprite=0>");
    }
}
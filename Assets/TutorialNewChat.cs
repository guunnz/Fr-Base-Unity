using Architecture.Injector.Core;
using LocalizationSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialNewChat : MonoBehaviour
{
    public ILanguage language;
    public TextMeshProUGUI TitleStep1;
    public TextMeshProUGUI TextDescriptionStep1;
    public TextMeshProUGUI HereTextStep1;
    public TextMeshProUGUI TextDescriptionStep2;
    public TextMeshProUGUI TextDescriptionStep3;
    public TextMeshProUGUI ChatOnlyWithPlayerStep3;
    public TextMeshProUGUI ChatOnlyWithAllStep3;
    public TextMeshProUGUI ReadyStep3;
    public GameObject container;

    private void Start()
    {
        language = Injection.Get<ILanguage>();
        SetLanguage();
        bool DidTutorial = PlayerPrefs.GetInt("TutorialText") == 1;

        container.SetActive(!DidTutorial);
    }

    void SetLanguage()
    {
        TitleStep1.text = language.GetTextByKey(LangKeys.TUTORIAL_CHAT_NEW_PRIVATE_CHAT);
        TextDescriptionStep1.text = language.GetTextByKey(LangKeys.TUTORIAL_CHAT_START_PRIVATE_CHAT);
        HereTextStep1.text = language.GetTextByKey(LangKeys.TUTORIAL_CHAT_HERE);
        TextDescriptionStep2.text = language.GetTextByKey(LangKeys.TUTORIAL_CHAT_STARTED_CHAT);
        TextDescriptionStep3.text = language.GetTextByKey(LangKeys.TUTORIAL_CHAT_PRIVATE_AND_PUBLIC_CHAT);
        ChatOnlyWithPlayerStep3.text = language.GetTextByKey(LangKeys.TUTORIAL_CHAT_PUBLIC_CHAT);
        ChatOnlyWithAllStep3.text = language.GetTextByKey(LangKeys.TUTORIAL_CHAT_CHAT_WITH_EVERYONE);
        ReadyStep3.text = language.GetTextByKey(LangKeys.TUTORIAL_CHAT_READY);
    }

    public void ReadyBtn()
    {
        PlayerPrefs.SetInt("TutorialText", 1);
    }
}
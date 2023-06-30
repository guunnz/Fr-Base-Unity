using LocalizationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Architecture.Injector.Core;

public class WeHaveNewMinigame : MonoBehaviour
{
    [SerializeField] GameObject container;
    [SerializeField] ILanguage language;
    [SerializeField] TextMeshProUGUI Title;
    [SerializeField] TextMeshProUGUI GoPlay;

    private void Start()
    {
        int open = PlayerPrefs.GetInt("NewGameRacing");

        if (open == 0)
        {
            SetLanguage();
            container.SetActive(true);
            PlayerPrefs.SetInt("NewGameRacing", 1);
        }
    }

    void SetLanguage()
    {
        language = Injection.Get<ILanguage>();
        GoPlay.text = language.GetTextByKey(LangKeys.RACING_LETS_PLAY);
        Title.text = language.GetTextByKey(LangKeys.NEW_MINIGAME);
    }
    public void Close()
    {
        container.SetActive(false);
    }
}
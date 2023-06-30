using Architecture.Injector.Core;
using LocalizationSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModalGamesLocalize : MonoBehaviour
{

    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI OkButton;
    public ILanguage language;

    // Start is called before the first frame update
    void Start()
    {
        SetLanguage();
    }

    void SetLanguage()
    {
        language = Injection.Get<ILanguage>();
        language.SetTextByKey(Title, LangKeys.MAIN_PLAY_GAMES_TO_EARN_GOLD);
        language.SetTextByKey(Description, LangKeys.MAIN_USE_GOLD_TO_BUY_ITEMS_AND_GIFTS);
        language.SetTextByKey(OkButton, LangKeys.MAIN_GO_TO_GAMES);
    }

    public void GoToMinigames()
    {
        StartCoroutine(GoToMinigamesCoroutine());
    }

    IEnumerator GoToMinigamesCoroutine()
    {
        //Injection.Get<IViewManager>().Show<AvatarCustomizationPanel>();
        SceneManager.LoadScene(GameScenes.Minigames, LoadSceneMode.Additive);

        yield return new WaitForEndOfFrame();
        SceneManager.UnloadSceneAsync(GameScenes.AvatarCustomization);
    }
}
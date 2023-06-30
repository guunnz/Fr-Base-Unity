using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AddressablesSystem;
using Architecture.Injector.Core;
using FriendsView.Core.Domain;
using System;
using Data;
using Audio.Music;

public enum Game
{
    Racing = 1,
    SpaceJump = 2,
    ComingSoon = 3,
}

public class MinigameManager : MonoBehaviour
{
    AbstractMinigame miniGame;
    MinigameInformation gameInformation;
    private ILoader loader;
    IMusicPlayer musicPlayer;
    private IGameData gameData;
    [SerializeField] MinigameMenuManager menuManager;
    [SerializeField] Transform minigameParent;
    [SerializeField] GameObject loadingGame;



    private void Start()
    {/*Play music temporary*/
        Injection.Get(out musicPlayer);
        musicPlayer.Stop();
        gameData = Injection.Get<IGameData>();
        loader = Injection.Get<ILoader>();
    }

    public void LoadGame(MinigameInformation gameInformation)
    {
        StartCoroutine(LoadGameCoroutine(gameInformation));
    }

    private IEnumerator LoadGameCoroutine(MinigameInformation gameInformation)
    {
        loadingGame.SetActive(true);
        this.gameInformation = gameInformation;

        string minigame = Enum.GetName(typeof(Game), gameInformation.idGame);

        loader.LoadItem(new LoaderItemModel(minigame));

        LoaderAbstractItem item = loader.GetItem(minigame);

        while (item == null || item.State != LoaderItemState.SUCCEED)
        {
            yield return null;
        }

      
        SetMinigame(this.gameInformation);
    }

    private void SetMinigame(MinigameInformation gameInformation)
    {
        miniGame = GetMinigameInstance(gameInformation.idGame);
        miniGame.transform.parent = minigameParent;
        miniGame.SetMinigameManager(this);
        loadingGame.SetActive(false);
    }

    private AbstractMinigame GetMinigameInstance(Game idGame)
    {
        string miniGame = Enum.GetName(typeof(Game), idGame);
        return loader.GetModel(miniGame).GetComponent<AbstractMinigame>();
    }

    public void StartGame()
    {
        miniGame.StartGame();
    }

    public void GoToMainMenu()
    {
        loadingGame.SetActive(true);
        //musicPlayer.Play("minigames", 0.5f);
        if (miniGame == null)
            return;
        Destroy(miniGame.gameObject);
    }
}
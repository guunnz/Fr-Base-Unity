using AddressablesSystem;
using Architecture.Injector.Core;
using Data;
using LocalizationSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractMinigame : MonoBehaviour
{
    protected Game idGame;
    protected MinigameManager minigameManager;
    protected IGameData gameData;
    protected ILanguage language;
    public void SetMinigameManager(MinigameManager minigameManager)
    {
        this.minigameManager = minigameManager;
    }

    public virtual void StartGame()
    {

    }

    public virtual void UserLeave()
    {

    }

    public virtual void UserEnds()
    {

    }

    public virtual void OpponentWin()
    {

    }

    public virtual void PlayerFinish()
    {

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using UniRx;
using UnityEngine.UI;
using Data;
using PlayerMovement;

public class RoomTransition : ViewNode
{
    public string outPort;
    IViewManager viewManager;
    IGameData gameData;

    protected override void OnInit()
    {
        gameData = Injection.Get<IGameData>();
        viewManager = Injection.Get<IViewManager>();

    }

    void OnEnable()
    {
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        yield return null;
        viewManager ??= Injection.Get<IViewManager>();
        viewManager.DebugGetOut(outPort);
    }

    protected override void OnShow()
    {
        RemotePlayersPool.Current?.ClearRemotesPool();

    }

    protected override void OnHide()
    {
        RemotePlayersPool.Current?.ClearRemotesPool();
    }
}

using AddressablesSystem;
using Architecture.Injector.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MinigameMenuItem : MonoBehaviour
{
    private MinigameInformation minigameInfo;
    private Game idGame;
    private Sprite SelectedSprite;
    private Sprite UnselectedSprite;
    private Image ImageComponent;
    private MinigameMenuManager minigameMenu;
    private ILoader loader;

    public IEnumerator Initialize(MinigameInformation _minigameInfo, MinigameMenuManager _minigameMenu)
    {
        ImageComponent = GetComponent<Image>();
        ImageComponent.enabled = false;
        idGame = _minigameInfo.idGame;
        string miniGameName = Enum.GetName(typeof(Game), idGame);
        loader = Injection.Get<ILoader>();
        loader.LoadItem(new LoaderItemSprite(miniGameName + "_Sprite_Selected"));
        loader.LoadItem(new LoaderItemSprite(miniGameName + "_Sprite_Unselected"));
        this.minigameInfo = _minigameInfo;
        this.minigameMenu = _minigameMenu;

        LoaderAbstractItem spriteSelected = loader.GetItem(miniGameName + "_Sprite_Selected");

        LoaderAbstractItem spriteUnselected = loader.GetItem(miniGameName + "_Sprite_Unselected");

        while (spriteSelected == null || spriteUnselected == null || spriteUnselected.State != LoaderItemState.SUCCEED || spriteSelected.State != LoaderItemState.SUCCEED)
        {
            yield return null;
        }

        LoaderItemSprite selectedSpriteAux = spriteSelected as LoaderItemSprite;
        LoaderItemSprite unselectedSpriteAux = spriteUnselected as LoaderItemSprite;
        SelectedSprite = selectedSpriteAux.GetSprite();
        UnselectedSprite = unselectedSpriteAux.GetSprite();


        this.minigameMenu.SelectEvent.AddListener(delegate { this.SetSpriteImage(); });
        this.minigameMenu.SelectEvent.AddListener(delegate { this.SetMovement(); });
        //this.transform.localPosition = new Vector3(this.transform.localPosition.x, IsSelected() ? 0 : -48f, this.transform.localPosition.z);
        SetSpriteImage();
        yield return null;
        SetMovement();
        ImageComponent.enabled = true;
    }


    public Game GetIdGame()
    {
        return idGame;
    }

    public void SetSpriteImage()
    {
        ImageComponent.sprite = IsSelected() ? SelectedSprite : UnselectedSprite;
    }

    public void SetMovement()
    {
        this.transform.DOLocalMove(new Vector3(this.transform.localPosition.x, IsSelected() ? 0 : -48f, this.transform.localPosition.z), 0.3f);
    }

    private bool IsSelected()
    {
        Game currentMinigame = this.minigameMenu.GetCurrentMinigameSelected();

        return this.idGame == currentMinigame;
    }

    public void SelectDirectly()
    {
        this.minigameMenu.Select(this.minigameInfo);
    }


    public void Select()
    {
        if (IsSelected() && idGame != Game.ComingSoon)
        {
            this.minigameMenu.Select(this.minigameInfo);
        }
        else
        {
            int x = this.idGame - minigameMenu.GetCurrentMinigameSelected();

            if (x != 0)
            {
                this.minigameMenu.ChangeMinigame(x);
            }
        }
    }
}
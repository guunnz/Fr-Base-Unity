using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AddressablesSystem;
using Architecture.Injector.Core;

[RequireComponent(typeof(SpriteRenderer))]
public class Load2DSpriteRenderer : MonoBehaviour, IReceiveLoadedItem
{
    public enum LOADING_STATE { NONE, LOADING, COMPLETE, FAILED };

    [SerializeField] private GameObject loaderAnimation;

    public delegate void LoadingImageComplete(bool flag, Sprite sprite);
    public event LoadingImageComplete OnLoadingImageComplete;

    public SpriteRenderer spriteRenderer { get; private set; }

    private string nameAsset;
    public LOADING_STATE loadingState { get; private set; }

    private ILoader loader;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        loader = Injection.Get<ILoader>();
        loadingState = LOADING_STATE.NONE;
    }

    public void Load(string nameAsset, bool hideSpriteOnLoad = false)
    {
        loadingState = LOADING_STATE.LOADING;
        this.nameAsset = nameAsset;
        ShowLoader();

        loader.Suscribe(this, nameAsset);
        loader.LoadItem(new LoaderItemSprite(nameAsset));

        if (hideSpriteOnLoad)
        {
            spriteRenderer.enabled = false;
        }
    }

    public void ShowLoader()
    {
        if (loaderAnimation != null)
        {
            loaderAnimation.SetActive(true);
            SetAlphaImage(0);
        }
    }

    public void HideLoader()
    {
        if (loaderAnimation != null)
        {
            loaderAnimation.SetActive(false);
            SetAlphaImage(1);
        }
    }

    public void Destroy()
    {
        loadingState = LOADING_STATE.NONE;
        SetAlphaImage(1);
        if (nameAsset != null)
        {
            loader.Unsuscribe(this, nameAsset);
        }
    }

    void SetAlphaImage(float alpha)
    {
        Color currentColor = spriteRenderer.color;
        currentColor.a = alpha;
        spriteRenderer.color = currentColor;
    }

    public void ReceiveLoadedItem(LoaderAbstractItem item)
    {
        if (item.State == LoaderItemState.SUCCEED)
        {
            if (!nameAsset.Equals(item.Id))
            {
                //Prevent to receive another snapshot
                return;
            }
            loader.Unsuscribe(this, nameAsset);

            LoaderItemSprite itemSprite = item as LoaderItemSprite;
            if (itemSprite != null)
            {
                HideLoader();

                spriteRenderer.enabled = true;
                Sprite sprite = itemSprite.GetSprite();
                spriteRenderer.sprite = sprite;

                loadingState = LOADING_STATE.COMPLETE;
                if (OnLoadingImageComplete != null)
                {
                    OnLoadingImageComplete(true, sprite);
                }
            }
        }
        else
        {
            loadingState = LOADING_STATE.FAILED;

            if (OnLoadingImageComplete != null)
            {
                OnLoadingImageComplete(false, null);
            }
        }
    }
}

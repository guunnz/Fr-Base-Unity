using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AddressablesSystem;
using Architecture.Injector.Core;
using System;

public class Load2DObject : MonoBehaviour, IReceiveLoadedItem
{
    [SerializeField] protected GameObject imageContainer;
    [SerializeField] private GameObject loaderAnimation;

    public delegate void LoadingImageComplete(bool flag, Sprite sprite);
    public event LoadingImageComplete OnLoadingImageComplete;

    public GameObject GetImageContainer
    {
        get { return imageContainer; }
    }

    private string nameAsset;
    public bool IsLoaded { get; private set; }

    private Image imageHolder;
    private ILoader loader;

    void Awake()
    {
        loader = Injection.Get<ILoader>();
        imageHolder = imageContainer.GetComponent<Image>();
        IsLoaded = false;
    }

    void Start()
    {
        if (loader == null)
        {
            loader = Injection.Get<ILoader>();
        }
    }

    public void Load(string nameAsset)
    {
        IsLoaded = false;
        this.nameAsset = nameAsset;
        ShowLoader();

        loader.Suscribe(this, nameAsset);
        loader.LoadItem(new LoaderItemSprite(nameAsset));
    }

    public void ShowLoader()
    {
        if (loaderAnimation != null)
        {
            loaderAnimation.SetActive(true);
        }
        SetAlphaImage(0);
    }

    public void HideLoader()
    {
        if (loaderAnimation != null)
        {
            loaderAnimation.SetActive(false);
        }
        SetAlphaImage(1);
    }

    public void Destroy()
    {
        SetAlphaImage(1);
        if (nameAsset != null)
        {
            loader.Unsuscribe(this, nameAsset);
        }
    }

    void SetAlphaImage(float alpha)
    {
        Color currentColor = imageHolder.color;
        currentColor.a = alpha;
        imageHolder.color = currentColor;
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

                Sprite sprite = itemSprite.GetSprite();
                imageContainer.GetComponent<Image>().sprite = sprite;
                IsLoaded = true;
                if (OnLoadingImageComplete != null)
                {
                    OnLoadingImageComplete(true, sprite);
                }
            }
        }
        else
        {
            if (OnLoadingImageComplete != null)
            {
                OnLoadingImageComplete(false, null);
            }
        }
    }
}

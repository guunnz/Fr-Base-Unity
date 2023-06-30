using System.Collections;
using System.Collections.Generic;
using Data.Catalog;
using TMPro;
using UI.ScrollView;
using UnityEngine;
using UnityEngine.UI;
using System;
using Data.Bag;
using Architecture.Injector.Core;
using Data;

[RequireComponent(typeof(Load2DObject))]
public class UICatalogRoomsCardController : UIAbstractCardController
{
    [SerializeField] protected Image imgDiamond;
    [SerializeField] protected Image imgCoin;
    [SerializeField] protected TextMeshProUGUI txtPrice;
    [SerializeField] protected Image imgInventory;

    private Load2DObject load2dObject;
    public GenericCatalogItem ObjCat { get; private set; }

    protected Action<EventType, GenericCatalogItem, UIAbstractCardController> callback;
    private GenericBag bagRooms;

    public bool ShowPrice { get; private set; }

    protected virtual void Awake()
    {
        load2dObject = GetComponent<Load2DObject>();
        bagRooms = Injection.Get<IGameData>().GetBagByItemType(ItemType.ROOM);
    }

    public override void SetUpCard(System.Object itemData, Action<EventType, System.Object, UIAbstractCardController> callback)
    {
        ObjCat = (GenericCatalogItem)itemData;
        if (ObjCat == null)
        {
            return;
        }

        load2dObject.Load(ObjCat.NamePrefab);

        UpdateGems();

        this.callback = callback;
    }

    void UpdateGems()
    {
        ShowPrice = false;
        GenericBagItem bagItem = bagRooms.GetItemById(ObjCat.IdItem);
        if (bagItem == null)
        {
            ShowPrice = true;
        }

        if (ObjCat.CurrencyType == CurrencyType.GEM_PRICE)
        {
            imgDiamond.gameObject.SetActive(ShowPrice);
            imgCoin.gameObject.SetActive(false);
        }
        else if (ObjCat.CurrencyType == CurrencyType.GOLD_PRICE)
        {
            imgDiamond.gameObject.SetActive(false);
            imgCoin.gameObject.SetActive(ShowPrice);
        }

        txtPrice.gameObject.SetActive(ShowPrice);
        imgInventory.gameObject.SetActive(!ShowPrice);

        txtPrice.text = ObjCat.CurrencyType == CurrencyType.GEM_PRICE ? ObjCat.GemsPrice.ToString() : ObjCat.GoldPrice.ToString();
    }

    public void MouseDown()
    {
        if (callback != null)
        {
            callback(EventType.MouseDown, ObjCat, this);
        }
    }

    public void MouseUp()
    {
        if (callback != null)
        {
            callback(EventType.MouseUp, ObjCat, this);
        }
    }

    public override void Destroy()
    {
        base.Destroy();
    }
}

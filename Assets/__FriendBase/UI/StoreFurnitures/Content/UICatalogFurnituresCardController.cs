using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.ScrollView;
using System;
using Data.Catalog;
using TMPro;
using Data.Bag;
using Architecture.Injector.Core;
using Data;

[RequireComponent(typeof(Load2DObject))]
public class UICatalogFurnituresCardController : UIAbstractCardController
{
    [SerializeField] protected Image imgDiamond;
    [SerializeField] protected Image imgCoin;
    [SerializeField] protected TextMeshProUGUI txtPrice;
    [SerializeField] protected Image imgInventory;
    [SerializeField] protected GameObject selectedBorder;
    [SerializeField] protected UICatalogLimitedEdition catalogLimitedEdition;

    private Load2DObject load2dObject;
    public GenericCatalogItem ObjCat { get; private set; }
    public bool IsInventoryItem { get; private set; }

    private GenericBag bagFurnitures;

    protected Action<EventType, GenericCatalogItem, UIAbstractCardController> callback;

    private bool isCardSelected;
    public bool IsCardSelected
    {
        get { return isCardSelected; }
        set
        {
            isCardSelected = value;
            ControlIfSelectable();
        }
    }

    protected virtual void Awake()
    {
        load2dObject = GetComponent<Load2DObject>();
    }

    public override void SetUpCard(System.Object itemData, Action<EventType, System.Object, UIAbstractCardController> callback)
    {
        ObjCat = (GenericCatalogItem)itemData;
        if (ObjCat == null)
        {
            return;
        }

        IsCardSelected = false;
        ControlIfSelectable();

        string namePrefab = ObjCat.GetNameFurniturePrefabUIByItem();
        load2dObject.Load(namePrefab);

        UpdateGems();
        catalogLimitedEdition.CheckLimitedEdition(ObjCat, IsInventoryItem, txtPrice.text);

        this.callback = callback;
    }

    void UpdateGems()
    {
        bagFurnitures = Injection.Get<IGameData>().GetBagByItemType(ObjCat.ItemType);
        GenericBagItem bagItem = bagFurnitures.GetItemById(ObjCat.IdItem);
        if (bagItem != null && bagItem.Amount > 0)
        {
            //We have this item in the inventory
            IsInventoryItem = true;
            txtPrice.text = bagItem.Amount.ToString();
        }
        else
        {
            //This item is not in the inventory => We show the price
            IsInventoryItem = false;
            txtPrice.text = ObjCat.CurrencyType == CurrencyType.GOLD_PRICE ? ObjCat.GoldPrice.ToString() : ObjCat.GemsPrice.ToString();
        }


        if (ObjCat.CurrencyType == CurrencyType.GEM_PRICE)
        {
            imgDiamond.gameObject.SetActive(!IsInventoryItem);
            imgCoin.gameObject.SetActive(false);
        }
        else if (ObjCat.CurrencyType == CurrencyType.GOLD_PRICE)
        {
            imgDiamond.gameObject.SetActive(false);
            imgCoin.gameObject.SetActive(!IsInventoryItem);
        }

        imgInventory.gameObject.SetActive(IsInventoryItem);
        txtPrice.gameObject.SetActive(true);
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
        IsCardSelected = true;
        ControlIfSelectable();
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    void ControlIfSelectable()
    {
        selectedBorder.gameObject.SetActive(isCardSelected);
    }
}

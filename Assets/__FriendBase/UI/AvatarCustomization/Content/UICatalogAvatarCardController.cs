using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.ScrollView;
using System;
using Data.Catalog;
using Data;
using Data.Bag;
using Architecture.Injector.Core;
using TMPro;
using AvatarCustomization;

[RequireComponent(typeof(Load2DObject))]
public class UICatalogAvatarCardController : UIAbstractCardController
{
    protected Action<EventType, AvatarGenericCatalogItem, UIAbstractCardController> callback;

    [SerializeField] protected Image selectedBorder;
    [SerializeField] protected Image selectedIcon;
    [SerializeField] protected Image imgDiamond;
    [SerializeField] protected Image imgCoin;
    [SerializeField] protected TextMeshProUGUI txtPrice;
    [SerializeField] protected UICatalogLimitedEdition catalogLimitedEdition;

    public bool UseBoobs { get; set; }
    public int IdColor { get; set; }

    private Load2DObject load2dObject;
    public AvatarGenericCatalogItem ObjCat { get; private set; }

    private IGameData gameData = Injection.Get<IGameData>();
    private Dictionary<ItemType, AvatarCustomizationRule> avatarCustomizationRules;

    public UICatalogAvatarManager.AVATAR_PANEL_TYPE AvatarPanelType { get; set; }

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
        avatarCustomizationRules = gameData.GetAvatarCustomizationRules();
    }

    public override void SetUpCard(System.Object itemData, Action<EventType, System.Object, UIAbstractCardController> callback)
    {
        ObjCat = (AvatarGenericCatalogItem)itemData;
        if (ObjCat == null)
        {
            return;
        }

        string namePrefab = ObjCat.GetNamePrefabUIByItem(UseBoobs, IdColor);
        load2dObject.Load(namePrefab);

        this.callback = callback;
        IsCardSelected = false;
        ControlIfSelectable();

        imgDiamond.gameObject.SetActive(false);
        imgCoin.gameObject.SetActive(false);
        txtPrice.gameObject.SetActive(false);
        if (AvatarPanelType == UICatalogAvatarManager.AVATAR_PANEL_TYPE.CHANGE_AVATAR)
        {
            GenericBagItem bagItem = gameData.GetBagByItemType(ObjCat.ItemType).GetItemById(ObjCat.IdItem);
            if (bagItem == null)
            {
                if (ObjCat.CurrencyType == CurrencyType.GOLD_PRICE)
                {
                    imgDiamond.gameObject.SetActive(false);
                    imgCoin.gameObject.SetActive(true);
                }
                else
                {
                    imgDiamond.gameObject.SetActive(true);
                    imgCoin.gameObject.SetActive(false);
                }
                txtPrice.gameObject.SetActive(true);
                txtPrice.text = ObjCat.CurrencyType == CurrencyType.GOLD_PRICE ? ObjCat.GoldPrice.ToString() : ObjCat.GemsPrice.ToString();
            }

            catalogLimitedEdition.CheckLimitedEdition(ObjCat, bagItem != null, "");
        }
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

    void ControlIfSelectable()
    {
        selectedBorder.gameObject.SetActive(isCardSelected);
        selectedIcon.gameObject.SetActive(isCardSelected);
    }
}

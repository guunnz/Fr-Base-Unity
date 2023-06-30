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
public class UICatalogPetsCardController : UIAbstractCardController
{
    protected Action<EventType, GenericCatalogItem, UIAbstractCardController> callback;

    [SerializeField] protected Image selectedBorder;
    [SerializeField] protected Image selectedIcon;
    [SerializeField] protected Image imgDiamond;
    [SerializeField] protected Image imgCoin;
    [SerializeField] protected TextMeshProUGUI txtPrice;
    [SerializeField] protected UICatalogLimitedEdition catalogLimitedEdition;

    private Load2DObject load2dObject;
    public GenericCatalogItem ObjCat { get; private set; }

    internal bool Obtained;

    private IGameData gameData = Injection.Get<IGameData>();
    private Dictionary<ItemType, AvatarCustomizationRule> avatarCustomizationRules;

    //public UICatalogAvatarManager.AVATAR_PANEL_TYPE AvatarPanelType { get; set; }

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

        string namePrefab = ObjCat.NamePrefab;

        load2dObject.Load(namePrefab + "_Profile");

        this.callback = callback;
        ControlIfSelectable();
        imgCoin.gameObject.SetActive(false);
        imgDiamond.gameObject.SetActive(false);
        txtPrice.gameObject.SetActive(false);

        GenericBagItem bagItem = gameData.GetBagByItemType(ObjCat.ItemType).GetItemById(ObjCat.IdItem);

        if (bagItem == null)
        {
            Obtained = false;
            if (ObjCat.CurrencyType == CurrencyType.GOLD_PRICE)
            {
                imgDiamond.gameObject.SetActive(false);
                imgCoin.gameObject.SetActive(true);
                txtPrice.text = ObjCat.GoldPrice.ToString();
            }
            else
            {
                imgCoin.gameObject.SetActive(false);
                imgDiamond.gameObject.SetActive(true);
                txtPrice.text = ObjCat.GemsPrice.ToString();
            }
            txtPrice.gameObject.SetActive(true);
        }
        else
        {
            Obtained = true;
        }

        catalogLimitedEdition.CheckLimitedEdition(ObjCat, Obtained, "");

        AvatarPetManager myAvatarPetManager = CurrentRoom.Instance.AvatarsManager.GetMyAvatar().avatarPetManager;

        if (myAvatarPetManager.CurrentPetId == ObjCat.IdItem)
        {
            IsCardSelected = true;
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
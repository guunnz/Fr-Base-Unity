using System.Collections;
using System.Collections.Generic;
using UI.ScrollView;
using UnityEngine;
using DebugConsole;
using Architecture.Injector.Core;
using Data;
using Data.Users;
using Data.Catalog;
using UI.TabController;
using System;

public class UICatalogAvatarColorScrollView : UIAbstractUIElementWithScroll
{
    [SerializeField] private UICatalogAvatarColorCardPool catalogAvatarColorCardPool;

    private IGameData gameData = Injection.Get<IGameData>();
    private Dictionary<ItemType, AvatarCustomizationRule> rules;

    public ItemType ItemType { get; private set; }

    public delegate void CardSelected(ColorCatalogItem element, UIAbstractCardController cardController);
    public event CardSelected OnCardSelected;

    private AvatarCustomizationData avatarCustomizationData;

    protected override void Awake()
    {
        base.Awake();
        rules = gameData.GetAvatarCustomizationRules();
    }

    public void ShowObjects(ItemType itemType)
    {
        ItemType = itemType;
        base.ShowObjects();
        RefreshCardsSelectionState();
    }

    public void SetAvatarCustomizationData(AvatarCustomizationData avatarCustomizationData)
    {
        this.avatarCustomizationData = avatarCustomizationData;
    }

    //---------------------------------------------------------------------
    //---------------------------------------------------------------------
    //-----------------------  S C R O L L   V I E W   --------------------
    //---------------------------------------------------------------------
    //---------------------------------------------------------------------

    protected override void ReturnObjectToPool(UIAbstractCardController card)
    {
        UICatalogAvatarColorCardController cardController = card as UICatalogAvatarColorCardController;
        if (card != null)
        {
            catalogAvatarColorCardPool.ReturnToPool(cardController);
        }
        else
        {
            Injection.Get<IDebugConsole>().ErrorLog("UI_CatalogAvatarColorScrollView:ReturnObjectToPool", "Error Casting Object", "");
        }
    }

    protected override UIAbstractCardController GetNewCard()
    {
        UIAbstractCardController newCard = catalogAvatarColorCardPool.Get();
        return newCard;
    }

    public override List<System.Object> GetListElements()
    {
        //Get colors available
        int[] colorsAvalable = rules[ItemType].ColorIdsAvailable;

        List<System.Object> listItems = new List<System.Object>();

        GenericCatalog catalog = gameData.GetCatalogByItemType(ItemType.COLOR);
        int amount = catalog.GetAmountItems();

        for (int i = 0; i < amount; i++)
        {
            GenericCatalogItem genericCatalogItem = catalog.GetItemByIndex(i);
            if (Array.IndexOf(colorsAvalable, genericCatalogItem.IdItem) >=0 )
            {
                listItems.Add(genericCatalogItem);
            }
        }

        return listItems;
    }

    protected override void MouseDownElement(System.Object element, UIAbstractCardController cardController)
    {
    }

    protected override void MouseUpElement(System.Object element, UIAbstractCardController cardController)
    {
        ColorCatalogItem colorCatalogItem = element as ColorCatalogItem;
        if (colorCatalogItem != null)
        {
            if (OnCardSelected != null)
            {
                OnCardSelected(colorCatalogItem, cardController);
            }
            RefreshCardsSelectionState();
        }
    }

    public void RefreshCardsSelectionState()
    {
        int idItemSelected = avatarCustomizationData.GetDataUnit(ItemType).ColorObjCat.IdItem;
        int amount = GetAmountElements();
        for (int i = 0; i < amount; i++)
        {
            UICatalogAvatarColorCardController currenCard = GetCardByIndex(i).GetComponent<UICatalogAvatarColorCardController>();
            currenCard.IsCardSelected = (currenCard.ObjCat.IdItem == idItemSelected);
        }
    }
}

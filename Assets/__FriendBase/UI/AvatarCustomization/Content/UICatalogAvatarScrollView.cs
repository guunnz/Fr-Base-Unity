using System.Collections;
using System.Collections.Generic;
using UI.ScrollView;
using UnityEngine;
using DebugConsole;
using Architecture.Injector.Core;
using Data;
using Data.Catalog;
using UI.TabController;
using Data.Users;
using AvatarCustomization;
using Data.Bag;

public class UICatalogAvatarScrollView : UIAbstractUIElementWithScroll
{
    [SerializeField] private UICatalogAvatarCardPool catalogAvatarCardPool;

    private AvatarCustomizationData avatarCustomizationData;

    public ItemType ItemType { get; private set; }

    public delegate void CardSelected(AvatarGenericCatalogItem element, UIAbstractCardController cardController);
    public event CardSelected OnCardSelected;

    private RectTransform scrollViewRectTransform; //Reference to viewScroll rectTransform
    private IGameData gameData = Injection.Get<IGameData>();
    private Dictionary<ItemType, AvatarCustomizationRule> rules;

    private int idColor; // We keep the idColor or the cases that we can discard color (ex: lips)

    private UICatalogAvatarManager.AVATAR_PANEL_TYPE avatarPanelType;

    protected override void Awake()
    {
        base.Awake();
        rules = gameData.GetAvatarCustomizationRules();
        scrollViewRectTransform = dragBagManager.GetComponent<RectTransform>();
    }

    public void SetAvatarPanelType(UICatalogAvatarManager.AVATAR_PANEL_TYPE avatarPanelType)
    {
        this.avatarPanelType = avatarPanelType;
    }

    public void SetAvatarCustomizationData(AvatarCustomizationData avatarCustomizationData)
    {
        this.avatarCustomizationData = avatarCustomizationData;
    }

    public void ShowObjects(ItemType itemType, int idColor)
    {
        ItemType = itemType;
        this.idColor = idColor;
        ResetPosition();

        base.ShowObjects();
    }

    public void ChangeColor(int idColor)
    {
        bool refreshAllItems = false;
        if (rules[ItemType].CanDisableColor)
        {
            if ( (this.idColor == 0 && idColor>0) || (this.idColor >0 && idColor == 0) )
            {
                refreshAllItems = true;
            }
        }

        this.idColor = idColor;

        if (refreshAllItems)
        {
            base.ShowObjects();
        }
    }

    public void EnlargeAnchorsScrollView(bool flagColorsActive)
    {
        //If no colors are selectables, we make the area of cards bigger
        if (flagColorsActive)
        {
            scrollViewRectTransform.anchorMin = new Vector2(0,0.2f);
        }
        else
        {
            scrollViewRectTransform.anchorMin = new Vector2(0, 0.05f);
        }
    }

    //---------------------------------------------------------------------
    //---------------------------------------------------------------------
    //-----------------------  S C R O L L   V I E W   --------------------
    //---------------------------------------------------------------------
    //---------------------------------------------------------------------

    protected override void ReturnObjectToPool(UIAbstractCardController card)
    {
        UICatalogAvatarCardController cardController = card as UICatalogAvatarCardController;
        if (card != null)
        {
            catalogAvatarCardPool.ReturnToPool(cardController);
        }
        else
        {
            Injection.Get<IDebugConsole>().ErrorLog("UI_CatalogAvatarManager:ReturnObjectToPool", "Error Casting Object", "");
        }
    }

    protected override UIAbstractCardController GetNewCard()
    {
        UICatalogAvatarCardController newCard = catalogAvatarCardPool.Get();
        newCard.UseBoobs = avatarCustomizationData.IsBoobsActive();
        newCard.IdColor = idColor;
        newCard.AvatarPanelType = avatarPanelType;

        return newCard;
    }

    protected override void OnAddCard(UIAbstractCardController card)
    {
        UICatalogAvatarCardController currentCard = card as UICatalogAvatarCardController;
        if (currentCard!=null)
        {
            if (avatarCustomizationData.GetDataUnit(currentCard.ObjCat.ItemType).AvatarObjCat!=null)
            {
                currentCard.IsCardSelected = (currentCard.ObjCat.IdItem == avatarCustomizationData.GetDataUnit(currentCard.ObjCat.ItemType).AvatarObjCat.IdItem);
            }
            else
            {
                currentCard.IsCardSelected = false;
            }
        }
    }

    public override List<System.Object> GetListElements()
    {
        List<GenericCatalogItem> listObjCat = new List<GenericCatalogItem>();

        GenericCatalog catalog = gameData.GetCatalogByItemType(ItemType);
        int amount = catalog.GetAmountItems();

        for (int i = 0; i < amount; i++)
        {
            GenericCatalogItem objCat = catalog.GetItemByIndex(i);
            if (objCat.ActiveInCatalog)
            {
                listObjCat.Add(objCat);
            }
            else
            {
                //Not in catalog, lets check if we have the item in the inventory
                GenericBagItem bagItem = gameData.GetBagByItemType(objCat.ItemType).GetItemById(objCat.IdItem);
                if (bagItem!=null)
                {
                    listObjCat.Add(objCat);
                }
            }
        }

        GenericCatalogItem.SortGenericCatalogWithLimitedEdition(listObjCat);

        return new List<System.Object>(listObjCat);
    }

    protected override void MouseDownElement(System.Object element, UIAbstractCardController cardController)
    {
    }

    protected override void MouseUpElement(System.Object element, UIAbstractCardController cardController)
    {
        AvatarGenericCatalogItem catalogItem = element as AvatarGenericCatalogItem;
        if (catalogItem!=null)
        {
            if (OnCardSelected!=null)
            {
                OnCardSelected(catalogItem, cardController);
            }
            RefreshCardsSelectionState();
        }
    }

    public void RefreshCardsSelectionState()
    {
        AvatarGenericCatalogItem catalogItem = avatarCustomizationData.GetDataUnit(ItemType).AvatarObjCat;
        int idItemSelected = -1;
        if (catalogItem!=null)
        {
            idItemSelected = catalogItem.IdItem;
        }
        
        int amount = GetAmountElements();
        for (int i = 0; i < amount; i++)
        {
            UICatalogAvatarCardController currenCard = GetCardByIndex(i).GetComponent<UICatalogAvatarCardController>();
            currenCard.IsCardSelected = (currenCard.ObjCat.IdItem == idItemSelected);
        }
    }
}
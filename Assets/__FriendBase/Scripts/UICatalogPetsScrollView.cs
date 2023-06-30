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
using Data.Bag;

public class UICatalogPetsScrollView : UIAbstractUIElementWithScroll
{
    [SerializeField] private UICatalogPetsCardPool catalogPetsCardPool;

    private IGameData gameData = Injection.Get<IGameData>();

    public ItemType ItemType { get; private set; }

    public delegate void CardSelected(GenericCatalogItem element, UIAbstractCardController cardController);
    public event CardSelected OnCardSelected;

    private AvatarCustomizationData avatarCustomizationData;

    private AvatarPetManager myAvatarPetManager;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        ShowObjects(ItemType.PETS);
    }

    private void Start()
    {

        myAvatarPetManager = CurrentRoom.Instance.AvatarsManager.GetMyAvatar().avatarPetManager;

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
        UICatalogPetsCardController cardController = card as UICatalogPetsCardController;
        if (card != null)
        {
            catalogPetsCardPool.ReturnToPool(cardController);
        }
        else
        {
            Injection.Get<IDebugConsole>().ErrorLog("UI_CatalogPetsScrollView:ReturnObjectToPool", "Error Casting Object", "");
        }
    }

    protected override UIAbstractCardController GetNewCard()
    {
        UIAbstractCardController newCard = catalogPetsCardPool.Get();
        return newCard;
    }

    public override List<System.Object> GetListElements()
    {
        List<GenericCatalogItem> listObjCat = new List<GenericCatalogItem>();

        GenericCatalog catalog = gameData.GetCatalogByItemType(ItemType.PETS);
        int amount = catalog.GetAmountItems();

        for (int i = 0; i < amount; i++)
        {
            GenericCatalogItem genericCatalogItem = catalog.GetItemByIndex(i);
            if (genericCatalogItem != null)
            {
                if (genericCatalogItem.ActiveInCatalog)
                {
                    listObjCat.Add(genericCatalogItem);
                }
                else
                {
                    //Not in catalog, lets check if we have the item in the inventory
                    GenericBagItem bagItem = gameData.GetBagByItemType(genericCatalogItem.ItemType).GetItemById(genericCatalogItem.IdItem);
                    if (bagItem != null)
                    {
                        listObjCat.Add(genericCatalogItem);
                    }
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
        GenericCatalogItem catalogItem = element as GenericCatalogItem;
        if (catalogItem != null)
        {
            if (OnCardSelected != null)
            {
                OnCardSelected(catalogItem, cardController);
            }
            RefreshCardsSelectionState(cardController);
        }
    }

    public void RefreshCardsSelectionState(UIAbstractCardController currentCard)
    {
        int amount = GetAmountElements();
        for (int i = 0; i < amount; i++)
        {
            UICatalogPetsCardController currenCard = GetCardByIndex(i).GetComponent<UICatalogPetsCardController>();
            if (currenCard != currentCard)
            {
                currenCard.IsCardSelected = false;
            }
        }
    }

    public void RefreshCardsSelectionState()
    {
        int amount = GetAmountElements();
        for (int i = 0; i < amount; i++)
        {
            UICatalogPetsCardController currenCard = GetCardByIndex(i).GetComponent<UICatalogPetsCardController>();


            if (myAvatarPetManager == null || myAvatarPetManager.CurrentPetId != currenCard.ObjCat.IdItem)
                currenCard.IsCardSelected = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Data.Bag;
using Data.Catalog;
using DebugConsole;
using UI.ScrollView;
using UnityEngine;

public class UICatalogFurnituresScrollView : UIAbstractUIElementWithScroll
{
    [SerializeField] private UICatalogFurnituresCardPool catalogFurnituresCardPool;
    [SerializeField] private UIStoreFurnitureNoItems noItemsPanel;
    
    public ItemType ItemType { get; private set; }

    public delegate void CardSelected(GenericCatalogItem element, UIAbstractCardController cardController);
    public event CardSelected OnCardSelected;
    private IGameData gameData = Injection.Get<IGameData>();

    public void ShowObjects(ItemType itemType)
    {
        ItemType = itemType;
        ResetPosition();

        base.ShowObjects();
    }

    //---------------------------------------------------------------------
    //---------------------------------------------------------------------
    //-----------------------  S C R O L L   V I E W   --------------------
    //---------------------------------------------------------------------
    //---------------------------------------------------------------------

    protected override void ReturnObjectToPool(UIAbstractCardController card)
    {
        UICatalogFurnituresCardController cardController = card as UICatalogFurnituresCardController;
        if (card != null)
        {
            catalogFurnituresCardPool.ReturnToPool(cardController);
        }
        else
        {
            Injection.Get<IDebugConsole>().ErrorLog("UICatalogFurnituresScrollView:ReturnObjectToPool", "Error Casting Object", "");
        }
    }

    protected override UIAbstractCardController GetNewCard()
    {
        UICatalogFurnituresCardController newCard = catalogFurnituresCardPool.Get();
        
        return newCard;
    }

    public override List<System.Object> GetListElements()
    {
        noItemsPanel.gameObject.SetActive(false);

        if (ItemType != ItemType.FURNITURES_INVENTORY)
        {
            return GetCatalogFurnituresList();
        }
        else
        {
            return GetFurnituresInventoryList();
        }
    }

    List<System.Object> GetFurnituresInventoryList()
    {
        List<System.Object> listItems = new List<System.Object>();

        foreach (ItemType currentItemType in GameData.RoomItemsType)
        {
            GenericBag bag = gameData.GetBagByItemType(currentItemType);
            int amount = bag.GetAmountItems();

            for (int i = 0; i < amount; i++)
            {
                GenericBagItem bagItem = bag.GetItemByIndex(i);
                if (bagItem.Amount>0)
                {
                    listItems.Add(bagItem.ObjCat);
                }
            }
        }

        noItemsPanel.gameObject.SetActive(listItems.Count==0);

        return listItems;
    }

    List<System.Object> GetCatalogFurnituresList()
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
                if (bagItem != null && bagItem.Amount>0)
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
        GenericCatalogItem catalogItem = element as GenericCatalogItem;
        if (catalogItem != null)
        {
            if (OnCardSelected != null)
            {
                OnCardSelected(catalogItem, cardController);
            }
        }
        ResetCardsSelectionState();
    }

    public void ResetCardsSelectionState()
    {
        int amount = GetAmountElements();
        for (int i = 0; i < amount; i++)
        {
            UICatalogFurnituresCardController currenCard = GetCardByIndex(i).GetComponent<UICatalogFurnituresCardController>();
            currenCard.IsCardSelected = false;
        }
    }
}

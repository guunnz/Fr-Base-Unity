using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Data.Bag;
using Data.Catalog;
using DebugConsole;
using UI.ScrollView;
using UnityEngine;

public class UICatalogRoomsScrollView : UIAbstractUIElementWithScroll
{
    [SerializeField] private UICatalogRoomsCardPool catalogRoomsCardPool;

    public ItemType ItemType { get; } = ItemType.ROOM;

    public delegate void CardSelected(GenericCatalogItem element, UIAbstractCardController cardController);
    public event CardSelected OnCardSelected;
    private IGameData gameData = Injection.Get<IGameData>();

    public override void ShowObjects()
    {
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
        UICatalogRoomsCardController cardController = card as UICatalogRoomsCardController;
        if (card != null)
        {
            catalogRoomsCardPool.ReturnToPool(cardController);
        }
        else
        {
            Injection.Get<IDebugConsole>().ErrorLog("UICatalogRoomsScrollView:ReturnObjectToPool", "Error Casting Object", "");
        }
    }

    protected override UIAbstractCardController GetNewCard()
    {
        UICatalogRoomsCardController newCard = catalogRoomsCardPool.Get();

        return newCard;
    }

    public override List<System.Object> GetListElements()
    {
        List<System.Object> listItems = new List<System.Object>();

        GenericCatalog catalog = gameData.GetCatalogByItemType(ItemType);
        int amount = catalog.GetAmountItems();

        for (int i = 0; i < amount; i++)
        {
            GenericCatalogItem roomItem = catalog.GetItemByIndex(i);
            listItems.Add(roomItem);
        }

        return listItems;
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
    }
}

using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data.Rooms;
using DebugConsole;
using UI.ScrollView;
using UnityEngine;

public class UISelectEventTypeScrollView : UIAbstractUIElementWithScroll
{
    [SerializeField] private UISelectEventTypeCardPool eventTypeCardPool;
    private List<SelectEventTypeData> listEventTypeData;

    public delegate void CardSelected(SelectEventTypeData element, UIAbstractCardController cardController);
    public event CardSelected OnCardSelected;

    public void ShowObjects(List<SelectEventTypeData> listEventTypeData)
    {
        this.listEventTypeData = listEventTypeData;
        base.ShowObjects();
    }

    //---------------------------------------------------------------------
    //---------------------------------------------------------------------
    //-----------------------  S C R O L L   V I E W   --------------------
    //---------------------------------------------------------------------
    //---------------------------------------------------------------------

    protected override void ReturnObjectToPool(UIAbstractCardController card)
    {
        UISelectEventTypeCardController cardController = card as UISelectEventTypeCardController;
        if (card != null)
        {
            eventTypeCardPool.ReturnToPool(cardController);
        }
        else
        {
            Injection.Get<IDebugConsole>().ErrorLog("UISelectEventTypeScrollView:ReturnObjectToPool", "Error Casting Object", "");
        }
    }

    protected override UIAbstractCardController GetNewCard()
    {
        UISelectEventTypeCardController newCard = eventTypeCardPool.Get();

        return newCard;
    }

    public override List<System.Object> GetListElements()
    {
        return GetList();
    }

    List<System.Object> GetList()
    {
        List<System.Object> listItems = new List<System.Object>();

        foreach (SelectEventTypeData eventTypeData in listEventTypeData)
        {
            listItems.Add(eventTypeData);
        }

        return listItems;
    }

    protected override void MouseDownElement(System.Object element, UIAbstractCardController cardController)
    {
    }

    protected override void MouseUpElement(System.Object element, UIAbstractCardController cardController)
    {
        if (OnCardSelected != null)
        {
            SelectEventTypeData selectEventTypeData = element as SelectEventTypeData;
            OnCardSelected(selectEventTypeData, cardController);
        }
    }
}

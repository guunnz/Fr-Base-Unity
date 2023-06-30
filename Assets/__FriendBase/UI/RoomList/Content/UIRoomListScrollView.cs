using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data.Rooms;
using DebugConsole;
using UI.ScrollView;
using UnityEngine;

public class UIRoomListScrollView : UIAbstractUIElementWithScroll
{
    [SerializeField] private UIRoomListCardPool roomListCardPool;

    public delegate void CardSelected(RoomInformation element, UIAbstractCardController cardController);
    public event CardSelected OnCardSelected;

    private List<RoomInformation> listRoomInformation;

    public void ShowObjects(List<RoomInformation> roomInformation)
    {
        listRoomInformation = roomInformation;
        base.ShowObjects();
    }

    //---------------------------------------------------------------------
    //---------------------------------------------------------------------
    //-----------------------  S C R O L L   V I E W   --------------------
    //---------------------------------------------------------------------
    //---------------------------------------------------------------------

    protected override void ReturnObjectToPool(UIAbstractCardController card)
    {
        UIRoomListCardController cardController = card as UIRoomListCardController;
        if (card != null)
        {
            roomListCardPool.ReturnToPool(cardController);
        }
        else
        {
            Injection.Get<IDebugConsole>().ErrorLog("UIRoomListScrollView:ReturnObjectToPool", "Error Casting Object", "");
        }
    }

    protected override UIAbstractCardController GetNewCard()
    {
        UIRoomListCardController newCard = roomListCardPool.Get();

        return newCard;
    }

    public override List<System.Object> GetListElements()
    {
        return GetTempRoomList();
    }

    List<System.Object> GetTempRoomList()
    {
        List<System.Object> listItems = new List<System.Object>();

        foreach (RoomInformation roomInformation in listRoomInformation)
        {
           
            listItems.Add(roomInformation);
        }

        return listItems;
    }

    protected override void MouseDownElement(System.Object element, UIAbstractCardController cardController)
    {
    }

    protected override void MouseUpElement(System.Object element, UIAbstractCardController cardController)
    {
        RoomInformation roomInformation = element as RoomInformation;
        if (roomInformation != null)
        {
            if (OnCardSelected != null)
            {
                OnCardSelected(roomInformation, cardController);
            }
        }
    }
}

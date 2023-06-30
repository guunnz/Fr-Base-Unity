using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Data.Rooms;
using Architecture.Injector.Core;
using UniRx;

public class UICancelEventPanel : AbstractUIPanel
{
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtBtnEndEvent;
    [SerializeField] protected TextMeshProUGUI txtBtnCancel;
    [SerializeField] protected GameObject loader;

    private RoomInformation roomInformation;

    public void Open(RoomInformation roomInformation)
    {
        base.Open();
        this.roomInformation = roomInformation;

        txtTitle.text = "Your event will end right now";
        txtBtnCancel.text = "Cancel";
        txtBtnEndEvent.text = "End Event";
    }

    public void Cancel()
    {
        CurrentRoom.Instance.GetRoomUIReferences().OpenRoomListPanelOnEvents();
        Close();
    }

    public void EndEvent()
    {
        loader.SetActive(true);
        Injection.Get<IRoomListEndpoints>().FinishEvent().Subscribe(room =>
        {
            CurrentRoom.Instance.GetRoomUIReferences().EnableBtnFurnitures();
            loader.SetActive(false);
            Close();
        });
    }
}

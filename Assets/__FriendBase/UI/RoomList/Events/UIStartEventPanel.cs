using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Data.Rooms;
using Data;
using Architecture.Injector.Core;
using UniRx;
using Newtonsoft.Json.Linq;
using Architecture.Injector.Core;

public class UIStartEventPanel : AbstractUIPanel
{
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtBtnGoBack;
    [SerializeField] protected TextMeshProUGUI txtBtnStart;

    [SerializeField] protected TextMeshProUGUI txtEventName;
    [SerializeField] protected TextMeshProUGUI txtEventHostName;
    [SerializeField] protected Load2DObject load2DObject;
    [SerializeField] protected UIHostEventPanel hostEventPanel;
    [SerializeField] protected GameObject loader;

    private RoomInformation roomInformation;
    private SelectEventTypeData selectEventTypeData;
    private IGameData gameData;

    public void Open(RoomInformation roomInformation, SelectEventTypeData selectEventTypeData)
    {
        base.Open();
        loader.SetActive(false);
        gameData = Injection.Get<IGameData>();

        this.roomInformation = roomInformation;
        this.selectEventTypeData = selectEventTypeData;

        txtTitle.text = "Ready to start?";
        txtBtnGoBack.text = "Go back";
        txtBtnStart.text = "Start";
        txtEventName.text = selectEventTypeData.TextEvent;
        txtEventHostName.text = gameData.GetUserInformation().UserName;

        load2DObject.Load(roomInformation.NamePrefab+"_Thumb");
    }

    public void OnGoBack()
    {
        Close();
        hostEventPanel.OpenWithSelectTypeSelected(roomInformation, selectEventTypeData);
    }

    public void OnCreateEvent()
    {
        loader.SetActive(true);
        Injection.Get<IRoomListEndpoints>().CreateEvent(selectEventTypeData.Index).Subscribe(room =>
        {
            loader.SetActive(false);
            if (room!=null)
            {
                CurrentRoom.Instance.UpdateEventType(selectEventTypeData.Index);
            }
            Close();
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Data.Rooms;
using Data;
using Architecture.Injector.Core;

public class UIHostEventPanel : AbstractUIPanel
{
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtDesc;
    [SerializeField] protected TextMeshProUGUI txtBtnCreate;
    [SerializeField] protected TextMeshProUGUI txtBtnCancel;
    [SerializeField] protected TextMeshProUGUI txtBtnSelectType;
    IGameData gameData;
    [SerializeField] protected UIStartEventPanel startEventPanel;
    [SerializeField] protected UISelectEventType selectEventTypePanel;
    [SerializeField] protected GameObject GuestPanelCannotCreateEvent;

    private RoomInformation roomInformation;
    private SelectEventTypeData selectEventTypeData;

    private void Awake()
    {
        this.gameData = Injection.Get<IGameData>();
    }

    public void Open(RoomInformation roomInformation)
    {
        if (gameData.IsGuest())
        {
            GuestPanelCannotCreateEvent.SetActive(true);
            return;
        }

        base.Open();
        this.roomInformation = roomInformation;

        txtTitle.text = language.GetTextByKey(LangKeys.NAV_HOST_AN_EVENT);
        txtDesc.text = language.GetTextByKey(LangKeys.EVENTS_OPEN_FOR_ONE_HOUR);
        txtBtnCancel.text = language.GetTextByKey(LangKeys.STORE_CANCEL);
        txtBtnCreate.text = language.GetTextByKey(LangKeys.EVENTS_CREATE);
        txtBtnSelectType.text = language.GetTextByKey(LangKeys.EVENTS_SELECT_TYPE_OF_EVENT);
    }

    public void OnCancel()
    {
        Close();
        CurrentRoom.Instance.GetRoomUIReferences().OpenRoomListPanelOnEvents();
    }

    public void OnCreate()
    {
        Close();
        if (selectEventTypeData == null)
        {
            selectEventTypeData = selectEventTypePanel.GetDefault();
        }

        startEventPanel.Open(roomInformation, selectEventTypeData);
    }

    public void OnSelectTypeEvent()
    {
        Close();
        selectEventTypePanel.Open(roomInformation);
    }

    public void OpenWithSelectTypeSelected(RoomInformation roomInformation, SelectEventTypeData selectEventTypeData)
    {
        this.selectEventTypeData = selectEventTypeData;
        Open(roomInformation);
        txtBtnSelectType.text = selectEventTypeData.TextEvent;
    }
}

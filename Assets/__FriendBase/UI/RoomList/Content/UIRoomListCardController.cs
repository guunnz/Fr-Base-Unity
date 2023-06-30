using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data.Rooms;
using LocalizationSystem;
using Data.Rooms;
using TMPro;
using UI.ScrollView;
using UnityEngine;
using UnityEngine.UI;
using Data;

public class UIRoomListCardController : UIAbstractCardController
{
    [SerializeField] protected TextMeshProUGUI txtRoomName;
    [SerializeField] protected TextMeshProUGUI txtAmountUsers;
    [SerializeField] protected Load2DObject load2DObject;
    [SerializeField] protected GameObject imgPeople;
    [SerializeField] protected GameObject imgNoPeople;
    [SerializeField] protected Image imgEventThumb;
    [SerializeField] protected TextMeshProUGUI txtEventName;
    [SerializeField] protected TextMeshProUGUI txtEventHostName;
    [SerializeField] protected Button btnEvent;
    [SerializeField] protected Button btnCard;
    [SerializeField] protected ILanguage language;
    [SerializeField] protected IGameData gameData;

    private RoomInformation roomInformation;
    protected Action<EventType, RoomInformation, UIAbstractCardController> callback;

    private void Awake()
    {
        language = Injection.Get<ILanguage>();
        gameData = Injection.Get<IGameData>();
    }

    public override void SetUpCard(System.Object itemData, Action<EventType, System.Object, UIAbstractCardController> callback)
    {
        roomInformation = (RoomInformation)itemData;
        if (roomInformation == null)
        {
            return;
        }

        switch (roomInformation.EventState)
        {
            case RoomInformation.EVENT_STATE.NONE:
                ShowPublicRoom();
                break;
            case RoomInformation.EVENT_STATE.EVENT:
                ShowEvent();
                break;
            case RoomInformation.EVENT_STATE.MY_EVENT_CARD_TO_END:
            case RoomInformation.EVENT_STATE.MY_EVENT_CARD_TO_HOST:
                ShowMyEvent();
                break;

        }

        this.callback = callback;
    }

    void ShowPublicRoom()
    {
        HideAll();

        btnCard.gameObject.SetActive(true);

        txtRoomName.gameObject.SetActive(true);
        txtRoomName.text = roomInformation.RoomName;

        load2DObject.GetImageContainer.SetActive(true);
        load2DObject.Load(roomInformation.NamePrefab + "_Thumb");

        ShowCurrentPeople();
    }

    void ShowCurrentPeople()
    {
        txtAmountUsers.gameObject.SetActive(true);
        txtAmountUsers.text = roomInformation.AmountUsers.ToString();

        imgPeople.SetActive(roomInformation.AmountUsers > 0);
        imgNoPeople.SetActive(roomInformation.AmountUsers == 0);

        if (roomInformation.AmountUsers > 0)
        {
            txtAmountUsers.color = GameObjectUtils.HexToColor("7A1602");
        }
        else
        {
            txtAmountUsers.color = GameObjectUtils.HexToColor("6B7280");
        }
    }

    void ShowEvent()
    {
        HideAll();

        btnCard.gameObject.SetActive(true);

        txtEventName.gameObject.SetActive(true);
        txtEventName.text = roomInformation.RoomType.Equals(RoomType.PUBLIC) ? roomInformation.RoomName : UISelectEventType.instance.GetEventTypeNameFromIndex(roomInformation.EventType);

        txtEventHostName.gameObject.SetActive(true);
        txtEventHostName.text = roomInformation.HostUserName;

        load2DObject.GetImageContainer.SetActive(true);
        load2DObject.Load(roomInformation.NamePrefab + "_Thumb");

        ShowCurrentPeople();
    }

    void ShowMyEvent()
    {
        HideAll();

        txtRoomName.gameObject.SetActive(true);
        language.SetTextByKey(txtRoomName, LangKeys.NAV_HOST_AN_EVENT);
        //txtRoomName.text = "Host an event";

        imgEventThumb.gameObject.SetActive(true);

        btnEvent.gameObject.SetActive(true);
        if (roomInformation.EventState == RoomInformation.EVENT_STATE.MY_EVENT_CARD_TO_HOST)
        {
            btnEvent.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = language.GetTextByKey(LangKeys.NAV_HOST);
        }
        else
        {
            btnEvent.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = language.GetTextByKey(LangKeys.NAV_END);
        }

        ShowCurrentPeople();
    }

    public void OnBtnEvent()
    {
        if (gameData.IsGuest())
        {
            FindObjectOfType<UIRoomListManager>().guestModal.SetActive(true);
            return;
        }
        if (roomInformation.EventState == RoomInformation.EVENT_STATE.MY_EVENT_CARD_TO_HOST)
        {
            CurrentRoom.Instance.GetRoomUIReferences().CloseRoomListPanel();
            CurrentRoom.Instance.GetRoomUIReferences().OpenHostEventPanel(roomInformation);
        }
        else
        {
            CurrentRoom.Instance.GetRoomUIReferences().CloseRoomListPanel();
            CurrentRoom.Instance.GetRoomUIReferences().OpenCancelEventPanel(roomInformation);
        }
    }

    void HideAll()
    {
        txtRoomName.gameObject.SetActive(false);
        txtAmountUsers.gameObject.SetActive(false);
        load2DObject.GetImageContainer.SetActive(false);
        imgPeople.gameObject.SetActive(false);
        imgNoPeople.gameObject.SetActive(false);

        txtEventName.gameObject.SetActive(false);
        txtEventHostName.gameObject.SetActive(false);
        btnEvent.gameObject.SetActive(false);
        btnCard.gameObject.SetActive(false);

        imgEventThumb.gameObject.SetActive(false);
    }

    public void MouseDown()
    {
        if (callback != null)
        {
            callback(EventType.MouseDown, roomInformation, this);
        }
    }

    public void MouseUp()
    {
        if (callback != null)
        {
            callback(EventType.MouseUp, roomInformation, this);
        }
    }
}

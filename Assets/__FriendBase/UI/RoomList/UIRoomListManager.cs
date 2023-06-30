using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Newtonsoft.Json.Linq;
using Data.Bag;
using Architecture.Injector.Core;
using Data;
using Data.Catalog;
using TMPro;
using Data.Rooms;
using System.Threading.Tasks;
using UI.ScrollView;
using LocalizationSystem;
using System.Linq;

public class UIRoomListManager : AbstractUIPanel
{
    enum ROOM_LIST_STATES { PLACES, EVENTS, CHOOSE_ROOM }

    public delegate void OnOpen();
    public static event OnOpen OnOpenEvent;

    public delegate void OnClose();
    public static event OnClose OnCloseEvent;

    [SerializeField] private UIRoomListScrollView scrollView;
    [SerializeField] private GameObject tabLeft;
    [SerializeField] private GameObject tabRight;
    [SerializeField] private GameObject chooseRoom;
    [SerializeField] public GameObject guestModal;
    [SerializeField] protected Button btnTabLeft;
    [SerializeField] protected Button btnTabRight;
    [SerializeField] protected TextMeshProUGUI txtTabLeft;
    [SerializeField] protected TextMeshProUGUI txtTabRight;
    [SerializeField] protected TextMeshProUGUI txtChooseRoom;
    [SerializeField] protected TextMeshProUGUI guestOnlyRegisteredUsersCan;
    [SerializeField] protected TextMeshProUGUI guestRegisterAndGetGems;
    [SerializeField] protected GameObject loader;

    [SerializeField] protected TextMeshProUGUI txtEventsMessageTitle;
    [SerializeField] protected TextMeshProUGUI txtEventsMessageDescription;
    IAnalyticsSender analyticsSender;


    private ROOM_LIST_STATES currentState;

    protected override void Start()
    {
        base.Start();
        analyticsSender = Injection.Get<IAnalyticsSender>();
        language.SetTextByKey(txtTabLeft, LangKeys.NAV_PLACES);
        language.SetTextByKey(txtTabRight, LangKeys.NAV_EVENTS);
        language.SetTextByKey(txtEventsMessageTitle, LangKeys.NAV_EVENTS_ARE_COMING_SOON);
        language.SetTextByKey(guestOnlyRegisteredUsersCan, LangKeys.GUEST_ONLY_GUEST_USERS_CAN_CREATE_EVENTS);
        guestRegisterAndGetGems.text = language.GetTextByKey(LangKeys.GUEST_REGISTER_GET_FREE_GEMS).Replace("[GEM ICON]", "<sprite=0>");
        txtEventsMessageDescription.text = "Renovated and cooler than ever.\nWe will notify you when it's ready :)";
        scrollView.OnCardSelected += OnCardSelected;
    }

    private void OnDestroy()
    {
        scrollView.OnCardSelected -= OnCardSelected;
    }

    public override void Open()
    {
        base.Open();
        OnClickPlaces();
    }

    void UpdateState(ROOM_LIST_STATES newState, RoomInformation roomInformation)
    {
        currentState = newState;
        switch (newState)
        {
            case ROOM_LIST_STATES.PLACES:
                SetStatePlaces();
                break;
            case ROOM_LIST_STATES.EVENTS:
                SetStateEvents();
                break;
            case ROOM_LIST_STATES.CHOOSE_ROOM:
                SetStateChooseRoom(roomInformation);
                break;
        }
    }

    public void SetStatePlaces()
    {
        currentState = ROOM_LIST_STATES.PLACES;
        chooseRoom.SetActive(false);
        tabRight.SetActive(false);
        tabLeft.SetActive(true);
        txtTabLeft.gameObject.SetActive(true);
        txtTabRight.gameObject.SetActive(true);
        btnTabLeft.gameObject.SetActive(true);
        btnTabRight.gameObject.SetActive(true);

        txtEventsMessageTitle.gameObject.SetActive(false);
        txtEventsMessageDescription.gameObject.SetActive(false);

        scrollView.ResetPosition();
        scrollView.gameObject.SetActive(false);
        loader.SetActive(true);

        Injection.Get<IRoomListEndpoints>().GetPublicRoomsList().Subscribe(listRooms =>
        {
            if (currentState == ROOM_LIST_STATES.PLACES)
            {
                scrollView.gameObject.SetActive(true);
                loader.SetActive(false);
                List<RoomInformation> roomInformation = listRooms;
                roomInformation.RemoveAll(x => x.IsEnable == false);
                scrollView.ShowObjects(roomInformation);
            }
        });
    }

    public void SetStateEvents()
    {
        currentState = ROOM_LIST_STATES.EVENTS;
        chooseRoom.SetActive(false);
        tabRight.SetActive(true);
        tabLeft.SetActive(false);
        txtTabLeft.gameObject.SetActive(true);
        txtTabRight.gameObject.SetActive(true);
        btnTabLeft.gameObject.SetActive(true);
        btnTabRight.gameObject.SetActive(true);

        txtEventsMessageTitle.gameObject.SetActive(false);
        txtEventsMessageDescription.gameObject.SetActive(false);

        scrollView.gameObject.SetActive(false);
        loader.SetActive(true);

        bool isMyRoom = CurrentRoom.Instance.IsMyRoom();

        Injection.Get<IRoomListEndpoints>().GetEventList(isMyRoom).Subscribe(listRooms =>
        {
            if (currentState == ROOM_LIST_STATES.EVENTS)
            {
                scrollView.gameObject.SetActive(true);
                loader.SetActive(false);
                //List<RoomInformation> roomInformationList = new List<RoomInformation>();
                //RoomInformation roomInformation = new RoomInformation(roomIdInstance: "1", roomName:"Mini", amountUsers:10, roomId:1, namePrefab:"Rio", isEnable:true, playerLimit:10, roomRank:10, roomType:"private", idUser:400, hostUserName:"Matutes", eventState:RoomInformation.EVENT_STATE.MY_EVENT_CARD_TO_HOST);
                //roomInformationList.Add(roomInformation);

                //RoomInformation roomInformation2 = new RoomInformation(roomIdInstance: "2", roomName: "Mini 2", amountUsers: 11, roomId: 2, namePrefab: "Supermarket", isEnable: true, playerLimit: 10, roomRank: 10, roomType: "private", idUser: 401, hostUserName: "Carlos", eventState: RoomInformation.EVENT_STATE.EVENT);
                //roomInformationList.Add(roomInformation2);
                //scrollView.ShowObjects(roomInformationList);

                scrollView.ShowObjects(listRooms);
            }
        });
    }

    public void LinkProviders()
    {
        CurrentRoom.Instance.LinkProvider();
    }

    public void SetStateChooseRoom(RoomInformation roomInformation)
    {
        currentState = ROOM_LIST_STATES.CHOOSE_ROOM;

        txtChooseRoom.text = "Choose " + roomInformation.RoomName;
        chooseRoom.SetActive(true);
        tabRight.SetActive(false);
        tabLeft.SetActive(false);
        txtTabLeft.gameObject.SetActive(false);
        txtTabRight.gameObject.SetActive(false);
        btnTabLeft.gameObject.SetActive(false);
        btnTabRight.gameObject.SetActive(false);

        txtEventsMessageTitle.gameObject.SetActive(false);
        txtEventsMessageDescription.gameObject.SetActive(false);

        scrollView.ResetPosition();
        scrollView.gameObject.SetActive(false);
        loader.SetActive(true);

        Injection.Get<IRoomListEndpoints>().GetPublicRoomsListInside(roomInformation.RoomId).Subscribe(listRooms =>
        {
            if (currentState == ROOM_LIST_STATES.CHOOSE_ROOM)
            {
                List<RoomInformation> roomInformation = listRooms;
                scrollView.gameObject.SetActive(true);
                loader.SetActive(false);
                scrollView.ShowObjects(roomInformation);
            }
        });
    }

    public void OnClickPlaces()
    {
        UpdateState(ROOM_LIST_STATES.PLACES, null);
    }

    public void OnClickEvents()
    {
        analyticsSender.SendAnalytics(AnalyticsEvent.OpensMoreGemsModal);
        UpdateState(ROOM_LIST_STATES.EVENTS, null);
    }

    void OnCardSelected(RoomInformation roomInformation, UIAbstractCardController cardController)
    {
        switch (currentState)
        {
            case ROOM_LIST_STATES.PLACES:
                SetStateChooseRoom(roomInformation);
                break;
            case ROOM_LIST_STATES.EVENTS:
                CurrentRoom.Instance.GoToNewRoom(roomInformation);
                break;
            case ROOM_LIST_STATES.CHOOSE_ROOM:
                CurrentRoom.Instance.GoToNewRoom(roomInformation);
                break;
        }
    }
}

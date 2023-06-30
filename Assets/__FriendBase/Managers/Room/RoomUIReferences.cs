using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using BurguerMenu.View;
using UnityEngine.SceneManagement;
using Data.Rooms;
using Data;
using Architecture.Injector.Core;

public class RoomUIReferences : MonoBehaviour
{
    public enum UI_TYPES
    {
        BtnAvatar,
        BtnChat,
        BtnBurger,
        BtnRooms,
        BrtnFurnitures,
        BtnHome,
        GemsHolder,
        GoldHolder
    }

    [SerializeField] public UIStoreManager UIStoreManager;
    [SerializeField] public UIStoreFurnituresManager uIStoreFurnitures;
    [SerializeField] public UIRoomListManager UIRoomListPanel;
    [SerializeField] public Button AvatarButton;
    [SerializeField] public ChatView.View.ChatView Chat;
    [SerializeField] public BurguerView BurguerMenu;
    [SerializeField] public Button BtnChat;
    [SerializeField] public Button BtnBurger;
    [SerializeField] public Button BtnRooms;
    [SerializeField] public Button BtnFurnitures;
    [SerializeField] public Button BtnHome;
    [SerializeField] public Button BtnAddGems;
    [SerializeField] public Button BtnGoMinigames;
    [SerializeField] public Button BtnGoMinigames2;
    [SerializeField] public Button BtnGoMinigamesModal;
    [SerializeField] public GameObject GemsHolder;
    [SerializeField] public GameObject GoldHolder;
    [SerializeField] public GameObject Loader;
    [SerializeField] public Camera Camera;
    [SerializeField] public UIHostEventPanel HostEventPanel;
    [SerializeField] public UICancelEventPanel CancelEventPanel;
    [SerializeField] public UIMsgPanelBanned MsgPanelBanned;

    private bool isPublicRoom;
    private bool isMyRoom;
    private int eventType;

    private IGameData gameData;

    IAnalyticsSender analyticsSender;
    public void Initialize(bool isPublicRoom, bool isMyRoom, int eventType)
    {
        this.isPublicRoom = isPublicRoom;
        this.isMyRoom = isMyRoom;
        this.eventType = eventType;

        UIStoreFurnituresManager.OnOpenEvent += OnOpenFurnituresStore;
        UIStoreFurnituresManager.OnCloseEvent += OnCloseFurnituresStore;
        ChatView.View.ChatView.OnCloseChat += OnCloseChat;

        BtnFurnitures.onClick.AddListener(OpenStoreFurnituresPanel);
        BtnAddGems.onClick.AddListener(OpenStorePanel);
        BtnGoMinigames.onClick.AddListener(GoMinigames);
        BtnGoMinigames2.onClick.AddListener(GoMinigames);
        BtnGoMinigamesModal.onClick.AddListener(GoMinigames);
        BtnRooms.onClick.AddListener(OpenRoomListPanel);
        BtnChat.onClick.AddListener(OnShowChat);
        BtnBurger.onClick.AddListener(OnShowBurguerMenu);
        BtnHome.onClick.AddListener(OnGoToMyHouse);

        ShowAll();
        OpenChatOnPublicRooms();
    }

    private void OpenChatOnPublicRooms()
    {
        if (this.isPublicRoom)
        {
            OnShowChat();
        }
    }

    private void Start()
    {
        analyticsSender = Injection.Get<IAnalyticsSender>();
    }

    public void ShowAll()
    {
        AvatarButton.gameObject.SetActive(true);
        BtnBurger.gameObject.SetActive(true);
        BtnRooms.gameObject.SetActive(true);
        BtnFurnitures.gameObject.SetActive(isMyRoom);
        BtnHome.gameObject.SetActive(!isMyRoom);
        GemsHolder.SetActive(true);
        GoldHolder.SetActive(true);
        BtnGoMinigames.gameObject.SetActive(true);
        UpdateEventType(eventType);
    }

    public void UpdateEventType(int eventType)
    {
        this.eventType = eventType;
        RefreshChatIconState();

        if (isMyRoom)
        {
            if (eventType >= 0)
            {
                DisableBtnFurnitures();
            }
            else
            {
                EnableBtnFurnitures();
            }
        }
    }

    public void RefreshChatIconState()
    {
        bool flagShow = false;
        if (isPublicRoom)
        {
            flagShow = true;
        }
        if (eventType >= 0)
        {
            flagShow = true;
        }
        //BtnChat.gameObject.SetActive(flagShow);
        Chat.gameObject.SetActive(flagShow);
    }

    public void HideAll()
    {
        AvatarButton.gameObject.SetActive(false);
        BtnChat.gameObject.SetActive(false);
        BtnBurger.gameObject.SetActive(false);
        BtnRooms.gameObject.SetActive(false);
        BtnFurnitures.gameObject.SetActive(false);
        BtnHome.gameObject.SetActive(false);
        GemsHolder.SetActive(false);
        GoldHolder.SetActive(false);
        BtnGoMinigames.gameObject.SetActive(false);
    }

    public void ShowUiElement(UI_TYPES uiElementType)
    {
        string nameUiElement = Enum.GetName(typeof(UI_TYPES), uiElementType);
        GameObject uiElement = GameObject.Find(nameUiElement);
        if (uiElement != null)
        {
            uiElement.SetActive(true);
        }
    }

    public void HideUiElement(UI_TYPES uiElementType)
    {
        string nameUiElement = Enum.GetName(typeof(UI_TYPES), uiElementType);
        GameObject uiElement = GameObject.Find(nameUiElement);
        if (uiElement != null)
        {
            uiElement.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        UIStoreFurnituresManager.OnOpenEvent -= OnOpenFurnituresStore;
        UIStoreFurnituresManager.OnCloseEvent -= OnCloseFurnituresStore;
        ChatView.View.ChatView.OnCloseChat -= OnCloseChat;

        BtnFurnitures.onClick.RemoveListener(OpenStoreFurnituresPanel);
        BtnAddGems.onClick.RemoveListener(OpenStorePanel);
        BtnRooms.onClick.RemoveListener(OpenRoomListPanel);
        BtnChat.onClick.RemoveListener(OnShowChat);
        BtnBurger.onClick.RemoveListener(OnShowBurguerMenu);
        BtnHome.onClick.RemoveListener(OnGoToMyHouse);
        BtnGoMinigames.onClick.RemoveListener(GoMinigames);
    }

    public void OpenCancelEventPanel(RoomInformation roomInformation)
    {
        CancelEventPanel.Open(roomInformation);
    }

    public void OpenHostEventPanel(RoomInformation roomInformation)
    {
        HostEventPanel.Open(roomInformation);
    }

    public void OpenStorePanel()
    {
        UIStoreManager.Open();
    }

    public void GoMinigames()
    {
        CurrentRoom.Instance.GoToMinigames();
    }

    public void CloseStorePanel()
    {
        UIStoreManager.Close();
    }

    public void OpenStoreFurnituresPanel()
    {
        analyticsSender.SendAnalytics(AnalyticsEvent.OpenEditHouse);
        uIStoreFurnitures.Open();
    }

    public void CloseStoreFurnituresPanel()
    {
        uIStoreFurnitures.Close();
    }

    public void OpenRoomListPanel()
    {
        gameData = Injection.Get<IGameData>();
        if (gameData.GetUserInformation().UserStatus.IsSuspended())
        {
            MsgPanelBanned.OpenWithSuspendedDescription(gameData.GetUserInformation().UserStatus.GetTimeSuspensionLeft(), null);
        }
        else
        {
            UIRoomListPanel.Open();
        }
    }

    public void OpenRoomListPanelOnEvents()
    {
        UIRoomListPanel.Open();
        UIRoomListPanel.SetStateEvents();
    }

    public void CloseRoomListPanel()
    {
        UIRoomListPanel.Close();
    }

    void OnOpenFurnituresStore()
    {
        HideAll();
    }

    void OnCloseFurnituresStore()
    {
        ShowAll();
    }

    private void OnCloseChat()
    {
        //Note (Andy): Since ShowUiElement method uses GameObject.Find() this won't return inactive objects.
        //"GemsHolder" and "GoldHolder" are re-enabled with the OnClick() editor event of the "Close Chat" button.

        //ShowUiElement(UI_TYPES.GoldHolder);
        //ShowUiElement(UI_TYPES.GemsHolder);
    }

    private void OnShowChat()
    {
        HideUiElement(UI_TYPES.GemsHolder);
        HideUiElement(UI_TYPES.GoldHolder);
        analyticsSender.SendAnalytics(AnalyticsEvent.OpensChatIcon);
        Chat.ShowChat();
    }

    private void OnShowBurguerMenu()
    {
        BurguerMenu.gameObject.SetActive(true);
    }

    private void OnGoToMyHouse()
    {
        CurrentRoom.Instance.GoToMyHouse();
    }

    public void DisableBtnFurnitures()
    {
        BtnFurnitures.interactable = false;
        Color color = BtnFurnitures.GetComponent<Image>().color;
        color.a = 0.5f;
        BtnFurnitures.GetComponent<Image>().color = color;
    }

    public void EnableBtnFurnitures()
    {
        BtnFurnitures.interactable = true;
        Color color = BtnFurnitures.GetComponent<Image>().color;
        color.a = 1f;
        BtnFurnitures.GetComponent<Image>().color = color;
    }
}
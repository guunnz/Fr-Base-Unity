using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Data.Catalog;
using Data.Users;
using UI.ScrollView;
using UI.TabController;
using UnityEngine;
using UniRx;
using Newtonsoft.Json.Linq;
using Data.Bag;

public class UIStoreFurnituresManager : AbstractUIPanel
{
    public delegate void OnOpen();
    public static event OnOpen OnOpenEvent;

    public delegate void OnClose();
    public static event OnClose OnCloseEvent;

    public delegate void OnCardSelected(UICatalogFurnituresCardController card);
    public static event OnCardSelected OnCardSelectedEvent;

    public delegate void OnCardThemeSelected(UICatalogRoomsCardController card);
    public static event OnCardThemeSelected OnCardThemeSelectedEvent;

    public delegate void ClickBtnBack();
    public static event ClickBtnBack OnClickBtnBack;

    public delegate void OpenGemsStore();
    public static event OpenGemsStore OnOpenGemsStore;

    [SerializeField] private TabManager principalTabManager;
    [SerializeField] private TabManager secondaryTabManager;
    [SerializeField] private GameObject secondaryTabsContainer;
    [SerializeField] private UICatalogFurnituresScrollView furnitureScrollView;
    [SerializeField] private UICatalogRoomsScrollView roomsScrollView;
    [SerializeField] private GameObject loaderPanel;
    [SerializeField] private GameObject panelFurnitures;
    [SerializeField] private GameObject panelRooms;
    [SerializeField] private UIPanelBuyFurnitures panelBuyFurnitures;
    [SerializeField] private UIDialogPanel dialogPanel;
    [SerializeField] private UIStoreManager storeManager;

    private List<ItemType> itemTypesSecondaryTabs = new List<ItemType> { ItemType.CHAIR, ItemType.FLOOR, ItemType.LAMP, ItemType.TABLE, ItemType.FURNITURES_INVENTORY };

    private IGameData gameData;
    private IAvatarEndpoints avatarEndpoints;
    private UserInformation userInformation;

    void Start()
    {
        gameData = Injection.Get<IGameData>();
        avatarEndpoints = Injection.Get<IAvatarEndpoints>();
        userInformation = gameData.GetUserInformation();

        principalTabManager.OnTabSelected += OnPrincipalTabSelected;
        secondaryTabManager.OnTabSelected += OnSecondaryTabSelected;
        furnitureScrollView.OnCardSelected += OnFurnitureCardSelected;
        roomsScrollView.OnCardSelected += OnRoomCardSelected;
    }

    void OnDestroy()
    {
        principalTabManager.OnTabSelected -= OnPrincipalTabSelected;
        secondaryTabManager.OnTabSelected -= OnSecondaryTabSelected;
        furnitureScrollView.OnCardSelected -= OnFurnitureCardSelected;
        roomsScrollView.OnCardSelected -= OnRoomCardSelected;
    }

    public TabManager GetPrincipalTabManager()
    {
        return principalTabManager;
    }

    public override void Open()
    {
        base.Open();
        loaderPanel.SetActive(false);
        ShowSecondaryTabs();
        principalTabManager.UnselectAllTabs();
        principalTabManager.SetTab(0);
        if (OnOpenEvent!=null)
        {
            OnOpenEvent();
        }
    }

    public void SelectSecondaryTab(int index)
    {
        secondaryTabManager.SetTab(index);
    }

    public override void Close()
    {
        base.Close();
        if (OnCloseEvent != null)
        {
            OnCloseEvent();
        }
    }

    public void OnClickButtonBack()
    {
        Close();
        if (OnClickBtnBack != null)
        {
            OnClickBtnBack();
        }
    }

    void OnPrincipalTabSelected(int index)
    {
        if (index==0)
        {
            //Show Furnitures Panel and tabs
            panelFurnitures.gameObject.SetActive(true);
            secondaryTabsContainer.SetActive(true);
            secondaryTabManager.SetTab(0);
            //Hide Rooms  Panel
            panelRooms.SetActive(false);
        }
        else
        {
            //Hide Furnitures Panel and tabs
            panelFurnitures.gameObject.SetActive(false);
            secondaryTabsContainer.SetActive(false);

            //Show rooms panel
            panelRooms.SetActive(true);
            roomsScrollView.ShowObjects();
        }
    }

    public void OnOpenStore()
    {
        Close();
        storeManager.Open();
        if (OnOpenGemsStore!=null)
        {
            OnOpenGemsStore();
        }
    }

    //------------------------------------
    //--------- SECONDARY TABS -----------
    //------------------------------------

    void OnSecondaryTabSelected(int index)
    {
        UICatalogAvatarTabController tabItem = secondaryTabManager.GetTabByIndex(index) as UICatalogAvatarTabController;
        furnitureScrollView.ShowObjects(tabItem.ItemType);
    }

    void ShowSecondaryTabs()
    {
        HideSecondaryTabs();
        int amount = itemTypesSecondaryTabs.Count;
        for (int i = 0; i < amount; i++)
        {
            UICatalogAvatarTabController tabItem = secondaryTabManager.GetTabByIndex(i) as UICatalogAvatarTabController;
            tabItem.gameObject.SetActive(true);
            tabItem.SetTab(itemTypesSecondaryTabs[i]);
        }
    }

    void HideSecondaryTabs()
    {
        secondaryTabManager.HideAllTabs();
        secondaryTabManager.UnselectAllTabs();
    }

    //------------------------------
    //--------- BUY ITEM -----------
    //------------------------------

    void OnRoomCardSelected(GenericCatalogItem catalogItem, UIAbstractCardController cardController)
    {
        if (OnCardThemeSelectedEvent!=null)
        {
            OnCardThemeSelectedEvent(cardController as UICatalogRoomsCardController);
        }
    }

    void OnFurnitureCardSelected(GenericCatalogItem catalogItem, UIAbstractCardController cardController)
    {
        if (OnCardSelectedEvent!=null)
        {
            OnCardSelectedEvent(cardController as UICatalogFurnituresCardController);
        }
    }
}

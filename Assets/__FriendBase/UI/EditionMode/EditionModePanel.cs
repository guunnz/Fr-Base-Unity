using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Data.Catalog;
using Socket;
using Data.Bag;
using Data;
using Architecture.Injector.Core;
using DG.Tweening;

public class EditionModePanel : AbstractUIPanel
{
    enum EDITION_MODE_STATE { NONE, TAP_TO_PLACE, TAP_TO_MOVE_FROM_STORE, TAP_TO_MOVE_FROM_ROOM };

    [SerializeField] private FurnitureRoomController furnitureRoomController;
    [SerializeField] private ControlFurniturePlacement controlFurniturePlacement;
    [SerializeField] protected Image imgBar;
    [SerializeField] protected Button btnClose;
    [SerializeField] protected Button btnInventory;
    [SerializeField] protected Button btnFlip;
    [SerializeField] protected Button btnOk;
    [SerializeField] protected Button btnBack;
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtTitleLeft;
    [SerializeField] protected UIStoreFurnituresManager storeFurnituresManager;
    [SerializeField] protected UIPanelBuyFurnitures panelBuyFurnitures;
    [SerializeField] protected GameObject loader;
    [SerializeField] protected GameObject gemsHolder;
    [SerializeField] protected CameraEditModeController cameraEditController;

    private EDITION_MODE_STATE state;
    private UICatalogFurnituresCardController cardSelected;
    private FurnitureRoomController furnitureRoomControllerInRoom;
    private bool clickOverUi;
    private Camera cam;
    private IGameData gameData;

    protected override void Start()
    {
        base.Start();
        gameData = Injection.Get<IGameData>();
        state = EDITION_MODE_STATE.NONE;
        furnitureRoomController.gameObject.SetActive(false);
        UIStoreFurnituresManager.OnCardSelectedEvent += OnCardSelected;
        UIStoreFurnituresManager.OnClickBtnBack += OnClickBtnBack;
        UIStoreFurnituresManager.OnOpenGemsStore += OnOpenGemsStore;
        InputManager.OnTapFurniture += OnTapFurniture;
        controlFurniturePlacement.OnStatusNotify += OnControlFurnitureNotify;
        cam = CurrentRoom.Instance.GetRoomUIReferences().Camera;
        clickOverUi = false;

        storeFurnituresManager.GetPrincipalTabManager().OnTabSelected += OnTabSelectedFurnitureManager;
    }

    void OnTabSelectedFurnitureManager(int index)
    {
        //If theme is selected => Close Edition
        if (index == 1)
        {
            txtTitleLeft.gameObject.SetActive(false);
            state = EDITION_MODE_STATE.NONE;
        }
    }

    private void OnDestroy()
    {
        UIStoreFurnituresManager.OnCardSelectedEvent -= OnCardSelected;
        UIStoreFurnituresManager.OnClickBtnBack -= OnClickBtnBack;
        UIStoreFurnituresManager.OnOpenGemsStore -= OnOpenGemsStore;
        InputManager.OnTapFurniture -= OnTapFurniture;
        controlFurniturePlacement.OnStatusNotify -= OnControlFurnitureNotify;
        storeFurnituresManager.GetPrincipalTabManager().OnTabSelected -= OnTabSelectedFurnitureManager;
    }

    void OnControlFurnitureNotify(ControlFurniturePlacement controlFurniturePlacement, bool canPlace)
    {
        SetButtonOkInteractable(canPlace);
    }

    void OnOpenGemsStore()
    {
        Close();
    }

    void SetButtonOkInteractable(bool flag)
    {
        btnOk.interactable = flag;
        float alpha = 1;
        if (!flag)
        {
            alpha = 0.5f;
        }
        btnOk.GetComponent<Image>().DOFade(alpha, 0);
    }

    public override void OnClose()
    {
        state = EDITION_MODE_STATE.NONE;
        RestoreRoomUI();
    }

    private void HideAll()
    {
        imgBar.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        btnInventory.gameObject.SetActive(false);
        btnFlip.gameObject.SetActive(false);
        btnOk.gameObject.SetActive(false);
      
        txtTitle.gameObject.SetActive(false);
        txtTitleLeft.gameObject.SetActive(false);
        gemsHolder.SetActive(false);
        btnBack.gameObject.SetActive(false);
    }

    //State Tap to Place
    public void SetStateTapToPlace()
    {
        Open();
        state = EDITION_MODE_STATE.TAP_TO_PLACE;
        HideAll();

        txtTitleLeft.gameObject.SetActive(true);

        txtTitleLeft.text = "Tap to place";

        //gemsHolder.SetActive(true);
    }

    //State Tap to Move (When Store Open)
    public void SetStateTapToMoveFromStore()
    {
        Open();
        state = EDITION_MODE_STATE.TAP_TO_MOVE_FROM_STORE;
        HideAll();

        txtTitle.gameObject.SetActive(true);
        txtTitle.text = "Tap to move";
        cameraEditController.enabled = true;
        imgBar.gameObject.SetActive(true);
        btnClose.gameObject.SetActive(true);
        btnFlip.gameObject.SetActive(true);
        btnOk.gameObject.SetActive(true);
        //gemsHolder.SetActive(true);
    }

    //State Tap to Move (When long tapping a furniture)
    public void SetStateTapToMoveFromRoom()
    {
        storeFurnituresManager.Close();
        CurrentRoom.Instance.GetRoomUIReferences().HideAll();

        Open();
        state = EDITION_MODE_STATE.TAP_TO_MOVE_FROM_ROOM;
        HideAll();

        txtTitle.gameObject.SetActive(true);
        txtTitle.text = "Tap to move";
        cameraEditController.enabled = true;
        imgBar.gameObject.SetActive(true);
        btnInventory.gameObject.SetActive(true);
        btnFlip.gameObject.SetActive(true);
        btnOk.gameObject.SetActive(true);
        btnBack.gameObject.SetActive(true);
    }

    //Card is selected from the store
    void OnCardSelected(UICatalogFurnituresCardController card)
    {
        cardSelected = card;
        SetStateTapToPlace();
        StartEditionMode(card.ObjCat, new FurnitureRoomData(card.ObjCat, -1, -1, new Vector2(-1000, 0), 1));
    }

    void StartEditionMode(GenericCatalogItem objCat, FurnitureRoomData furnitureRoomData)
    {
        clickOverUi = true;
        furnitureRoomController.gameObject.SetActive(true);
        furnitureRoomController.Init(furnitureRoomData);
        CurrentRoom.Instance.IsInEditionMode = true;
        CurrentRoom.Instance.AvatarsManager.GetMyAvatar().Hide();
        CurrentRoom.Instance.GetRoomUIReferences().HideAll();
    }


    private void Update()
    {
        switch (state)
        {
            case EDITION_MODE_STATE.TAP_TO_PLACE:
                if (ClickAndPosition())
                {
                    SetStateTapToMoveFromStore();
                    storeFurnituresManager.Close();
                    CurrentRoom.Instance.GetRoomUIReferences().HideAll();
                }
                break;
            case EDITION_MODE_STATE.TAP_TO_MOVE_FROM_ROOM:
            case EDITION_MODE_STATE.TAP_TO_MOVE_FROM_STORE:
                ClickAndPosition();
                break;
        }
    }

    bool ClickAndPosition()
    {
        //Mouse Down
        if (InputFunctions.GetMouseButtonDown(0))
        {
            clickOverUi = InputManager.IsPointerOverGameObject();
        }

        //Mouse Up
        if (InputFunctions.GetMouseButtonUp(0))
        {
            if (!clickOverUi)
            {
                CurrentRoom.Instance.CurrentRoomManager.ActivateColliders();
                Vector2 position = cam.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                furnitureRoomController.SetPosition(position);
                controlFurniturePlacement.Select();
                return true;
            }
        }
        return false;
    }

    public void OnClickOpenGemsStore()
    {
        CurrentRoom.Instance.GetRoomUIReferences().OpenStorePanel();
        Close();
    }

    public void OnClickClose()
    {
        switch (state)
        {
            case EDITION_MODE_STATE.TAP_TO_MOVE_FROM_STORE:
                Close();
                break;
        }
    }

    public void OnClickInventory()
    {
        switch (state)
        {
            case EDITION_MODE_STATE.TAP_TO_MOVE_FROM_ROOM:
                loader.SetActive(true);

                SimpleSocketManager.Instance.Suscribe(SocketEventTypes.REMOVE_FURNITURE, OnRemoveFurniture);

                SimpleSocketManager.Instance.SendFurnitureRemove(
                     CurrentRoom.Instance.RoomInformation.RoomName,
                     CurrentRoom.Instance.RoomInformation.RoomIdInstance,
                     furnitureRoomController.FurnitureRoomData.IdInstance);

                break;
        }
    }

    void OnRemoveFurniture(AbstractIncomingSocketEvent abstractIncomingSocketEvent)
    {
        loader.SetActive(false);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.REMOVE_FURNITURE, OnRemoveFurniture);
        Close();
        storeFurnituresManager.Open();
        storeFurnituresManager.SelectSecondaryTab(4);
    }

    void RestoreRoomUI()
    {
        CurrentRoom.Instance.GetRoomUIReferences().ShowAll();
        furnitureRoomController.gameObject.SetActive(false);
        cameraEditController.enabled = false;
        CurrentRoom.Instance.IsInEditionMode = false;
        CurrentRoom.Instance.AvatarsManager.GetMyAvatar().Show();
        loader.SetActive(false);
    }

    public void OnClickRotate()
    {
        furnitureRoomController.ChangeOrientation();
    }

    public void OnClickDone()
    {
        switch (state)
        {
            case EDITION_MODE_STATE.TAP_TO_MOVE_FROM_STORE:
                if (!cardSelected.IsInventoryItem)
                {
                    //If it is an item to buy
                    Close();
                    panelBuyFurnitures.Open();
                    panelBuyFurnitures.CreateItemsToBuy(new List<GenericCatalogItem> { cardSelected.ObjCat });
                    panelBuyFurnitures.OnBuyComplete += OnBuyComplete;
                }
                else
                {
                    //It is an item from the inventory => We add it to the room
                    AddFurniture();
                }
                break;
            case EDITION_MODE_STATE.TAP_TO_MOVE_FROM_ROOM:
                MoveFurniture();
                break;
        }
    }

    //Send Move endpoint to the server and wait for response
    void MoveFurniture()
    {
        loader.SetActive(true);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.MOVE_FURNITURE, OnMoveFurniture);

        SimpleSocketManager.Instance.SendFurnitureMove
        (
            CurrentRoom.Instance.RoomInformation.RoomName,
            CurrentRoom.Instance.RoomInformation.RoomIdInstance,
            furnitureRoomController.FurnitureRoomData.IdInstance,
            furnitureRoomController.FurnitureRoomData.Position.x,
            furnitureRoomController.FurnitureRoomData.Position.y,
            furnitureRoomController.FurnitureRoomData.Orientation
        );
    }

    //Response from the server
    void OnMoveFurniture(AbstractIncomingSocketEvent abstractIncomingSocketEvent)
    {
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.MOVE_FURNITURE, OnMoveFurniture);
        Close();
    }

    //Send Add Furniture to the server and wait for response
    void AddFurniture()
    {
        loader.SetActive(true);
        GenericBagItem genericBagItem = gameData.GetBagByItemType(cardSelected.ObjCat.ItemType).GetItemById(cardSelected.ObjCat.IdItem);
        if (genericBagItem != null)
        {
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.ADD_FURNITURE, OnAddFurniture);

            SimpleSocketManager.Instance.SendFurnitureAdd
            (
                CurrentRoom.Instance.RoomInformation.RoomName,
                CurrentRoom.Instance.RoomInformation.RoomIdInstance,
                genericBagItem.IdInstance,
                genericBagItem.ObjCat.IdItemWebClient,
                furnitureRoomController.FurnitureRoomData.Position.x,
                furnitureRoomController.FurnitureRoomData.Position.y,
                furnitureRoomController.FurnitureRoomData.Orientation
            );
        }
        else
        {
            Close();
        }
    }

    //Response from the server
    void OnAddFurniture(AbstractIncomingSocketEvent abstractIncomingSocketEvent)
    {
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.ADD_FURNITURE, OnAddFurniture);
        Close();
    }

    //It is called when buying furnitures is complete
    void OnBuyComplete(bool flagSucceed)
    {
        panelBuyFurnitures.OnBuyComplete -= OnBuyComplete;
        if (flagSucceed)
        {
            AddFurniture();
        }
        else
        {
            Close();
        }
    }

    //Call when we long tap a furniture
    void OnTapFurniture(FurnitureRoomController furnitureRoomControllerTapped)
    {
        SetStateTapToMoveFromRoom();
        StartEditionMode(furnitureRoomControllerTapped.FurnitureRoomData.ObjCat, furnitureRoomControllerTapped.FurnitureRoomData);
        furnitureRoomControllerInRoom = furnitureRoomControllerTapped;
        furnitureRoomControllerInRoom.Hide();
        CurrentRoom.Instance.RescanPathfinding();

        furnitureRoomController.SetPosition(furnitureRoomControllerInRoom.transform.position);
        controlFurniturePlacement.Select();
    }

    //Callback when the btn back of storeFurniture is clicked
    void OnClickBtnBack()
    {
        Close();
    }

    //Btn back from editionMode is clicked (In Moving Room) -> We show the hidden furniture
    public void OnBtnBack()
    {
        switch (state)
        {
            case EDITION_MODE_STATE.TAP_TO_MOVE_FROM_ROOM:
                furnitureRoomControllerInRoom.Show();
                CurrentRoom.Instance.RescanPathfinding();
                break;
        }
        Close();
    }
}
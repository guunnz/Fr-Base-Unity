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
using AddressablesSystem;
using Data.Rooms;
using UniRx;

public class EditThemePanel : AbstractUIPanel
{
    [SerializeField] protected Button btnCancel;
    [SerializeField] protected Button btnOk;
    [SerializeField] protected GameObject loaderHolder;
    [SerializeField] protected GameObject gemsHolder;
    [SerializeField] protected UIStoreFurnituresManager storeFurnituresManager;
    [SerializeField] protected UIDialogPanel dialogPanel;
    [SerializeField] protected GameObject themeContainer;
    [SerializeField] protected UIPanelBuyFurnitures panelBuyFurnitures;

    private IGameData gameData;
    public GenericCatalogItem ObjCat { get; private set; }
    private ILoader loaderManager;
    private RoomManager currentRoomThemeManager;
    private Coroutine loadThemeCoroutine;

    protected override void Start()
    {
        base.Start();
        loaderManager = Injection.Get<ILoader>();
        gameData = Injection.Get<IGameData>();
        UIStoreFurnituresManager.OnCardThemeSelectedEvent += OnThemeSelected;
    }

    private void OnDestroy()
    {
        UIStoreFurnituresManager.OnCardThemeSelectedEvent -= OnThemeSelected;
    }

    void OnThemeSelected(UICatalogRoomsCardController themeCard)
    {
        //gameData.GetUserInformation().SetGems(4);
        //Check if the user is selecting the same Room
        string namePrefab = themeCard.ObjCat.NamePrefab.Split('_')[1];
        if (CurrentRoom.Instance.RoomInformation.NamePrefab.Equals(namePrefab))
        {
            return;
        }

        Open();
        storeFurnituresManager.Close();
        StartEditionMode(themeCard.ObjCat);

        if (loadThemeCoroutine!=null)
        {
            StopCoroutine(loadThemeCoroutine);
        }
        loadThemeCoroutine = StartCoroutine(CreateRoomPrefab(themeCard.ObjCat));
    }

    IEnumerator CreateRoomPrefab(GenericCatalogItem objCat)
    {
        string namePrefab = objCat.NamePrefab.Split('_')[1] + "_prefab";
        loaderManager.LoadItem(new LoaderItemModel(namePrefab));

        GameObject roomPrefab = loaderManager.GetModel(namePrefab);
        while (roomPrefab == null)
        {
            yield return new WaitForEndOfFrame();
            roomPrefab = loaderManager.GetModel(namePrefab);
        }
        roomPrefab.transform.position = Vector3.zero;
        roomPrefab.transform.SetParent(themeContainer.transform, true);

        if (currentRoomThemeManager!=null)
        {
            Destroy(currentRoomThemeManager);
        }

        currentRoomThemeManager = roomPrefab.GetComponent<RoomManager>();
        currentRoomThemeManager.DeactivateCameras();
    }

    void StartEditionMode(GenericCatalogItem objCat)
    {
        ObjCat = objCat;
        CurrentRoom.Instance.IsInEditionMode = true;
        CurrentRoom.Instance.AvatarsManager.GetMyAvatar().Hide();
        CurrentRoom.Instance.GetRoomUIReferences().HideAll();
        CreateRoomPrefab(ObjCat);
    }

    void RestoreRoomUI()
    {
        CurrentRoom.Instance.GetRoomUIReferences().ShowAll();
        CurrentRoom.Instance.IsInEditionMode = false;
        CurrentRoom.Instance.AvatarsManager.GetMyAvatar().Show();
        loaderHolder.SetActive(false);
        if (currentRoomThemeManager != null)
        {
            Destroy(currentRoomThemeManager.gameObject);
        }
    }

    public override void OnClose()
    {
        RestoreRoomUI();
    }

    public void OnClickClose()
    {
        Close();
    }

    public void OnClickDone()
    {
        GenericBagItem bagItem = gameData.GetBagByItemType(ItemType.ROOM).GetItemById(ObjCat.IdItem);
        if (bagItem!=null)
        {
            //Theme already in the inventory
            string txtTitle = language.GetTextByKey(LangKeys.STORE_CHANGING_THEME_WILL_MOVE_FURNITURE_TO_STORAGE);
            string txtDesc = "";
            string txtBtnAccept = language.GetTextByKey(LangKeys.COMMON_LABEL_OK);
            string txtBtnDiscard = language.GetTextByKey(LangKeys.STORE_DONT_CHANGE);
            Close();

            dialogPanel.Open(txtTitle, txtDesc, txtBtnAccept, txtBtnDiscard, () =>
            {
                ChangeTheme();
            });
        }
        else
        {
            //Item to buy
            panelBuyFurnitures.Open();
            panelBuyFurnitures.CreateRoomsToBuy(new List<GenericCatalogItem> { ObjCat });
            panelBuyFurnitures.OnBuyComplete += OnBuyComplete;
            Close();
        }
    }

    //It is called when buying theme is complete
    void OnBuyComplete(bool flagSucceed)
    {
        panelBuyFurnitures.OnBuyComplete -= OnBuyComplete;
        if (flagSucceed)
        {
            //Theme already in the inventory
            string txtTitle = language.GetTextByKey(LangKeys.STORE_THEME_IS_YOURS); 
            string txtDesc = language.GetTextByKey(LangKeys.STORE_IF_YOU_USE_IT_NOW_FURNITURE_STORED);
            string txtBtnAccept = language.GetTextByKey(LangKeys.STORE_OK_USE_THEME); 
            string txtBtnDiscard = language.GetTextByKey(LangKeys.STORE_DONT_USE);
            Close();

            dialogPanel.Open(txtTitle, txtDesc, txtBtnAccept, txtBtnDiscard, () =>
            {
                ChangeTheme();
            });
        }
        else
        {
            Close();
        }
    }

    void ChangeTheme()
    {
        GenericBagItem bagItem = gameData.GetBagByItemType(ItemType.ROOM).GetItemById(ObjCat.IdItem);
        if (bagItem != null)
        {
            int idTheme = bagItem.IdInstance;
            Injection.Get<IRoomListEndpoints>().ChangeTheme(idTheme).Subscribe(roomInformation =>
            {
                ReturnFurnituresToLocalInventory();

                gameData.SetMyHouseInformation(roomInformation);
                CurrentRoom.Instance.GoToMyHouse();
            });
        }
    }

    void ReturnFurnituresToLocalInventory()
    {
        FurnituresRoomManager furnituresManager = CurrentRoom.Instance.FurnituresRoomManager;
        int amount = furnituresManager.GetAmountOfFurnitures();
        for (int i=0; i<amount; i++)
        {
            FurnitureRoomController furnitureController = furnituresManager.GetFurnitureByIndex(i);
            FurnitureRoomData furnitureRoomData = furnitureController.FurnitureRoomData;
            GenericBagItem bagItem = new GenericBagItem(furnitureRoomData.ObjCat.ItemType, furnitureRoomData.IdInstance, 1, furnitureRoomData.ObjCat);
            gameData.AddItemToBag(bagItem);
        }
    }

    public void OnClickOpenGemsStore()
    {
        Close();
        CurrentRoom.Instance.GetRoomUIReferences().OpenStorePanel();
    }
}

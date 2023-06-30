using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Data.Catalog;
using Data.Users;
using TMPro;
using UnityEngine;
using UniRx;
using Data.Bag;

public class UIPanelBuyFurnitures : AbstractUIPanel
{
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtBtnCancel;
    [SerializeField] protected TextMeshProUGUI txtBtnBuy;
    [SerializeField] protected TextMeshProUGUI txtTotalLabel;
    [SerializeField] protected TextMeshProUGUI txtTotalPrice;
    [SerializeField] protected TextMeshProUGUI txtTotalPriceGold;
    [SerializeField] protected GameObject goldContainer;
    [SerializeField] protected GameObject gemContainer;
    [SerializeField] protected GameObject itemsContainer;
    [SerializeField] protected GameObject roomContainer;
    [SerializeField] protected Load2DObject prefabItemToBuy;
    [SerializeField] protected Load2DObject prefabRoomToBuy;
    [SerializeField] protected GameObject loader;
    [SerializeField] private UIDialogPanel dialogPanel;

    public delegate void BuyComplete(bool succeed);
    public event BuyComplete OnBuyComplete;

    public int TotalPriceGem { get; private set; }
    public int TotalPriceGold { get; private set; }

    private List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

    private IGameData gameData;
    private UserInformation userInformation;
    private IAvatarEndpoints avatarEndpoints;

    protected override void Start()
    {
        base.Start();
        gameData = Injection.Get<IGameData>();
        userInformation = gameData.GetUserInformation();
        avatarEndpoints = Injection.Get<IAvatarEndpoints>();
        language.SetTextByKey(txtTitle, LangKeys.STORE_READY_TO_BUY);
        language.SetTextByKey(txtBtnCancel, LangKeys.STORE_CANCEL);
        language.SetTextByKey(txtBtnBuy, LangKeys.EVENTS_YES_BUY);
        language.SetText(txtTotalLabel, language.GetTextByKey(LangKeys.STORE_TOTAL) + ":");
    }

    public List<GenericCatalogItem> GetListOfItemsToBuy()
    {
        return listItems;
    }

    void CleanContainers()
    {
        //Clean Container of items
        foreach (Transform child in itemsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        //Clean Container of Rooms
        foreach (Transform child in roomContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void CreateItemsToBuy(List<GenericCatalogItem> listItems)
    {
        this.listItems = listItems;

        CleanContainers();

        TotalPriceGem = 0;
        TotalPriceGold = 0;

        foreach (GenericCatalogItem item in listItems)
        {
            //Create Item Card
            GameObject cardGameobject = Instantiate(prefabItemToBuy.gameObject, transform.position, Quaternion.identity);
            Load2DObject card = cardGameobject.GetComponent<Load2DObject>();

            card.Load(item.GetNameFurniturePrefabUIByItem());
            card.transform.SetParent(itemsContainer.transform);
            card.transform.localScale = Vector3.one;

            if (item.CurrencyType == CurrencyType.GOLD_PRICE)
            {
                TotalPriceGold += item.GoldPrice;
            }
            else if (item.CurrencyType == CurrencyType.GEM_PRICE)
            {
                TotalPriceGem += item.GemsPrice;
            }
        }

        txtTotalPriceGold.text = TotalPriceGold.ToString();
        txtTotalPrice.text = TotalPriceGem.ToString();

        if (TotalPriceGold > 0)
        {
            gemContainer.SetActive(false);
            goldContainer.SetActive(true);
        }
        else
        {
            gemContainer.SetActive(true);
            goldContainer.SetActive(false);
        }
    }

    public void CreateRoomsToBuy(List<GenericCatalogItem> listItems)
    {
        this.listItems = listItems;

        CleanContainers();

        TotalPriceGem = 0;
        TotalPriceGold = 0;

        foreach (GenericCatalogItem item in listItems)
        {
            //Create Item Card
            GameObject cardGameobject = Instantiate(prefabRoomToBuy.gameObject, transform.position, Quaternion.identity);
            Load2DObject card = cardGameobject.GetComponent<Load2DObject>();

            card.Load(item.NamePrefab);
            card.transform.SetParent(roomContainer.transform);
            card.transform.localScale = Vector3.one;

            if (item.CurrencyType == CurrencyType.GOLD_PRICE)
            {
                TotalPriceGold += item.GoldPrice;
            }
            else if (item.CurrencyType == CurrencyType.GEM_PRICE)
            {
                TotalPriceGem += item.GemsPrice;
            }
        }

        txtTotalPriceGold.text = TotalPriceGold.ToString();
        txtTotalPrice.text = TotalPriceGem.ToString();

        if (TotalPriceGold > 0)
        {
            gemContainer.SetActive(false);
            goldContainer.SetActive(true);
        }
        else
        {
            gemContainer.SetActive(true);
            goldContainer.SetActive(false);
        }
    }

    public void OnButtonCancel()
    {
        Close();
        if (OnBuyComplete != null)
        {
            OnBuyComplete(false);
        }
    }

    public void OnButtonBuy()
    {
        if (userInformation.Gems >= TotalPriceGem && userInformation.Gold >= TotalPriceGold)
        {
            loader.SetActive(true);
            avatarEndpoints.PurchaseItem(GetListOfItemsToBuy()).Subscribe(listBagItems =>
            {
                loader.SetActive(false);
                foreach (GenericBagItem bagItem in listBagItems)
                {
                    gameData.AddItemToBag(bagItem);
                    if (bagItem.ObjCat.CurrencyType == CurrencyType.GEM_PRICE)
                    {
                        userInformation.SubstractGems(bagItem.ObjCat.GemsPrice);
                    }
                    else
                    {
                        userInformation.SubstractGold(bagItem.ObjCat.GoldPrice);
                    }
                }

                if (OnBuyComplete != null)
                {
                    OnBuyComplete(true);
                }
                Close();
            });
        }
        else
        {
            if (userInformation.Gems < TotalPriceGem)
            {
                string txtTitle = language.GetTextByKey(LangKeys.STORE_YOU_NEED_MORE_GEMS);
                string txtDesc = string.Format(language.GetTextByKey(LangKeys.COMMON_YOU_CANT_PAY_FOR_SOME_OF_THE_ITEMS_YOU_HAVE_SELECTED), TotalPriceGem - gameData.GetUserInformation().Gems);
                string txtBtnAccept = language.GetTextByKey(LangKeys.STORE_GET_MORE_GEMS);
                string txtBtnDiscard = language.GetTextByKey(LangKeys.STORE_CANCEL);
                Close();

                dialogPanel.Open(txtTitle, txtDesc, txtBtnAccept, txtBtnDiscard, () =>
                {
                    CurrentRoom.Instance.GetRoomUIReferences().OpenStorePanel();
                });

                if (OnBuyComplete != null)
                {
                    OnBuyComplete(false);
                }
            }
            else
            {
                string txtTitle = language.GetTextByKey(LangKeys.STORE_YOU_NEED_MORE_GEMS).Replace("gems", "gold").Replace("gemas", language.GetCurrentLanguage() == "es" ? "monedas" : "ouro").Replace("mücevher", "Altın").Replace("Gems", "gold").Replace("Gemas", language.GetCurrentLanguage() == "es" ? "Monedas" : "ouro").Replace("Mücevher", "Altın");
                string txtDesc = string.Format(language.GetTextByKey(LangKeys.STORE_YOU_CANT_PAY_NEED_MORE_GEMS), TotalPriceGold - gameData.GetUserInformation().Gold).Replace("gems", "gold").Replace("gemas", language.GetCurrentLanguage() == "es" ? "monedas" : "ouro").Replace("mücevher", "Altın").Replace("Gems", "gold").Replace("Gemas", language.GetCurrentLanguage() == "es" ? "Monedas" : "ouro").Replace("Mücevher", "Altın");
                string txtBtnAccept = language.GetTextByKey(LangKeys.STORE_GET_MORE_GEMS);
                string txtBtnDiscard = language.GetTextByKey(LangKeys.STORE_CANCEL);

                Close();

                dialogPanel.Open(txtTitle, txtDesc, txtBtnAccept, txtBtnDiscard, () =>
                {
                    CurrentRoom.Instance.GetRoomUIReferences().OpenStorePanel();
                }, gemPurchase: false);

                if (OnBuyComplete != null)
                {
                    OnBuyComplete(false);
                }
            }
        }
    }
}
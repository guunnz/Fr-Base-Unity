using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Data.Users;
using Data.Catalog;

public class UIPanelBuyClothes : AbstractUIPanel
{
    //Small Panel
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtBtnCancel;
    [SerializeField] protected TextMeshProUGUI txtBtnBuy;
    [SerializeField] protected TextMeshProUGUI txtTotalLabel;
    [SerializeField] protected TextMeshProUGUI txtTotalPrice;
    [SerializeField] protected TextMeshProUGUI txtTotalPriceGold;
    [SerializeField] protected GameObject goldContainer;
    [SerializeField] protected GameObject gemContainer;
    [SerializeField] protected GameObject itemsContainer;
    [SerializeField] protected GameObject popUp;
    //Big Panel
    [SerializeField] protected TextMeshProUGUI txtTitleBig;
    [SerializeField] protected TextMeshProUGUI txtBtnCancelBig;
    [SerializeField] protected TextMeshProUGUI txtBtnBuyBig;
    [SerializeField] protected TextMeshProUGUI txtTotalLabelBig;
    [SerializeField] protected TextMeshProUGUI txtTotalPriceBig;
    [SerializeField] protected GameObject itemsContainerBig;
    [SerializeField] protected GameObject popUpBig;

    [SerializeField] protected Load2DObject prefabItemToBuy;


    public int TotalPriceGem { get; private set; }
    public int TotalPriceGold { get; private set; }

    public delegate void BuyButtonPressed();
    public event BuyButtonPressed OnBuyButtonPressed;

    public AvatarCustomizationData AvatarCustomizationData;

    public override void OnOpen()
    {
        language.SetTextByKey(txtTitle, LangKeys.STORE_READY_TO_BUY);
        language.SetTextByKey(txtBtnCancel, LangKeys.STORE_CANCEL);
        language.SetTextByKey(txtBtnBuy, LangKeys.STORE_BUY);
        language.SetText(txtTotalLabel, language.GetTextByKey(LangKeys.STORE_TOTAL) + ":");

        language.SetTextByKey(txtTitleBig, LangKeys.STORE_READY_TO_BUY);
        language.SetTextByKey(txtBtnCancelBig, LangKeys.STORE_CANCEL);
        language.SetTextByKey(txtBtnBuyBig, LangKeys.STORE_BUY);
        language.SetText(txtTotalLabelBig, language.GetTextByKey(LangKeys.STORE_TOTAL) + ":");
    }

    public List<GenericCatalogItem> GetListOfItemsToBuy()
    {
        return AvatarCustomizationData.GetListItemsMissingOnInventory();
    }

    public void CreateItemsToBuy(AvatarCustomizationData avatarCustomizationData)
    {
        this.AvatarCustomizationData = avatarCustomizationData;
        List<GenericCatalogItem> listItems = avatarCustomizationData.GetListItemsMissingOnInventory();

        //We scale the pop up if there is several items to buy
        GameObject currentItemsContainer = itemsContainer;
        if (listItems.Count <= 6)
        {
            popUp.SetActive(true);
            popUpBig.SetActive(false);
            currentItemsContainer = itemsContainer;
        }
        else
        {
            popUp.SetActive(false);
            popUpBig.SetActive(true);
            currentItemsContainer = itemsContainerBig;
        }

        //Clean Container of items
        foreach (Transform child in currentItemsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        TotalPriceGem = 0;
        TotalPriceGold = 0;
        foreach (AvatarGenericCatalogItem item in listItems)
        {
            //Create Item Card
            GameObject cardGameobject = Instantiate(prefabItemToBuy.gameObject, transform.position, Quaternion.identity);
            Load2DObject card = cardGameobject.GetComponent<Load2DObject>();

            AvatarCustomizationDataUnit dataUnit = avatarCustomizationData.GetDataUnit(item.ItemType);
            int idColor = dataUnit.ColorObjCat.IdItem;
            string namePrefab = dataUnit.AvatarObjCat.GetNamePrefabUIByItem(avatarCustomizationData.IsBoobsActive(), idColor);

            card.Load(namePrefab);
            card.transform.SetParent(currentItemsContainer.transform);
            card.transform.localScale = Vector3.one;

            if (item.CurrencyType == Data.CurrencyType.GEM_PRICE)
            {
                TotalPriceGem += item.GemsPrice;
            }
            else
            {
                TotalPriceGold += item.GoldPrice;
            }
        }

        txtTotalPrice.text = TotalPriceGem.ToString();
        txtTotalPriceGold.text = TotalPriceGold.ToString();

        goldContainer.SetActive(false);
        gemContainer.SetActive(false);

        if (TotalPriceGold > 0)
        {
            goldContainer.SetActive(true);
        }


        if (TotalPriceGem > 0)
        {
            gemContainer.SetActive(true);
        }
        txtTotalPriceBig.text = TotalPriceGem.ToString();
    }

    public void OnButtonCancel()
    {
        Close();
    }

    public void OnButtonBuy()
    {
        if (OnBuyButtonPressed != null)
        {
            OnBuyButtonPressed();
        }
    }
}

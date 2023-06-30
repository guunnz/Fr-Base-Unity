using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data.Catalog;
using Data.Users;
using TMPro;
using Architecture.Injector.Core;
using Data;
using Data.Bag;
using System;
using UniRx;
using Newtonsoft.Json.Linq;
using UI.ScrollView;

public class UIPanelChangeAvatar : AbstractUIPanel
{
    [SerializeField] private UIStoreManager storeManager;
    [SerializeField] private UIPanelBuyClothes panelBuyClothes;
    [SerializeField] private UIDialogPanel dialogPanel;
    [SerializeField] protected UICatalogAvatarScrollView catalogAvatarScrollView;

    [SerializeField] private Image imgBtnDone;
    [SerializeField] private Image imgBtnBuy;

    [SerializeField] protected TextMeshProUGUI txtBtnDone;

    public delegate void BackButtonPressed();
    public event BackButtonPressed OnBackButtonPressed;
    IAnalyticsSender analyticsSender;
    private IGameData gameData;
    private IAvatarEndpoints avatarEndpoints;
    private UserInformation userInformation;
    private AvatarCustomizationData avatarCustomizationData;

    protected override void Start()
    {
        analyticsSender = Injection.Get<IAnalyticsSender>();
        base.Start();
        gameData = Injection.Get<IGameData>();
        avatarEndpoints = Injection.Get<IAvatarEndpoints>();
        userInformation = gameData.GetUserInformation();
    }

    public void Open(AvatarCustomizationData avatarCustomizationData)
    {
        base.Open();

        language.SetTextByKey(txtBtnDone, LangKeys.STORE_DONE);

        panelBuyClothes.OnBuyButtonPressed += OnBuyButtonPressed;
        catalogAvatarScrollView.OnCardSelected += OnCardSelected;

        this.avatarCustomizationData = avatarCustomizationData;
        StartCoroutine(RefreshIconsButtonDone());
    }

    public override void Close()
    {
        panelBuyClothes.OnBuyButtonPressed -= OnBuyButtonPressed;
        catalogAvatarScrollView.OnCardSelected -= OnCardSelected;
    }

    void OnCardSelected(AvatarGenericCatalogItem element, UIAbstractCardController cardController)
    {
        StartCoroutine(RefreshIconsButtonDone());
    }

    IEnumerator RefreshIconsButtonDone()
    {
        yield return null;
        List<GenericCatalogItem> listMissingItems = avatarCustomizationData.GetListItemsMissingOnInventory();
        if (listMissingItems.Count > 0)
        {
            imgBtnBuy.gameObject.SetActive(true);
            imgBtnDone.gameObject.SetActive(false);
        }
        else
        {
            imgBtnBuy.gameObject.SetActive(false);
            imgBtnDone.gameObject.SetActive(true);
        }
    }

    void OnBuyButtonPressed()
    {
        if (userInformation.Gems >= panelBuyClothes.TotalPriceGem && userInformation.Gold >= panelBuyClothes.TotalPriceGold)
        {
            avatarEndpoints.PurchaseItem(panelBuyClothes.GetListOfItemsToBuy())
           .Subscribe(listItemsBought =>
           {
               //Send new skin avatar to backend
               JObject json = panelBuyClothes.AvatarCustomizationData.GetSerializeDataWebClient();
               Injection.Get<IAvatarEndpoints>().SetAvatarSkin(json).Subscribe(json => { });

               //Change skin on my avatar structure
               gameData.GetUserInformation().GetAvatarCustomizationData().SetData(avatarCustomizationData);
               analyticsSender.SendAnalytics(AnalyticsEvent.ChangedInRoomSpent);
               userInformation.SubstractGems(panelBuyClothes.TotalPriceGem);
               userInformation.SubstractGold(panelBuyClothes.TotalPriceGold);

               //Add items to my inventory
               List<GenericCatalogItem> listItems = panelBuyClothes.GetListOfItemsToBuy() as List<GenericCatalogItem>;
               foreach (GenericCatalogItem item in listItems)
               {
                   gameData.AddItemToBag(new GenericBagItem(item.ItemType, item.IdItem, 1, item));
               }

               //Refresh Items on Panel
               catalogAvatarScrollView.ShowObjects();

               panelBuyClothes.Close();

               //Open Panel
               string txtTitle = string.Format(language.GetTextByKey(LangKeys.STORE_YOU_HAVE_X_NEW_ITEMS), listItems.Count);
               string txtDesc = language.GetTextByKey(LangKeys.STORE_IS_YOUR_LOOK_READY);
               string txtBtnAccept = language.GetTextByKey(LangKeys.STORE_YES);
               string txtBtnDiscard = language.GetTextByKey(LangKeys.STORE_NO_THANKS);
               dialogPanel.Open(txtTitle, txtDesc, txtBtnAccept, txtBtnDiscard, () =>
               {
                   OnButtonBack();
               });
           });
        }
        else
        {
            if (userInformation.Gems < panelBuyClothes.TotalPriceGem)
            {
                string txtTitle = language.GetTextByKey(LangKeys.STORE_YOU_NEED_MORE_GEMS);
                string txtDesc = string.Format(language.GetTextByKey(LangKeys.COMMON_YOU_CANT_PAY_FOR_SOME_OF_THE_ITEMS_YOU_HAVE_SELECTED), panelBuyClothes.TotalPriceGem - gameData.GetUserInformation().Gems);
                string txtBtnAccept = language.GetTextByKey(LangKeys.STORE_GET_MORE_GEMS);
                string txtBtnDiscard = language.GetTextByKey(LangKeys.STORE_CANCEL);

                panelBuyClothes.Close();
                dialogPanel.Open(txtTitle, txtDesc, txtBtnAccept, txtBtnDiscard, () =>
                {
                    storeManager.Open();
                });
            }
            else
            {
                string txtTitle = language.GetTextByKey(LangKeys.STORE_YOU_NEED_MORE_GEMS).Replace("gems", "gold").Replace("gemas", language.GetCurrentLanguage() == "es" ? "monedas" : "ouro").Replace("mücevher", "Altın").Replace("Gems", "gold").Replace("Gemas", language.GetCurrentLanguage() == "es" ? "Monedas" : "ouro").Replace("Mücevher", "Altın");
                string txtDesc = string.Format(language.GetTextByKey(LangKeys.STORE_YOU_CANT_PAY_NEED_MORE_GEMS), panelBuyClothes.TotalPriceGold - gameData.GetUserInformation().Gold).Replace("gems", "gold").Replace("gemas", language.GetCurrentLanguage() == "es" ? "monedas" : "ouro").Replace("mücevher", "Altın").Replace("Gems", "gold").Replace("Gemas", language.GetCurrentLanguage() == "es" ? "Monedas" : "ouro").Replace("Mücevher", "Altın");
                string txtBtnAccept = language.GetTextByKey(LangKeys.STORE_GET_MORE_GEMS);
                string txtBtnDiscard = language.GetTextByKey(LangKeys.STORE_CANCEL);

                panelBuyClothes.Close();
                dialogPanel.Open(txtTitle, txtDesc, txtBtnAccept, txtBtnDiscard, () =>
                {
                    storeManager.Open();
                }, gemPurchase: false);
            }
        }
    }

    public void OnButtonBack()
    {
        if (OnBackButtonPressed != null)
        {
            OnBackButtonPressed();
        }
    }

    public void OnOpenGemsStore()
    {
        storeManager.Open();
    }

    public void ShowPanelBuyClothes(AvatarCustomizationData avatarCustomizationData)
    {
        panelBuyClothes.Open();
        panelBuyClothes.CreateItemsToBuy(avatarCustomizationData);
    }
}

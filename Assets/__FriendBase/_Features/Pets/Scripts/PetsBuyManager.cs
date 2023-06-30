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
using LocalizationSystem;

public class PetsBuyManager : MonoBehaviour
{
    [SerializeField] private UIDialogPanel dialogPanel;

    public delegate void BuyComplete(bool succeed);
    public event BuyComplete OnBuyComplete;

    public int TotalPrice { get; private set; }

    private GenericCatalogItem PetToBuy;

    private IGameData gameData;
    private ILanguage language;
    private UserInformation userInformation;
    private IAvatarEndpoints avatarEndpoints;
    public UICatalogPetsManager UICatalogManager;

    void Start()
    {
        gameData = Injection.Get<IGameData>();
        language = Injection.Get<ILanguage>();
        userInformation = gameData.GetUserInformation();
        avatarEndpoints = Injection.Get<IAvatarEndpoints>();
        this.OnBuyComplete += ContinueAfterPetPurchase;
    }

    private void OnDestroy()
    {
        this.OnBuyComplete -= ContinueAfterPetPurchase;
    }

    public GenericCatalogItem GetPetToBuy()
    {
        return PetToBuy;
    }

    public void SetPetToBuy(GenericCatalogItem petToBuy)
    {
        if (petToBuy == null)
        {
            this.PetToBuy = null;
            TotalPrice = 0;
            return;
        }
        this.PetToBuy = petToBuy;

        TotalPrice = PetToBuy.CurrencyType == CurrencyType.GOLD_PRICE ? petToBuy.GoldPrice : petToBuy.GemsPrice;
    }

    public void OnButtonBuy()
    {
        if ((GetPetToBuy().CurrencyType == CurrencyType.GOLD_PRICE ? userInformation.Gold : userInformation.Gems) >= TotalPrice)
        {
            avatarEndpoints.PurchaseItem(new List<GenericCatalogItem>() { GetPetToBuy() }).Subscribe(listBagItems =>
             {
                 //foreach (GenericBagItem bagItem in listBagItems)
                 //{
                 //    gameData.AddItemToBag(bagItem);
                 //    userInformation.SubstractGems(bagItem.ObjCat.GemsPrice);
                 //}
                 GenericCatalogItem gci = GetPetToBuy();

                 gameData.AddItemToBag(new GenericBagItem(ItemType.PETS, gci.IdItem, 1, gci));

                 if (gci.CurrencyType == CurrencyType.GOLD_PRICE)
                 {
                     userInformation.SubstractGold(gci.GoldPrice);
                 }
                 else
                 {
                     userInformation.SubstractGems(gci.GemsPrice);
                 }

                 if (OnBuyComplete != null)
                 {
                     OnBuyComplete(true);
                 }
             });
        }
        else
        {
            bool gemPurchase = GetPetToBuy().CurrencyType == CurrencyType.GEM_PRICE;
            string txtTitle = language.GetTextByKey(LangKeys.STORE_YOU_NEED_MORE_GEMS);
            string txtDesc = string.Format(language.GetTextByKey(LangKeys.PETS_YOU_CANT_PAY_NEED_MORE_GEMS), TotalPrice - gameData.GetUserInformation().Gems);
            string txtBtnAccept = language.GetTextByKey(LangKeys.STORE_GET_MORE_GEMS);
            string txtBtnDiscard = language.GetTextByKey(LangKeys.STORE_CANCEL);

            if (!gemPurchase)
            {
                txtTitle = language.GetTextByKey(LangKeys.STORE_YOU_NEED_MORE_GEMS).Replace("gems", "gold").Replace("gemas", language.GetCurrentLanguage() == "es" ? "monedas" : "ouro").Replace("mücevher", "Altın").Replace("Gems", "gold").Replace("Gemas", language.GetCurrentLanguage() == "es" ? "Monedas" : "ouro").Replace("Mücevher", "Altın");
                txtDesc = string.Format(language.GetTextByKey(LangKeys.PETS_YOU_CANT_PAY_NEED_MORE_GEMS), TotalPrice - gameData.GetUserInformation().Gold).Replace("gems", "gold").Replace("gemas", language.GetCurrentLanguage() == "es" ? "monedas" : "ouro").Replace("mücevher", "Altın").Replace("Gems", "gold").Replace("Gemas", language.GetCurrentLanguage() == "es" ? "Monedas" : "ouro").Replace("Mücevher", "Altın");
                //txtBtnAccept = language.GetTextByKey(LangKeys.STORE_GET_MORE_GEMS).Replace("gems", "gold").Replace("gemas", language.GetCurrentLanguage() == "es" ? "monedas" : "ouro").Replace("mücevher", "Altın");
                txtBtnDiscard = language.GetTextByKey(LangKeys.STORE_CANCEL);
            }

            Cancel();

            dialogPanel.Open(txtTitle, txtDesc, txtBtnAccept, txtBtnDiscard, () =>
           {
               CurrentRoom.Instance.GetRoomUIReferences().OpenStorePanel();
           }, gemPurchase: gemPurchase);

            if (OnBuyComplete != null)
            {
                OnBuyComplete(false);
            }
        }
    }

    public void Cancel()
    {

    }

    public void ContinueAfterPetPurchase(bool succeded)
    {
        if (succeded)
        {
            UICatalogManager.ChoosePet(PetToBuy, true);
        }
        else
        {
            Debug.LogError("PURCHASE FAILED");
        }
    }

}
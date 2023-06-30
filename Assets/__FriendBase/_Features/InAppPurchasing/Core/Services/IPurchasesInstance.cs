using System.Collections.Generic;

namespace InAppPurchasing.Core.Services
{
    public interface IPurchasesInstance
    {
        void Initialize(Dictionary<string, InAppPurchasingItem> items);
        void BuyItem(string idItem);
        void RestorePurchases();
        void AddCallBack(CallBackInAppPurchasing callbackInAppPurchasing);
        void DeleteCallBack(CallBackInAppPurchasing callbackInAppPurchasing);
        string GetPriceByItem(string idItem);
        bool IsInitialized();
    }
}
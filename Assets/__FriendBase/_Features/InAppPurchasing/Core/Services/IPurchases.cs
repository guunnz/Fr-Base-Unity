using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InAppPurchasing.Core;

namespace InAppPurchasing.Core.Services
{
    public delegate void CallBackInAppPurchasing(string eventValue, string idItem, bool isRestorePurchase = false);

    public interface IPurchases
    {
        void Initialize(IPurchasesInstance instance, Dictionary<string, InAppPurchasingItem> items);
        void BuyItem(string idItem);
        void RestorePurchases();
        void AddCallBack(CallBackInAppPurchasing callbackInAppPurchasing);
        void DeleteCallBack(CallBackInAppPurchasing callbackInAppPurchasing);
        string GetPriceByItem(string idItem);
        bool IsInitialized();
    }
}
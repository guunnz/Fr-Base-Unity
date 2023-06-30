using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InAppPurchasing.Core.Services;

namespace InAppPurchasing.Core
{
    public class InAppPurchasingManager : IPurchases
    {
        private IPurchasesInstance _instance;

        public void Initialize(IPurchasesInstance instance, Dictionary<string, InAppPurchasingItem> items)
        {
            _instance = instance;
            if (_instance != null)
            {
                _instance.Initialize(items);
            }
        }

        public void BuyItem(string idItem)
        {
            if (_instance != null)
            {
                _instance.BuyItem(idItem);
            }
        }

        public void RestorePurchases()
        {
            if (_instance != null)
            {
                _instance.RestorePurchases();
            }
        }

        public void AddCallBack(CallBackInAppPurchasing callback)
        {
            if (_instance != null)
            {
                _instance.AddCallBack(callback);
            }
        }

        public void DeleteCallBack(CallBackInAppPurchasing callback)
        {
            if (_instance != null)
            {
                _instance.DeleteCallBack(callback);
            }
        }

        public string GetPriceByItem(string idItem)
        {
            if (_instance != null)
            {
                return _instance.GetPriceByItem(idItem);
            }
            return "";
        }

        public bool IsInitialized()
        {
            if (_instance != null)
            {
                return _instance.IsInitialized();
            }
            return false;
        }
    }
}
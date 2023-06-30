using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InAppPurchasing.Core;
using InAppPurchasing.Core.Services;
using Architecture.Injector.Core;
using DebugConsole;
using InAppPurchasing.Custom.Unity;

namespace InAppPurchasing.Custom
{
    public class InAppPurchasingInitialize 
    {
        private Dictionary<string, InAppPurchasingItem> _items;
        private IPurchases servicePurchases = Injection.Get<IPurchases>();
        private IDebugConsole debugConsole = Injection.Get<IDebugConsole>();

        public InAppPurchasingInitialize()
        {
            
            _items = new Dictionary<string, InAppPurchasingItem>();
            _items[InAppPurchasingItemId.STORE_GEMS_TIER_1] = new InAppPurchasingItem(InAppPurchasingItemId.STORE_GEMS_TIER_1, PurchaseItemType.CONSUMABLE, "com.friendbase.ios.store_gems_tier_1", "com.friendbase.android.store_gems_tier_1", "");
            _items[InAppPurchasingItemId.STORE_GEMS_TIER_2] = new InAppPurchasingItem(InAppPurchasingItemId.STORE_GEMS_TIER_2, PurchaseItemType.CONSUMABLE, "com.friendbase.ios.store_gems_tier_2", "com.friendbase.android.store_gems_tier_2", "");
            _items[InAppPurchasingItemId.STORE_GEMS_TIER_3] = new InAppPurchasingItem(InAppPurchasingItemId.STORE_GEMS_TIER_3, PurchaseItemType.CONSUMABLE, "com.friendbase.ios.store_gems_tier_3", "com.friendbase.android.store_gems_tier_3", "");
            _items[InAppPurchasingItemId.STORE_GEMS_TIER_4] = new InAppPurchasingItem(InAppPurchasingItemId.STORE_GEMS_TIER_4, PurchaseItemType.CONSUMABLE, "com.friendbase.ios.store_gems_tier_4", "com.friendbase.android.store_gems_tier_4", "");
            _items[InAppPurchasingItemId.STORE_GEMS_TIER_5] = new InAppPurchasingItem(InAppPurchasingItemId.STORE_GEMS_TIER_5, PurchaseItemType.CONSUMABLE, "com.friendbase.ios.store_gems_tier_5", "com.friendbase.android.store_gems_tier_5", "");
            _items[InAppPurchasingItemId.STORE_GEMS_TIER_6] = new InAppPurchasingItem(InAppPurchasingItemId.STORE_GEMS_TIER_6, PurchaseItemType.CONSUMABLE, "com.friendbase.ios.store_gems_tier_6", "com.friendbase.android.store_gems_tier_6", "");
           
            servicePurchases.Initialize(new InAppPurchasingUnityManager(), _items);
            servicePurchases.AddCallBack(CallBackFunction);
        }

        void CallBackFunction(string eventValue, string idItem, bool isRestorePurchase = false)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_Manager:CallBackFunction: eventValue" + eventValue + " idItem:" + idItem);

            //TODO add analytics event
            //J2DM_AnalyticsManager.Instance.EventPurchaseItem(bankBundleCatalogItem.idItem.ToString(), bankBundleCatalogItem.namePrefab, bankBundleCatalogItem.itemType.ToString(), bankBundleCatalogItem.price, null);
        }
    }
}


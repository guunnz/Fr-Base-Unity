using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;
using DebugConsole;
using InAppPurchasing.Core;
using InAppPurchasing.Core.Services;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using UniRx;
using Newtonsoft.Json.Linq;

namespace InAppPurchasing.Custom.Unity
{
    public class InAppPurchasingUnityManager : IPurchasesInstance, IStoreListener
    {
        private ProductType[] _arrayProductType = new ProductType[] { ProductType.Consumable, ProductType.NonConsumable, ProductType.Subscription };

        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        private static event CallBackInAppPurchasing OnCallBackFunction;
        private static Dictionary<string, InAppPurchasingItem> _items;

        private string _currentItemIdInProccess;

        private IDebugConsole debugConsole = Injection.Get<IDebugConsole>();

        public void Initialize(Dictionary<string, InAppPurchasingItem> items)
        {
            if (m_StoreController == null)
            {
                InitializePurchasing(items);
            }
        }

        private void InitializePurchasing(Dictionary<string, InAppPurchasingItem> items)
        {
            if (IsInitialized())
            {
                return;
            }

            _items = items;

            if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:InitializePurchasing");

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (KeyValuePair<string, InAppPurchasingItem> item in _items)
            {
                builder.AddProduct(item.Key, _arrayProductType[(int)item.Value.itemType], new IDs() { { item.Value.idApple, AppleAppStore.Name }, { item.Value.idGoogle, GooglePlay.Name }, });
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public bool IsInitialized()
        {
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        public void BuyItem(string idItem)
        {
            Debug.Log("------------ BuyItem idItem:"+ idItem);
            if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:BuyItem idItem:" + idItem);

            try
            {
                Debug.Log("------------ BuyItem 00");
                if (IsInitialized())
                {
                    Debug.Log("------------ BuyItem 01");
                    Product itemToBuy = m_StoreController.products.WithID(idItem);
                    Debug.Log("------------ BuyItem 02 itemToBuy:"+ itemToBuy);
                    if (itemToBuy != null && itemToBuy.availableToPurchase)
                    {
                        Debug.Log("------------ BuyItem 03");
                        _currentItemIdInProccess = idItem;
                        if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:BuyItem. Purchasing product asychronously:" + idItem);
                        m_StoreController.InitiatePurchase(itemToBuy);
                    }
                    else
                    {
                        Debug.Log("------------ BuyItem 04");
                        if (OnCallBackFunction != null)
                        {
                            OnCallBackFunction(InAppPurchasingEvents.PURCHASE_FAIL, idItem);
                        }
                        if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:BuyItem Fail. Product not Found, idItem:" + idItem);
                    }
                }
                else
                {
                    Debug.Log("------------ BuyItem 05");
                    if (OnCallBackFunction != null)
                    {
                        OnCallBackFunction(InAppPurchasingEvents.PURCHASE_FAIL, idItem);
                    }
                    if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:BuyItem Fail. API Not initialized, idItem:" + idItem);
                }
            }
            catch (Exception e)
            {
                Debug.Log("------------ BuyItem Exception");
                if (OnCallBackFunction != null)
                {
                    OnCallBackFunction(InAppPurchasingEvents.PURCHASE_FAIL, idItem);
                }
                if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:BuyItem Fail. Exception during purchase, idItem:" + idItem + " ,error:" + e);
            }
        }

        public void RestorePurchases()
        {
            if (!IsInitialized())
            {
                if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:RestorePurchases Fail. API Not initialized.");
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
            {
                if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:RestorePurchases started...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) =>
                {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
                    if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                    if (result)
                    {
                        if (OnCallBackFunction != null)
                        {
                            OnCallBackFunction(InAppPurchasingEvents.RESTORE_PURCHASE_COMPLETE, "");
                        }
                    }
                    else
                    {
                        if (OnCallBackFunction != null)
                        {
                            OnCallBackFunction(InAppPurchasingEvents.RESTORE_PURCHASE_FAIL, "");
                        }
                    }
                });
            }
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

        public void AddCallBack(CallBackInAppPurchasing callback)
        {
            OnCallBackFunction += callback;
        }

        public void DeleteCallBack(CallBackInAppPurchasing callback)
        {
            OnCallBackFunction -= callback;
        }

        public string GetPriceByItem(string idItem)
        {
            if (!IsInitialized())
            {
                return "";
            }
            string price = "";
            try
            {
                Product product = m_StoreController.products.WithID(idItem);
                if (product != null)
                {
                    price = product.metadata.localizedPriceString;
                }
                if (price == null)
                {
                    price = "";
                }
            }
            catch (Exception e) { }
            return price;
        }

        //--------------------------------------------------------------
        //--------------------------------------------------------------
        //-------------------   C A L L B A C K S   --------------------
        //--------------------------------------------------------------
        //--------------------------------------------------------------

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:OnInitialized");
            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;
            if (OnCallBackFunction != null)
            {
                OnCallBackFunction(InAppPurchasingEvents.INIT_SUCCESS, "");
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:OnInitializeFailed reason:" + error);
            if (OnCallBackFunction != null)
            {
                OnCallBackFunction(InAppPurchasingEvents.INIT_FAIL, "");
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            string idItem = args.purchasedProduct.definition.id;
            if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:ProcessPurchase idItem:" + idItem);

            if (_items[idItem] != null)
            {
                if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:ProcessPurchase Complete idItem:" + idItem);

                //bool isRestorePurchasing = true;
                //if (_currentItemIdInProccess.Equals(idItem))
                //{
                //    isRestorePurchasing = false;
                //}
                //if (OnCallBackFunction != null)
                //{
                //    OnCallBackFunction(InAppPurchasingEvents.PURCHASE_COMPLETE, idItem, isRestorePurchasing);
                //}

                //return PurchaseProcessingResult.Complete;

#if UNITY_ANDROID
                JObject jsonReceipt = JObject.Parse(args.purchasedProduct.receipt);

                string payload = jsonReceipt["Payload"].Value<string>();
                JObject jsonPayload = JObject.Parse(payload);

                string jsonNode = jsonPayload["json"].Value<string>();
                JObject jsonJObject = JObject.Parse(jsonNode);
                string token = jsonJObject["purchaseToken"].Value<string>();

                string productId = jsonJObject["productId"].Value<string>();

                Injection.Get<IAvatarEndpoints>().PurchaseValidationGoogle(productId, token).Subscribe(flagSucceed =>
                {
                    if (flagSucceed)
                    {
                        bool isRestorePurchasing = true;
                        if (_currentItemIdInProccess.Equals(idItem))
                        {
                            isRestorePurchasing = false;
                        }
                        if (OnCallBackFunction != null)
                        {
                            OnCallBackFunction(InAppPurchasingEvents.PURCHASE_COMPLETE, idItem, isRestorePurchasing);
                        }
                    }
                    else
                    {
                        if (OnCallBackFunction != null)
                        {
                            OnCallBackFunction(InAppPurchasingEvents.PURCHASE_FAIL, idItem);
                        }
                    }
                    m_StoreController.ConfirmPendingPurchase(args.purchasedProduct);
                });





#elif UNITY_IPHONE
                JObject jsonReceipt = JObject.Parse(args.purchasedProduct.receipt);
                string receipt = jsonReceipt["Payload"].Value<string>();

                Injection.Get<IAvatarEndpoints>().PurchaseValidation(receipt, args.purchasedProduct.transactionID).Subscribe(flagSucceed =>
                {
                    if (flagSucceed)
                    {
                        bool isRestorePurchasing = true;
                        if (_currentItemIdInProccess.Equals(idItem))
                        {
                            isRestorePurchasing = false;
                        }
                        if (OnCallBackFunction != null)
                        {
                            OnCallBackFunction(InAppPurchasingEvents.PURCHASE_COMPLETE, idItem, isRestorePurchasing);
                        }
                    }
                    else
                    {
                        if (OnCallBackFunction != null)
                        {
                            OnCallBackFunction(InAppPurchasingEvents.PURCHASE_FAIL, idItem);
                        }
                    }
                    m_StoreController.ConfirmPendingPurchase(args.purchasedProduct);
                });
#endif
                return PurchaseProcessingResult.Pending;
            }
            else
            {
                if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, "IAP_UnityManager:ProcessPurchase FAIL. Unrecognized product:" + args.purchasedProduct.definition.id);

                if (OnCallBackFunction != null)
                {
                    OnCallBackFunction(InAppPurchasingEvents.PURCHASE_FAIL, idItem);
                }

                return PurchaseProcessingResult.Complete;
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            if (OnCallBackFunction != null)
            {
                OnCallBackFunction(InAppPurchasingEvents.PURCHASE_FAIL, product.definition.id);
            }
            if (debugConsole.isLogTypeEnable(LOG_TYPE.IAP)) debugConsole.TraceLog(LOG_TYPE.IAP, string.Format("IAP_UnityManager:OnPurchaseFailed, Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }
    }
}
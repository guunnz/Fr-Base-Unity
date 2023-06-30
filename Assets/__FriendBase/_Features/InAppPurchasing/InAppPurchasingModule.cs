using Architecture.Context;
using Architecture.Injector.Core;
using InAppPurchasing.Core.Services;
using InAppPurchasing.Core;
using InAppPurchasing.Custom.Unity;
using InAppPurchasing.Custom;

namespace InAppPurchasing
{
    public class InAppPurchasingModule : IModule
    {
        public void Init()
        {
            Injection.Register<IPurchases, InAppPurchasingManager>();
            Injection.Register<IPurchasesInstance, InAppPurchasingUnityManager>();

            new InAppPurchasingInitialize();
        }
    }
}
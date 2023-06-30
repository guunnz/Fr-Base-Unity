using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InAppPurchasing.Core
{
    public enum PurchaseItemType { CONSUMABLE = 0, NON_CONSUMABLE = 1, SUBSCRIPTION = 2 };

    public class InAppPurchasingItem
    {
        public string name;
        public PurchaseItemType itemType;
        public string idApple;
        public string idGoogle;
        public string idWebGl;

        public InAppPurchasingItem(string name, PurchaseItemType itemType, string idApple, string idGoogle, string idWebGl)
        {
            this.name = name;
            this.itemType = itemType;
            this.idApple = idApple;
            this.idGoogle = idGoogle;
            this.idWebGl = idWebGl;
        }
    }
}
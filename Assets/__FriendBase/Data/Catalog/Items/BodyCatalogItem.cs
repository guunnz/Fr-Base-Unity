using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Catalog
{
    public class BodyCatalogItem : AvatarGenericCatalogItem
    {
        public bool UseBoobs { get; private set; }

        public BodyCatalogItem(ItemType itemType, int idItemWebClient, int idItem, string nameItem, string namePrefab, int orderInCatalog, bool activeInCatalog, int gemsPrice, int goldPrice, CurrencyType currencyType, int[] layers, bool useBoobs) :
                base(itemType, idItemWebClient, idItem, nameItem, namePrefab, orderInCatalog, activeInCatalog, gemsPrice, goldPrice, currencyType, layers)
        {
            UseBoobs = useBoobs;
        }
    }
}


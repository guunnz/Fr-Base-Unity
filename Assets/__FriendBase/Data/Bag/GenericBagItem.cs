using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Catalog;

namespace Data.Bag
{
    public class GenericBagItem
    {
        public ItemType ItemType { get; }
        public int IdInstance { get; private set; }
        public GenericCatalogItem ObjCat { get; private set; }
        public int Amount { get; set; }

        public GenericBagItem(ItemType itemType, int idInstance, int amount, GenericCatalogItem objCat)
        {
            ItemType = itemType;
            IdInstance = idInstance;
            Amount = amount;
            ObjCat = objCat;
        }
    }
}
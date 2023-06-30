using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Catalog.Items
{
    public interface IItemTypeUtils
    {
        string GetNameItemType(ItemType itemType);
        ItemType GetItemTypeByName(string itemTypeName);
        string GetNameColorItemType(ItemType itemType);
        bool IsFurnitureType(ItemType itemType);
    }
}


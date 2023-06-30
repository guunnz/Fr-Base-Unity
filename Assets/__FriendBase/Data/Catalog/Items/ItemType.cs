using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using UnityEngine;
using Data.Catalog.Items;

namespace Data.Catalog
{
    public enum ItemType
    {
        NONE = -1,
        BODY = 0,
        EYE = 1,
        EAR = 2,
        EYEBROW = 3,
        HAIR = 4,
        FACE = 5,
        MOUTH = 6,
        NOSE = 7,
        UP_PART = 8,
        BOTTOM_PART = 9,
        DRESSES = 10,
        SHOES = 11,
        GLASSES = 12,
        ACCESORIES = 13,
        PETS = 14,

        FURNITURES_INVENTORY = 100,
        CHAIR = 101,
        FLOOR = 102,
        LAMP = 103,
        TABLE = 104,
        

        ROOM = 1001,
        COLOR = 1002,
    }

    public static class ItemTypeExtension
    {
        static IItemTypeUtils utils;

        public static string GetSerializationName(this ItemType type)
        {
            utils ??= Injection.Get<IItemTypeUtils>();
            return utils.GetNameItemType(type);
        }
    }

    public class ItemTypeUtils : IItemTypeUtils
    {
        public Dictionary<int, string> dictionaryItemTypes = new Dictionary<int, string>();
        public Dictionary<int, string> dictionaryColorItemTypes = new Dictionary<int, string>();

        public ItemTypeUtils()
        {
            dictionaryItemTypes = new Dictionary<int, string>
            {
                {(int) ItemType.BODY, "body"},
                {(int) ItemType.EYE, "eyes"},
                {(int) ItemType.EAR, "ear"},
                {(int) ItemType.EYEBROW, "eyebrows"},
                {(int) ItemType.HAIR, "hair"},
                {(int) ItemType.FACE, "head"},
                {(int) ItemType.MOUTH, "lips"},
                {(int) ItemType.NOSE, "nose"},
                {(int) ItemType.UP_PART, "shirt"},
                {(int) ItemType.BOTTOM_PART, "pants"},
                {(int) ItemType.DRESSES, "dress"},
                {(int) ItemType.SHOES, "shoes"},
                {(int) ItemType.GLASSES, "glasses"},
                {(int) ItemType.ACCESORIES, "accesory"},

                {(int) ItemType.CHAIR, "chair"},
                {(int) ItemType.TABLE, "table"},
                {(int) ItemType.FLOOR, "floor"},
                {(int) ItemType.LAMP, "lamp"},
                {(int) ItemType.FURNITURES_INVENTORY, "inventory"},
                {(int) ItemType.ROOM, "room"},
                {(int) ItemType.PETS, "pet"}
            };

            dictionaryColorItemTypes = new Dictionary<int, string>
            {
                {(int)ItemType.BODY, "skin_color" },
                {(int)ItemType.EYE, "eye_color"},
                {(int)ItemType.MOUTH, "lip_color"},
                {(int)ItemType.HAIR, "hair_color"},
                {(int)ItemType.EYEBROW, "eyebrow_color"}
        };
        }

        public string GetNameItemType(ItemType itemType)
        {
            string itemTypeName = "";
            if (dictionaryItemTypes.ContainsKey((int) itemType))
            {
                return dictionaryItemTypes[(int) itemType];
            }

            return itemTypeName;
        }

        public ItemType GetItemTypeByName(string itemTypeName)
        {
            foreach (KeyValuePair<int, string> item in dictionaryItemTypes)
            {
                if (item.Value.Equals(itemTypeName))
                {
                    return (ItemType) item.Key;
                }
            }

            return ItemType.NONE;
        }

        public string GetNameColorItemType(ItemType itemType)
        {
            string itemColorTypeName = "";
            if (dictionaryColorItemTypes.ContainsKey((int)itemType))
            {
                return dictionaryColorItemTypes[(int)itemType];
            }
            return itemColorTypeName;
        }

        public bool IsFurnitureType(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.CHAIR:
                case ItemType.TABLE:
                case ItemType.FLOOR:
                case ItemType.LAMP:
                    return true;
            }
            return false;
        }
    }
}
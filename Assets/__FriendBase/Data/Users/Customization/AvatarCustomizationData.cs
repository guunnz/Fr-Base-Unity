using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Catalog;
using Data.Bag;
using Data;
using Architecture.Injector.Core;
using DebugConsole;
using Newtonsoft.Json.Linq;
using Data.Catalog.Items;
using System;

namespace Data.Users
{
    public class AvatarCustomizationData
    {
        private IDebugConsole debugConsole;

        private Dictionary<ItemType, AvatarCustomizationDataUnit> dataUnits;

        private IGameData gameData = Injection.Get<IGameData>();
        private Dictionary<ItemType, AvatarCustomizationRule> rules;
        private IItemTypeUtils ItemTypeUtils = Injection.Get<IItemTypeUtils>();

        public AvatarCustomizationData()
        {
            debugConsole = Injection.Get<IDebugConsole>();
            rules = gameData.GetAvatarCustomizationRules();

            dataUnits = new Dictionary<ItemType, AvatarCustomizationDataUnit>();
            foreach (ItemType itemType in GameData.AvatarItemsType)
            {
                AvatarCustomizationRule rule = rules[itemType];
                AvatarGenericCatalogItem avatarGenericCatalogItem = gameData.GetCatalogByItemType(itemType).GetItem(rule.IdItemDefault) as AvatarGenericCatalogItem;
                ColorCatalogItem colorCatalogItem = gameData.GetCatalogByItemType(ItemType.COLOR).GetItem(rule.IdColorDefault) as ColorCatalogItem;
                dataUnits.Add(itemType, new AvatarCustomizationDataUnit(itemType: itemType, avatarObjCat: avatarGenericCatalogItem, colorObjCat: colorCatalogItem));
            }
        }

        public AvatarCustomizationData(Dictionary<ItemType, AvatarCustomizationDataUnit> dataUnits)
        {
            debugConsole = Injection.Get<IDebugConsole>();
            rules = gameData.GetAvatarCustomizationRules();

            this.dataUnits = dataUnits;
        }

        public bool IsBoobsActive()
        {
            AvatarGenericCatalogItem avatarGenericCatalogItem = dataUnits[ItemType.BODY].AvatarObjCat;
            BodyCatalogItem bodyCatalogItem = avatarGenericCatalogItem as BodyCatalogItem;
            if (bodyCatalogItem != null)
            {
                return bodyCatalogItem.UseBoobs;
            }
            return false;
        }

        public void SetDataFromJoinRoom(JObject avatarCustomizationJsonData)
        {
            //JObject dataJson = avatarCustomizationJsonData["user_avatar"].Value<JObject>();
            SetData(avatarCustomizationJsonData);
            // Debug.LogError(avatarCustomizationJsonData.ToString());
        }

        public void SetDataFromUserSkin(JObject avatarCustomizationJsonData, bool getDataFromJson = true)
        {
            if (getDataFromJson)
            {
                JObject dataJson = avatarCustomizationJsonData["data"].Value<JObject>();
                SetData(dataJson);
            }
            else
            {
                SetData(avatarCustomizationJsonData);
            }
        }

        private void SetData(JObject dataJson)
        {
            if (dataJson == null)
            {
                debugConsole.ErrorLog("AvatarCustomizationData:SetData", "invalid dataJson", "");
                return;
            }

            foreach (ItemType itemType in GameData.AvatarItemsType)
            {
                try
                {
                    JObject itemJson = dataJson[ItemTypeUtils.GetNameItemType(itemType)].Value<JObject>();
                    if (itemJson != null)
                    {
                        string sIdItem = itemJson["id_in_game"].Value<string>();
                        int idItem = int.Parse(sIdItem);

                        AvatarCustomizationRule rule = rules[itemType];
                        AvatarGenericCatalogItem avatarGenericCatalogItem = gameData.GetCatalogByItemType(itemType).GetItem(idItem) as AvatarGenericCatalogItem;

                        if (avatarGenericCatalogItem != null || rule.Deselectable)
                        {
                            ChangePart(itemType, avatarGenericCatalogItem);
                        }
                        else
                        {
                            debugConsole.ErrorLog("AvatarCustomizationData:SetData JObject", "idItem Not Found", "itemType:" + itemType + " idItem:" + idItem);
                        }

                        if (rule.ColorIdsAvailable != null && rule.ColorIdsAvailable.Length > 0)
                        {
                            string nameColor = ItemTypeUtils.GetNameColorItemType(itemType);
                            int idItemColor = dataJson[nameColor].Value<int>();

                            ColorCatalogItem colorCatalogItem = gameData.GetCatalogByItemType(ItemType.COLOR).GetItem(idItemColor) as ColorCatalogItem;
                            if (colorCatalogItem != null)
                            {
                                ChangeColorPart(itemType, colorCatalogItem);
                            }
                            else
                            {
                                debugConsole.ErrorLog("AvatarCustomizationData:SetData JObject", "idItemColor Not Found", "itemType:" + itemType + " idItemColor:" + idItemColor);
                            }
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        public void SetDataFromFriend(JObject avatarCustomizationJsonData)
        {
            JObject dataJson = avatarCustomizationJsonData["friend"].Value<JObject>();
            SetDataFriend(dataJson);
        }

        public void SetDataFriend(JObject dataJson)
        {
            if (dataJson == null)
            {
                debugConsole.ErrorLog("AvatarCustomizationData:SetDataFriend", "invalid dataJson", "");
                return;
            }

            foreach (ItemType itemType in GameData.AvatarItemsType)
            {
                try
                {
                    string nameBodyPart = ItemTypeUtils.GetNameItemType(itemType) + "_id";
                    int idItem = dataJson[nameBodyPart].Value<int>();

                    AvatarCustomizationRule rule = rules[itemType];
                    AvatarGenericCatalogItem avatarGenericCatalogItem = gameData.GetCatalogByItemType(itemType).GetItemByIdWebClient(idItem) as AvatarGenericCatalogItem;

                    if (avatarGenericCatalogItem != null || rule.Deselectable)
                    {
                        ChangePart(itemType, avatarGenericCatalogItem);
                    }
                    else
                    {
                        debugConsole.ErrorLog("AvatarCustomizationData:SetData JObject", "idItem Not Found", "itemType:" + itemType + " idItem:" + idItem);
                    }

                    if (rule.ColorIdsAvailable != null && rule.ColorIdsAvailable.Length > 0)
                    {
                        string nameColor = ItemTypeUtils.GetNameColorItemType(itemType);
                        int idItemColor = dataJson[nameColor].Value<int>();

                        ColorCatalogItem colorCatalogItem = gameData.GetCatalogByItemType(ItemType.COLOR).GetItem(idItemColor) as ColorCatalogItem;
                        if (colorCatalogItem != null)
                        {
                            ChangeColorPart(itemType, colorCatalogItem);
                        }
                        else
                        {
                            debugConsole.ErrorLog("AvatarCustomizationData:SetData JObject", "idItemColor Not Found", "itemType:" + itemType + " idItemColor:" + idItemColor);
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        public void SetData(AvatarCustomizationData avatarCustomizationData)
        {
            foreach (ItemType itemType in GameData.AvatarItemsType)
            {
                dataUnits[itemType].SetData(avatarCustomizationData.GetDataUnit(itemType));
            }
        }

        public AvatarCustomizationDataUnit GetDataUnit(ItemType itemType)
        {
            if (dataUnits.ContainsKey(itemType))
            {
                return dataUnits[itemType];
            }

            return null;
        }

        public JObject GetSerializeDataWebClient()
        {
            JObject avatarJson = new JObject();

            foreach (ItemType itemType in GameData.AvatarItemsType)
            {
                AvatarGenericCatalogItem objCat = dataUnits[itemType].AvatarObjCat;
                if (objCat != null)
                {
                    avatarJson[ItemTypeUtils.GetNameItemType(itemType) + "_id"] = objCat.IdItemWebClient;
                }
                else
                {
                    avatarJson[ItemTypeUtils.GetNameItemType(itemType) + "_id"] = null;
                }

                AvatarCustomizationRule rule = rules[itemType];
                if (rule.ColorIdsAvailable != null && rule.ColorIdsAvailable.Length > 0)
                {
                    avatarJson[ItemTypeUtils.GetNameColorItemType(itemType)] = dataUnits[itemType].ColorObjCat.IdItem;
                }
            }

            JObject json = new JObject { ["avatar"] = avatarJson };
            return json;
        }

        public AvatarCustomizationSimpleData GetSerializeData()
        {
            int amount = GameData.AvatarItemsType.Length;

            AvatarCustomizationSimpleDataUnit[] simpleDataUnits = new AvatarCustomizationSimpleDataUnit[amount];

            for (int i = 0; i < amount; i++)
            {
                int idItem = -1;
                ItemType itemType = GameData.AvatarItemsType[i];
                if (dataUnits[itemType].AvatarObjCat != null)
                {
                    idItem = dataUnits[itemType].AvatarObjCat.IdItem;
                }
                int idColor = dataUnits[itemType].ColorObjCat.IdItem;
                simpleDataUnits[i] = new AvatarCustomizationSimpleDataUnit((int)itemType, idItem, idColor);
            }

            AvatarCustomizationSimpleData avatarData = new AvatarCustomizationSimpleData(simpleDataUnits);
            return avatarData;
        }

        public void ChangePart(ItemType itemType, AvatarGenericCatalogItem catalogItem)
        {
            if (dataUnits.ContainsKey(itemType))
            {
                dataUnits[itemType].SetAvatarGenericCatalogItem(catalogItem);
            }
            else
            {
                debugConsole.ErrorLog("AvatarCustomizationData:ChangePart", "ItemType Not Found",
                    "itemType:" + itemType);
            }
        }

        public void ChangeColorPart(ItemType itemType, ColorCatalogItem colorCatalogItem)
        {
            if (dataUnits.ContainsKey(itemType))
            {
                dataUnits[itemType].SetColorObjCat(colorCatalogItem);
            }
            else
            {
                debugConsole.ErrorLog("AvatarCustomizationData:ChangeColorPart", "ItemType Not Found",
                    "itemType:" + itemType + " IdItem:" + colorCatalogItem.IdItem);
            }
        }

        public void DisableItemType(ItemType itemType)
        {
            dataUnits[itemType].DisableAvatarObjCat();
        }

        public bool HasAllItemsOnInventory()
        {
            foreach (ItemType itemType in GameData.AvatarItemsType)
            {
                AvatarGenericCatalogItem avatarCatalogItem = GetDataUnit(itemType).AvatarObjCat;
                if (avatarCatalogItem != null)
                {
                    bool flag = gameData.GetBagByItemType(itemType).HasAnyIdItem(avatarCatalogItem.IdItem);
                    if (!flag)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public List<GenericCatalogItem> GetListItemsMissingOnInventory()
        {
            List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

            foreach (ItemType itemType in GameData.AvatarItemsType)
            {
                AvatarGenericCatalogItem avatarCatalogItem = GetDataUnit(itemType).AvatarObjCat;
                if (avatarCatalogItem != null)
                {
                    bool flag = gameData.GetBagByItemType(itemType).HasAnyIdItem(avatarCatalogItem.IdItem);
                    if (!flag)
                    {
                        listItems.Add(avatarCatalogItem);
                    }
                }
            }
            return listItems;
        }
    }
}
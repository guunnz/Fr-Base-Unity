using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture.Injector.Core;
using DebugConsole;

namespace Data.Catalog
{
    public class AvatarGenericCatalogItem : GenericCatalogItem
    {
        public int[] Layers { get; private set; }
        static IGameData gameData = Injection.Get<IGameData>();
        public string PsbName { get; private set; }
        public string PsbNameUI { get; private set; }

        public AvatarGenericCatalogItem(ItemType itemType, int idItemWebClient, int idItem, string nameItem, string namePrefab, int orderInCatalog, bool activeInCatalog, int gemsPrice, int goldPrice, CurrencyType currencyType, int[] layers):
            base(itemType, idItemWebClient, idItem, nameItem, namePrefab, orderInCatalog, activeInCatalog, gemsPrice, goldPrice, currencyType)
        {
            Layers = layers;
        }

        public AvatarGenericCatalogItem(ItemType itemType, int idItemWebClient, int idItem, string nameItem, string namePrefab, int orderInCatalog, bool activeInCatalog, int gemsPrice, int goldPrice, CurrencyType currencyType, int[] layers, string psbName, string psbNameUI, string limitedEdition) :
            base(itemType, idItemWebClient, idItem, nameItem, namePrefab, orderInCatalog, activeInCatalog, gemsPrice, goldPrice, currencyType)
        {
            Layers = layers;
            PsbName = psbName;
            PsbNameUI = psbNameUI;
            LimitedEdition = limitedEdition;
        }

        public string GetNamePrefabByItem(int index, bool boobActive)
        {
            AvatarCustomizationRule rule = gameData.GetAvatarCustomizationRuleByItemType(ItemType);
            if (rule==null)
            {
                return null;
            }

            string psbName = rule.PsbName;
            if (!string.IsNullOrEmpty(this.PsbName))
            {
                psbName = this.PsbName;
            }

            string name = psbName + "[" + NamePrefab;
            if (!string.IsNullOrEmpty(rule.SubfixForGameObjects[index]))
            {
                name += "_" + rule.SubfixForGameObjects[index];
            }
            //If the item use boobs and the layer is the boobLayer
            if (boobActive && rule.UseBoobs == index )
            {
                name += "_B";
            }

            name += "]";
            return name;
        }

        public string GetNamePrefabUIByItem(bool boobActive, int idColor)
        {
            AvatarCustomizationRule rule = gameData.GetAvatarCustomizationRuleByItemType(ItemType);

            string namePrefab = NamePrefab;
            if (boobActive && rule.UseBoobs!=-1)
            {
                namePrefab += "_B";
            }

            if (rule.CanDisableColor && idColor==0)
            {
                namePrefab += "_NC";
            }

            string psbNameUI = rule.PsbNameUI;
            if (!string.IsNullOrEmpty(this.PsbNameUI))
            {
                psbNameUI = this.PsbNameUI;
            }

            namePrefab = psbNameUI + "[" + namePrefab + "]";

            return namePrefab;
        }
    }
}
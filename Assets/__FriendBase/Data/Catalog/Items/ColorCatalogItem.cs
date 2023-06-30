using UnityEngine;

namespace Data.Catalog
{
    public class ColorCatalogItem : GenericCatalogItem
    {
        public Color Color { get;}

        public ColorCatalogItem(ItemType itemType, int idItemWebClient, int idItem, string nameItem, string namePrefab, int orderInCatalog, bool activeInCatalog, int gemsPrice, int goldPrice, CurrencyType currencyType, string color)
            : base(itemType, idItemWebClient, idItem, nameItem, namePrefab, orderInCatalog, activeInCatalog, gemsPrice, goldPrice, currencyType)
        {
            Color = GameObjectUtils.HexToColor(color);
        }
    }
}
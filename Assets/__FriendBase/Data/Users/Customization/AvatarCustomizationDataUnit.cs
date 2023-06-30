using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Catalog;

namespace Data.Users
{
    public class AvatarCustomizationDataUnit
    {
        public ItemType ItemType { get; }
        public AvatarGenericCatalogItem AvatarObjCat { get; private set; }
        public ColorCatalogItem ColorObjCat { get; private set; }
        public AvatarGenericCatalogItem LastAvatarObjCat { get; private set; } //saving the last id if we need to put it on null (ex:dress -> top & pants)

        public AvatarCustomizationDataUnit(ItemType itemType, AvatarGenericCatalogItem avatarObjCat, ColorCatalogItem colorObjCat)
        {
            ItemType = itemType;
            AvatarObjCat = avatarObjCat;
            ColorObjCat = colorObjCat;
            LastAvatarObjCat = avatarObjCat;
        }

        public void SetData(AvatarCustomizationDataUnit avatarCustomizationData)
        {
            AvatarObjCat = avatarCustomizationData.AvatarObjCat;
            ColorObjCat = avatarCustomizationData.ColorObjCat;
            LastAvatarObjCat = avatarCustomizationData.AvatarObjCat;
        }

        public void SetAvatarGenericCatalogItem(AvatarGenericCatalogItem avatarObjCat)
        {
            AvatarObjCat = avatarObjCat;
            LastAvatarObjCat = avatarObjCat;
        }

        public void DisableAvatarObjCat()
        {
            AvatarObjCat = null;
        }

        public void SetColorObjCat(ColorCatalogItem colorObjCat)
        {
            ColorObjCat = colorObjCat;
        }

    }
}


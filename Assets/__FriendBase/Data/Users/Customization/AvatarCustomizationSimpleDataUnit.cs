using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Catalog;
using Data;

public class AvatarCustomizationSimpleDataUnit 
{
    
    
    public int ItemType { get; private set; }
    public int IdItem { get; private set; }
    public int IdColor { get; private set; }

    public AvatarCustomizationSimpleDataUnit(int itemType, int idItem, int idColor)
    {
        ItemType = itemType;
        IdItem = idItem;
        IdColor = idColor;
    }
}

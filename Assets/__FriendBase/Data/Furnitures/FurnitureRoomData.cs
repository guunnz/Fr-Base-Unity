using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Catalog;

public class FurnitureRoomData 
{
    public GenericCatalogItem ObjCat { get; private set; }
    public int IdInstance { get; private set; }
    public int IdInventoryItem { get; private set; }
    public Vector2 Position { get; set; }
    public int Orientation { get; set; }

    public FurnitureRoomData(GenericCatalogItem objCat, int idInstance, int idInventoryItem, Vector2 position, int orientation)
    {
        ObjCat = objCat;
        IdInstance = idInstance;
        IdInventoryItem = idInventoryItem;
        Position = position;
        Orientation = orientation;
    }
}

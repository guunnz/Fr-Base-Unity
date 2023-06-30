using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Catalog.Items;
using Data.Catalog;
using Data.Bag;
using Data;
using Architecture.Injector.Core;
using System.IO;

public class LocalBag 
{
    public LocalBag(Dictionary<ItemType, GenericCatalog> catalogsDictionary, Dictionary<ItemType, GenericBag> bagsDictionary)
    {
        //GenericBag faceBag = bagsDictionary[ItemType.FACE];
        //GenericCatalog faceCatalog = catalogsDictionary[ItemType.FACE];

        //List<GenericBagItem> listFaces = new List<GenericBagItem>();
        //listFaces.Add(new GenericBagItem(ItemType.FACE, "0", 1, faceCatalog.GetItem(0)));
        //listFaces.Add(new GenericBagItem(ItemType.FACE, "1", 1, faceCatalog.GetItem(1)));
        //listFaces.Add(new GenericBagItem(ItemType.FACE, "2", 1, faceCatalog.GetItem(2)));

        //faceBag.Initialize(listFaces);


        foreach (ItemType itemType in GameData.RoomItemsType)
        {
            if (itemType != ItemType.FURNITURES_INVENTORY)
            {
                GenericBag bag = bagsDictionary[itemType];
                GenericCatalog catalog = catalogsDictionary[itemType];

                List<GenericBagItem> listItems = new List<GenericBagItem>();

                listItems.Add(new GenericBagItem(itemType, 0, 1, catalog.GetItem(0)));
                listItems.Add(new GenericBagItem(itemType, 1, 1, catalog.GetItem(1)));

                //bag.Initialize(listItems);
            }
        }

        //List<GenericBagItem> listItems = new List<GenericBagItem>();
        //listItems.Add(new GenericBagItem(itemType, 1, 1, catalog.GetItem(1)));

        ////AddRoom to bag
        //GenericBag bagRooms = bagsDictionary[ItemType.ROOM];
        //GenericCatalog catalogRooms = catalogsDictionary[ItemType.ROOM];
        //List<GenericBagItem> listRooms = new List<GenericBagItem>();
        //listRooms.Add(new GenericBagItem(ItemType.ROOM, "0", 1, catalogRooms.GetItem(0)));
        //bagRooms.Initialize(listRooms);
    }
}

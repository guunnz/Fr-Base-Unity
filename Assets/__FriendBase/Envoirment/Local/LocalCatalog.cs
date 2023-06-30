using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Catalog.Items;
using Data.Catalog;
using Data;
using Architecture.Injector.Core;
using System.IO;

public class LocalCatalog
{
    private Dictionary<ItemType, List<GenericCatalogItem>> catalogLists;

    public LocalCatalog(Dictionary<ItemType, GenericCatalog> catalogsDictionary)
    {
        catalogLists = new Dictionary<ItemType, List<GenericCatalogItem>>();

        //Initialize(catalogsDictionary);
        //InitializeFurnitures(catalogsDictionary);
        //InitializeRooms(catalogsDictionary);

        InitializeColors(catalogsDictionary);

        //SerializeListFurnituresAndRooms();
        //SerializeListItems();
        //InitializePets(catalogsDictionary);
    }

    void InitializeColors(Dictionary<ItemType, GenericCatalog> catalogsDictionary)
    {
        catalogLists.Add(ItemType.COLOR, GetColorCatalogItems());
        catalogsDictionary[ItemType.COLOR].Initialize(GetListItemsByItemType(ItemType.COLOR));
    }

    void InitializeRooms(Dictionary<ItemType, GenericCatalog> catalogsDictionary)
    {
        catalogLists.Add(ItemType.ROOM, GetRoomsCatalogItem());

        catalogsDictionary[ItemType.ROOM].Initialize(GetListItemsByItemType(ItemType.ROOM));
    }

    void InitializeFurnitures(Dictionary<ItemType, GenericCatalog> catalogsDictionary)
    {
        catalogLists.Add(ItemType.CHAIR, GetChairCatalogItem());
        catalogLists.Add(ItemType.FLOOR, GetFloorCatalogItem());
        catalogLists.Add(ItemType.LAMP, GetLampCatalogItem());
        catalogLists.Add(ItemType.TABLE, GetTablesCatalogItem());

        catalogsDictionary[ItemType.CHAIR].Initialize(GetListItemsByItemType(ItemType.CHAIR));
        catalogsDictionary[ItemType.FLOOR].Initialize(GetListItemsByItemType(ItemType.FLOOR));
        catalogsDictionary[ItemType.LAMP].Initialize(GetListItemsByItemType(ItemType.LAMP));
        catalogsDictionary[ItemType.TABLE].Initialize(GetListItemsByItemType(ItemType.TABLE));

        //Empty
        catalogLists.Add(ItemType.FURNITURES_INVENTORY, new List<GenericCatalogItem>());
        catalogsDictionary[ItemType.FURNITURES_INVENTORY].Initialize(new List<GenericCatalogItem>());
    }

    void InitializePets(Dictionary<ItemType, GenericCatalog> catalogsDictionary)
    {
        catalogLists.Add(ItemType.PETS, GetPetsCatalogItem());

        catalogsDictionary[ItemType.PETS].Initialize(GetListItemsByItemType(ItemType.PETS));
    }

    void Initialize(Dictionary<ItemType, GenericCatalog> catalogsDictionary)
    {
        catalogLists.Add(ItemType.BODY, GetBodyCatalogItem());
        catalogLists.Add(ItemType.EYE, GetEyeCatalogItem());
        catalogLists.Add(ItemType.EAR, GetEarCatalogItem());
        catalogLists.Add(ItemType.EYEBROW, GetEyeBrowsCatalogItem());
        catalogLists.Add(ItemType.HAIR, GetHairCatalogItem());
        catalogLists.Add(ItemType.FACE, GetFaceCatalogItem());
        catalogLists.Add(ItemType.MOUTH, GetMouthCatalogItem());
        catalogLists.Add(ItemType.NOSE, GetNoseCatalogItems());

        catalogLists.Add(ItemType.UP_PART, GetUpPartCatalogItems());
        catalogLists.Add(ItemType.BOTTOM_PART, GetBootomPartCatalogItems());
        catalogLists.Add(ItemType.DRESSES, GetDressesCatalogItems());
        catalogLists.Add(ItemType.SHOES, GetShoesCatalogItems());
        catalogLists.Add(ItemType.GLASSES, GetGlassesCatalogItems());
        catalogLists.Add(ItemType.ACCESORIES, GetAccessoriesCatalogItems());


        catalogsDictionary[ItemType.BODY].Initialize(GetListItemsByItemType(ItemType.BODY));
        catalogsDictionary[ItemType.EYE].Initialize(GetListItemsByItemType(ItemType.EYE));
        catalogsDictionary[ItemType.EAR].Initialize(GetListItemsByItemType(ItemType.EAR));
        catalogsDictionary[ItemType.EYEBROW].Initialize(GetListItemsByItemType(ItemType.EYEBROW));
        catalogsDictionary[ItemType.HAIR].Initialize(GetListItemsByItemType(ItemType.HAIR));
        catalogsDictionary[ItemType.FACE].Initialize(GetListItemsByItemType(ItemType.FACE));
        catalogsDictionary[ItemType.MOUTH].Initialize(GetListItemsByItemType(ItemType.MOUTH));
        catalogsDictionary[ItemType.NOSE].Initialize(GetListItemsByItemType(ItemType.NOSE));
        catalogsDictionary[ItemType.SHOES].Initialize(GetListItemsByItemType(ItemType.SHOES));
        catalogsDictionary[ItemType.UP_PART].Initialize(GetListItemsByItemType(ItemType.UP_PART));
        catalogsDictionary[ItemType.BOTTOM_PART].Initialize(GetListItemsByItemType(ItemType.BOTTOM_PART));
        catalogsDictionary[ItemType.DRESSES].Initialize(GetListItemsByItemType(ItemType.DRESSES));
        catalogsDictionary[ItemType.GLASSES].Initialize(GetListItemsByItemType(ItemType.GLASSES));
        catalogsDictionary[ItemType.ACCESORIES].Initialize(GetListItemsByItemType(ItemType.ACCESORIES));
    }

    public void SerializeListFurnituresAndRooms()
    {
        IItemTypeUtils ItemTypeUtils = Injection.Get<IItemTypeUtils>();

        foreach (ItemType itemType in GameData.RoomItemsType)
        {
            string jsonList = SerializeItems(itemType);
            string pathJson = "Assets/Resources/Avatar/" + ItemTypeUtils.GetNameItemType(itemType) + ".json";
            WriteText(pathJson, jsonList);
        }

        string jsonListRoom = SerializeItems(ItemType.ROOM);
        string pathJsonRoom = "Assets/Resources/Avatar/" + ItemTypeUtils.GetNameItemType(ItemType.ROOM) + ".json";
        WriteText(pathJsonRoom, jsonListRoom);
    }

    public void SerializeListItems()
    {
        IItemTypeUtils ItemTypeUtils = Injection.Get<IItemTypeUtils>();

        foreach (ItemType itemType in GameData.AvatarItemsType)
        {
            string jsonList;
            if (itemType == ItemType.BODY)
            {
                jsonList = SerializeBodyItem();
            }
            else
            {
                jsonList = SerializeItems(itemType);
            }

            string pathJson = "Assets/Resources/Avatar/" + ItemTypeUtils.GetNameItemType(itemType) + ".json";
            WriteText(pathJson, jsonList);
        }
    }

    public string SerializeBodyItem()
    {
        List<GenericCatalogItem> calogItem = GetListItemsByItemType(ItemType.BODY);

        SerializableBodyCatalogItemList listSerializableCatalogItem = new SerializableBodyCatalogItemList();

        foreach (GenericCatalogItem item in calogItem)
        {
            SerializableBodyCatalogItem serializeBodyItem = new SerializableBodyCatalogItem(item as BodyCatalogItem);
            listSerializableCatalogItem.data.Add(serializeBodyItem);
        }

        string jsonList = JsonUtility.ToJson(listSerializableCatalogItem, true);
        return jsonList;
    }

    public string SerializeItems(ItemType itemType)
    {
        List<GenericCatalogItem> calogItem = GetListItemsByItemType(itemType);

        SerializableCatalogItemList listSerializableCatalogItem = new SerializableCatalogItemList();

        foreach (GenericCatalogItem item in calogItem)
        {
            SerializableCatalogItem serializeItem = new SerializableCatalogItem(item);
            listSerializableCatalogItem.data.Add(serializeItem);
        }

        string jsonList = JsonUtility.ToJson(listSerializableCatalogItem, true);
        return jsonList;
    }

    public void WriteText(string path, string text)
    {
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(text);
        writer.Close();
    }

    public List<GenericCatalogItem> GetListItemsByItemType(ItemType itemType)
    {
        if (catalogLists.ContainsKey(itemType))
        {
            return catalogLists[itemType];
        }
        return null;
    }

    private List<GenericCatalogItem> GetRoomsCatalogItem()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new GenericCatalogItem(itemType: ItemType.ROOM, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "ThemePreview_Fairyland", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 20, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.ROOM, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "ThemePreview_Hawaii", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 20, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.ROOM, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "ThemePreview_Rio", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 20, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.ROOM, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "ThemePreview_Yacht", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 20, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));

        return listItems;
    }

    private List<GenericCatalogItem> GetChairCatalogItem()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "Chair_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 20000, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "Chair_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));

        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "CouchA_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "CouchA_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "CouchA_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));

        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "CouchB_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "CouchB_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "CouchB_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));

        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "CouchSmall_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "CouchSmall_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));

        //--

        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 10, idItem: 10, nameItem: "", namePrefab: "Seat_Barton_Black", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 11, idItem: 11, nameItem: "", namePrefab: "Seat_Barton_LightBlue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 12, idItem: 12, nameItem: "", namePrefab: "Seat_Barton_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 13, idItem: 13, nameItem: "", namePrefab: "Seat_Bean_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 14, idItem: 14, nameItem: "", namePrefab: "Seat_Bean_Green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 15, idItem: 15, nameItem: "", namePrefab: "Seat_Bean_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 16, idItem: 16, nameItem: "", namePrefab: "Seat_Bella_Black", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 17, idItem: 17, nameItem: "", namePrefab: "Seat_Bella_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 18, idItem: 18, nameItem: "", namePrefab: "Seat_Bella_Yellow", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 19, idItem: 19, nameItem: "", namePrefab: "Seat_Blackstenius_White", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 20, idItem: 20, nameItem: "", namePrefab: "Seat_Decker_Brown", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 21, idItem: 21, nameItem: "", namePrefab: "Seat_Decker_Green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 22, idItem: 22, nameItem: "", namePrefab: "Seat_Decker_Purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 23, idItem: 23, nameItem: "", namePrefab: "Seat_Hilda_LightBlue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 24, idItem: 24, nameItem: "", namePrefab: "Seat_Hilda_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 25, idItem: 25, nameItem: "", namePrefab: "Seat_Hilda_Purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 26, idItem: 26, nameItem: "", namePrefab: "Seat_Holler_Brown", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 27, idItem: 27, nameItem: "", namePrefab: "Seat_Hurtig_White", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 28, idItem: 28, nameItem: "", namePrefab: "Seat_Lindahl_White", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 29, idItem: 29, nameItem: "", namePrefab: "Seat_Malm_Grey", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        // --

        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 30, idItem: 30, nameItem: "", namePrefab: "Seat_Malm_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 31, idItem: 31, nameItem: "", namePrefab: "Seat_Malm_Purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 32, idItem: 32, nameItem: "", namePrefab: "Seat_Niki_Brown", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 33, idItem: 33, nameItem: "", namePrefab: "Seat_Rattan_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 34, idItem: 34, nameItem: "", namePrefab: "Seat_Rattan_Purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 35, idItem: 35, nameItem: "", namePrefab: "Seat_Rattan_Yellow", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 36, idItem: 36, nameItem: "", namePrefab: "Seat_Rest_BlackWhite", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 37, idItem: 37, nameItem: "", namePrefab: "Seat_Rest_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 38, idItem: 38, nameItem: "", namePrefab: "Seat_Rest_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 39, idItem: 39, nameItem: "", namePrefab: "Seat_Savanna_Brown", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 40, idItem: 40, nameItem: "", namePrefab: "Seat_Savanna_Coral", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 41, idItem: 41, nameItem: "", namePrefab: "Seat_Savanna_Yellow", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 42, idItem: 42, nameItem: "", namePrefab: "Seat_Seger_White", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 43, idItem: 43, nameItem: "", namePrefab: "Seat_Woody_Brown", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 44, idItem: 44, nameItem: "", namePrefab: "Seat_Woody_Rainbow", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 45, idItem: 45, nameItem: "", namePrefab: "Seat_Woody_White", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 46, idItem: 46, nameItem: "", namePrefab: "Halloween_Seat_Count_DarkMauve", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 47, idItem: 47, nameItem: "", namePrefab: "Halloween_Seat_Monsters_Frankenstein", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 48, idItem: 48, nameItem: "", namePrefab: "Halloween_Seat_Monsters_Ghost", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 49, idItem: 49, nameItem: "", namePrefab: "Halloween_Seat_Monsters_Mummy", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 50, idItem: 50, nameItem: "", namePrefab: "Halloween_Seat_Monsters_Pumpkin", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 51, idItem: 51, nameItem: "", namePrefab: "Halloween_Seat_Monsters_Skeleton", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        //Christmas
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 52, idItem: 52, nameItem: "", namePrefab: "Christmas_Seat_Anna_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 53, idItem: 53, nameItem: "", namePrefab: "Christmas_Seat_CandyChair", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 54, idItem: 54, nameItem: "", namePrefab: "Christmas_Seat_Claus_Empty", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 55, idItem: 55, nameItem: "", namePrefab: "Christmas_Seat_Claus_Pillows", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 56, idItem: 56, nameItem: "", namePrefab: "Christmas_Seat_Clauset_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 57, idItem: 57, nameItem: "", namePrefab: "Christmas_Seat_Drum", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 58, idItem: 58, nameItem: "", namePrefab: "Christmas_Seat_Elise_SnowFlakes", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 59, idItem: 59, nameItem: "", namePrefab: "Christmas_Seat_Santas", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 60, idItem: 60, nameItem: "", namePrefab: "Christmas_Seat_Sleigh_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 61, idItem: 61, nameItem: "", namePrefab: "Christmas_Seat_Snowchair", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 62, idItem: 62, nameItem: "", namePrefab: "Christmas_Seat_XmasPattern", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        //San Valentin
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 63, idItem: 63, nameItem: "", namePrefab: "Valentine_Seat_BeanBag_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 64, idItem: 64, nameItem: "", namePrefab: "Valentine_Seat_BeanBag_Kirby", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 65, idItem: 65, nameItem: "", namePrefab: "Valentine_Seat_BeanBag_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 66, idItem: 66, nameItem: "", namePrefab: "Valentine_Seat_Chase_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.CHAIR, idItemWebClient: 67, idItem: 67, nameItem: "", namePrefab: "Valentine_Seat_Darling_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        return listItems;
    }

    private List<GenericCatalogItem> GetPetsCatalogItem()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new GenericCatalogItem(itemType: ItemType.PETS, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "Cat", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 10, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.PETS, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "Dog", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 10, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        //listItems.Add(new GenericCatalogItem(itemType: ItemType.PETS, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "Horse", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 10, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        //listItems.Add(new GenericCatalogItem(itemType: ItemType.PETS, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "Snail", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 10, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        //listItems.Add(new GenericCatalogItem(itemType: ItemType.PETS, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "Rabbit", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 10, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        return listItems;
    }

    private List<GenericCatalogItem> GetLampCatalogItem()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "Lamp_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 20, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "Lamp_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "Lamp_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 40, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "Plant_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "Plant_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 60, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "Plant_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 70, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "Light_Bob_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "Light_Bob_Green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "Light_Bob_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "Light_Donna_Purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 10, idItem: 10, nameItem: "", namePrefab: "Light_Donna_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 11, idItem: 11, nameItem: "", namePrefab: "Light_Donna_White", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 12, idItem: 12, nameItem: "", namePrefab: "Light_Sam_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 13, idItem: 13, nameItem: "", namePrefab: "Light_Tonvis_Copper", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 14, idItem: 14, nameItem: "", namePrefab: "Light_Tonvis_Gold", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 15, idItem: 15, nameItem: "", namePrefab: "Light_Tonvis_Silver", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 16, idItem: 16, nameItem: "", namePrefab: "Misc_Aqua_Green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 17, idItem: 17, nameItem: "", namePrefab: "Misc_Aqua_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 18, idItem: 18, nameItem: "", namePrefab: "Misc_Aqua_Rusted", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 19, idItem: 19, nameItem: "", namePrefab: "Misc_Aqua_Yellow", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 20, idItem: 20, nameItem: "", namePrefab: "Misc_BBQ_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 21, idItem: 21, nameItem: "", namePrefab: "Misc_BBQ_Grey", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 22, idItem: 22, nameItem: "", namePrefab: "Misc_BBQ_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 23, idItem: 23, nameItem: "", namePrefab: "Misc_BeachBall_Pastel", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 24, idItem: 24, nameItem: "", namePrefab: "Misc_BeachBall_PrimaryCol", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 25, idItem: 25, nameItem: "", namePrefab: "Misc_BeachBall_Purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 26, idItem: 26, nameItem: "", namePrefab: "Misc_Beat_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 27, idItem: 27, nameItem: "", namePrefab: "Misc_Beat_Green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 28, idItem: 28, nameItem: "", namePrefab: "Misc_Beat_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 29, idItem: 29, nameItem: "", namePrefab: "Misc_Easel_Car", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 30, idItem: 30, nameItem: "", namePrefab: "Misc_Easel_Modern", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 31, idItem: 31, nameItem: "", namePrefab: "Misc_Grandpa_Black", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 32, idItem: 32, nameItem: "", namePrefab: "Misc_Grandpa_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 33, idItem: 33, nameItem: "", namePrefab: "Misc_Grandpa_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 34, idItem: 34, nameItem: "", namePrefab: "Plant_Cacti_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 35, idItem: 35, nameItem: "", namePrefab: "Plant_Cacti_Brown", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 36, idItem: 36, nameItem: "", namePrefab: "Plant_Cacti_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 37, idItem: 37, nameItem: "", namePrefab: "Plant_Snake_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 38, idItem: 38, nameItem: "", namePrefab: "Plant_Snake_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 39, idItem: 39, nameItem: "", namePrefab: "Plant_Snake_Yellow", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 40, idItem: 40, nameItem: "", namePrefab: "Halloween_Light_Jack_Off", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 41, idItem: 41, nameItem: "", namePrefab: "Halloween_Light_Jack_On", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 42, idItem: 42, nameItem: "", namePrefab: "Halloween_Light_Wax_Bats", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 43, idItem: 43, nameItem: "", namePrefab: "Halloween_Misc_CandyBucket_Pumpkin", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 44, idItem: 44, nameItem: "", namePrefab: "Halloween_Misc_Cauldron_Green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 45, idItem: 45, nameItem: "", namePrefab: "Halloween_Misc_Pumpkin_Standard", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 46, idItem: 46, nameItem: "", namePrefab: "Halloween_Misc_Pumpkin_Group", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 47, idItem: 47, nameItem: "", namePrefab: "Halloween_Misc_SugarSkull_Purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 48, idItem: 48, nameItem: "", namePrefab: "Halloween_Misc_Wooden_DarkBrown", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 49, idItem: 49, nameItem: "", namePrefab: "Halloween_Plant_Marigold_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 50, idItem: 50, nameItem: "", namePrefab: "Halloween_ScaryBalloon_Ghost", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 51, idItem: 51, nameItem: "", namePrefab: "Halloween_ScaryBalloon_Pumpkin", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 52, idItem: 52, nameItem: "", namePrefab: "Halloween_ScaryBalloon_Spider", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        //Christmas
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 53, idItem: 53, nameItem: "", namePrefab: "Christmas_Light_Stars_Gold", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 54, idItem: 54, nameItem: "", namePrefab: "Christmas_Misc_ChristmasTree", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 55, idItem: 55, nameItem: "", namePrefab: "Christmas_Misc_Gifttower", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 56, idItem: 56, nameItem: "", namePrefab: "Christmas_Misc_PresentPile_Green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 57, idItem: 57, nameItem: "", namePrefab: "Christmas_Misc_PresentPile_Stripes", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 58, idItem: 58, nameItem: "", namePrefab: "Christmas_Misc_SmallXTree", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 59, idItem: 59, nameItem: "", namePrefab: "Christmas_Misc_Snowman_Purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 60, idItem: 60, nameItem: "", namePrefab: "Christmas_Misc_Snowman_Tophat", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 61, idItem: 61, nameItem: "", namePrefab: "Christmas_Plant_Poinsettia_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 62, idItem: 62, nameItem: "", namePrefab: "Misc_chimney", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 63, idItem: 63, nameItem: "", namePrefab: "Misc_Gnome", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 64, idItem: 64, nameItem: "", namePrefab: "Misc_Lantern", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 65, idItem: 65, nameItem: "", namePrefab: "Misc_Nutcracker", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 66, idItem: 66, nameItem: "", namePrefab: "Misc_PenguineDoll", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 67, idItem: 67, nameItem: "", namePrefab: "Misc_Ram", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 68, idItem: 68, nameItem: "", namePrefab: "Misc_SantaDoll", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        //San Valentin
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 69, idItem: 69, nameItem: "", namePrefab: "Valentine_Light_Dreamer_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 70, idItem: 70, nameItem: "", namePrefab: "Valentine_Misc_Chocolates", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 71, idItem: 71, nameItem: "", namePrefab: "Valentine_Misc_HeartBalloon_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 72, idItem: 72, nameItem: "", namePrefab: "Valentine_Misc_Rabbit", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 73, idItem: 73, nameItem: "", namePrefab: "Valentine_Misc_Teddy_Brown", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 74, idItem: 74, nameItem: "", namePrefab: "Valentine_Plant_Roses", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        //Easter
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 75, idItem: 75, nameItem: "", namePrefab: "Easter_Misc_Chick_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 76, idItem: 76, nameItem: "", namePrefab: "Easter_Misc_Korg_Purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 77, idItem: 77, nameItem: "", namePrefab: "Easter_Misc_Shelly_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 78, idItem: 78, nameItem: "", namePrefab: "Easter_Misc_Sticks_Yellow", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.LAMP, idItemWebClient: 79, idItem: 79, nameItem: "", namePrefab: "Easter_Seat_Eggward_Purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        return listItems;
    }

    private List<GenericCatalogItem> GetTablesCatalogItem()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "Table_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 20, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "Table_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "TableSquare_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "TableRectangle_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "Misc_Patricia_LightGrey", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "Misc_Patrik_LightGrey", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "Misc_Philip_DarkGrey", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "Misc_Philip_Grey", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "Misc_Philip_LightGrey", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "Misc_Philippa_DarkGrey", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 10, idItem: 10, nameItem: "", namePrefab: "Misc_Philippa_Grey", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 11, idItem: 11, nameItem: "", namePrefab: "Misc_Philippa_LightGrey", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 12, idItem: 12, nameItem: "", namePrefab: "Storage_Brook_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 13, idItem: 13, nameItem: "", namePrefab: "Storage_Brook_Brown", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 14, idItem: 14, nameItem: "", namePrefab: "Storage_Brook_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 15, idItem: 15, nameItem: "", namePrefab: "Storage_Brooklyn_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 16, idItem: 16, nameItem: "", namePrefab: "Halloween_Table_Countess_Decorations", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 17, idItem: 17, nameItem: "", namePrefab: "Halloween_Table_Countess_Tattered", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 18, idItem: 18, nameItem: "", namePrefab: "Halloween_Table_Countess_Webs", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        //Christmas
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 19, idItem: 19, nameItem: "", namePrefab: "Christmas_Table_GingerBreadHouse", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 20, idItem: 20, nameItem: "", namePrefab: "Christmas_Table_Julbord_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 21, idItem: 21, nameItem: "", namePrefab: "Table_CookiesAndMilk", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        //San Valentin
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 22, idItem: 22, nameItem: "", namePrefab: "Valentine_Table_Grace_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 23, idItem: 23, nameItem: "", namePrefab: "Valentine_Table_Letter", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        //Easter
        listItems.Add(new GenericCatalogItem(itemType: ItemType.TABLE, idItemWebClient: 24, idItem: 24, nameItem: "", namePrefab: "Easter_Table_Benedict_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        return listItems;
    }

    private List<GenericCatalogItem> GetFloorCatalogItem()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new GenericCatalogItem(itemType: ItemType.FLOOR, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "CarpetSquare_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 20, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.FLOOR, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "CarpetRectangle_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.FLOOR, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "CarpetCircle_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.FLOOR, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "Halloween_Floor_Spiderweb_Black", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        listItems.Add(new GenericCatalogItem(itemType: ItemType.FLOOR, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "Halloween_Floor_Vinden_WineRed", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        //San Valentin
        listItems.Add(new GenericCatalogItem(itemType: ItemType.FLOOR, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "Valentine_Floor_Evelyn_Pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));
        //Easter
        listItems.Add(new GenericCatalogItem(itemType: ItemType.FLOOR, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "Easter_Floor_EggCarpet_Green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 30, goldPrice: 0, currencyType: CurrencyType.GEM_PRICE));

        return listItems;
    }

    private List<GenericCatalogItem> GetBodyCatalogItem()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new BodyCatalogItem(itemType: ItemType.BODY, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "body_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null, useBoobs: false));
        listItems.Add(new BodyCatalogItem(itemType: ItemType.BODY, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "body_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null, useBoobs: true));

        for (int i = 10; i < 100; i++)
        {
            //listItems.Add(new GenericCatalogItem(ItemType.BODY, i, "", "Body-_0009_E2", 0, true, 0, 0, CurrencyType.GOLD_PRICE));
        }

        return listItems;
    }

    private List<GenericCatalogItem> GetEyeCatalogItem()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYE, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "eyes_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYE, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "eyes_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYE, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "eyes_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYE, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "eyes_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYE, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "eyes_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYE, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "eyes_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYE, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "eyes_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYE, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "eyes_8", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYE, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "eyes_9", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYE, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "eyes_10", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        return listItems;
    }

    private List<GenericCatalogItem> GetEarCatalogItem()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EAR, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "ear_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EAR, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "ear_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EAR, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "ear_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EAR, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "ear_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EAR, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "ear_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EAR, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "ear_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EAR, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "ear_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        return listItems;
    }

    private List<GenericCatalogItem> GetEyeBrowsCatalogItem()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYEBROW, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "eyebrows_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYEBROW, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "eyebrows_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYEBROW, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "eyebrows_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYEBROW, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "eyebrows_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYEBROW, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "eyebrows_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYEBROW, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "eyebrows_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYEBROW, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "eyebrows_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYEBROW, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "eyebrows_8", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYEBROW, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "eyebrows_9", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.EYEBROW, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "eyebrows_10", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));


        return listItems;
    }

    private List<GenericCatalogItem> GetHairCatalogItem()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "aya", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "robyn", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "marti", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "alexa", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "titus", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "rasta", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "sebastian", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "kevin", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "karu", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "tao", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 10, idItem: 10, nameItem: "", namePrefab: "audrey", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 11, idItem: 11, nameItem: "", namePrefab: "seifer", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 12, idItem: 12, nameItem: "", namePrefab: "uub", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 13, idItem: 13, nameItem: "", namePrefab: "jaden", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 14, idItem: 14, nameItem: "", namePrefab: "willow", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 15, idItem: 15, nameItem: "", namePrefab: "daniel", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 16, idItem: 16, nameItem: "", namePrefab: "lita", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 17, idItem: 17, nameItem: "", namePrefab: "alex", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 18, idItem: 18, nameItem: "", namePrefab: "ari", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 19, idItem: 19, nameItem: "", namePrefab: "zell", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 20, idItem: 20, nameItem: "", namePrefab: "elsa", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 21, idItem: 21, nameItem: "", namePrefab: "trunks", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 22, idItem: 22, nameItem: "", namePrefab: "jill", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 23, idItem: 23, nameItem: "", namePrefab: "granpa", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.HAIR, idItemWebClient: 24, idItem: 24, nameItem: "", namePrefab: "bald", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 2 }));

        return listItems;
    }

    private List<GenericCatalogItem> GetFaceCatalogItem()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.FACE, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "faceshape_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.FACE, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "faceshape_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.FACE, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "faceshape_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.FACE, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "faceshape_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.FACE, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "faceshape_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.FACE, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "faceshape_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.FACE, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "faceshape_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.FACE, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "faceshape_8", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        return listItems;
    }

    private List<GenericCatalogItem> GetMouthCatalogItem()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.MOUTH, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "mouth_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.MOUTH, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "mouth_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.MOUTH, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "mouth_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.MOUTH, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "mouth_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.MOUTH, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "mouth_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.MOUTH, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "Mouth_Fangs", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.MOUTH, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "mouth_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.MOUTH, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "mouth_8", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.MOUTH, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "mouth_9", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.MOUTH, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "mouth_10", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 33, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        return listItems;
    }

    private List<GenericCatalogItem> GetNoseCatalogItems()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.NOSE, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "nose_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.NOSE, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "nose_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.NOSE, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "nose_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.NOSE, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "nose_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.NOSE, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "nose_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.NOSE, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "nose_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 10, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.NOSE, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "nose_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 10, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.NOSE, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "nose_8", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 10, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.NOSE, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "nose_9", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 10, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.NOSE, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "nose_10", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 10, currencyType: CurrencyType.GOLD_PRICE, layers: null));


        return listItems;
    }

    private List<GenericCatalogItem> GetColorCatalogItems()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "321f1a"));

        //Eye
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "eyes1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "321f1a"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "eyes2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "793725"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "eyes3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "a1661d"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "eyes4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "2c88bd"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "eyes5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "85eaf6"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "eyes6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "4f6d2e"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "eyes7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "53645a"));
        //Hair
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "hair1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "742d1e"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "hair2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "1c1b21"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 10, idItem: 10, nameItem: "", namePrefab: "hair3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "9db1bf"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 11, idItem: 11, nameItem: "", namePrefab: "hair4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "b5822f"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 12, idItem: 12, nameItem: "", namePrefab: "hair5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "8b1113"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 13, idItem: 13, nameItem: "", namePrefab: "hair6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "2d201e"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 14, idItem: 14, nameItem: "", namePrefab: "hair7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "ebcc6e"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 15, idItem: 15, nameItem: "", namePrefab: "hair8", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "66423a"));
        //Lip
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 16, idItem: 16, nameItem: "", namePrefab: "lips1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "b92e4a"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 17, idItem: 17, nameItem: "", namePrefab: "lips2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "b72c7c"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 18, idItem: 18, nameItem: "", namePrefab: "lips3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "c56e4b"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 19, idItem: 19, nameItem: "", namePrefab: "lips4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "d1788b"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 20, idItem: 20, nameItem: "", namePrefab: "lips5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "630e1f"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 21, idItem: 21, nameItem: "", namePrefab: "lips6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "cc3e28"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 22, idItem: 22, nameItem: "", namePrefab: "lips7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "cf0714"));
        //Skin

        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 23, idItem: 23, nameItem: "", namePrefab: "skin1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "dcac86"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 24, idItem: 24, nameItem: "", namePrefab: "skin2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "edc3a5"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 25, idItem: 25, nameItem: "", namePrefab: "skin3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "f6d8c2"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 26, idItem: 26, nameItem: "", namePrefab: "skin4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "f2c386"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 27, idItem: 27, nameItem: "", namePrefab: "skin5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "f9d7a0"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 28, idItem: 28, nameItem: "", namePrefab: "skin6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "77371e"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 29, idItem: 29, nameItem: "", namePrefab: "skin7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "4a2127"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 30, idItem: 30, nameItem: "", namePrefab: "skin8", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "ffa78e"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 31, idItem: 31, nameItem: "", namePrefab: "skin9", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "d97c4f"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 32, idItem: 32, nameItem: "", namePrefab: "skin10", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "c67365"));

        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 33, idItem: 33, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "df2619"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 34, idItem: 34, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "f5ec30"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 35, idItem: 35, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "a22283"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 36, idItem: 36, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "ff3977"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 37, idItem: 37, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "3ed8d1"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 38, idItem: 38, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "ff8926"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 39, idItem: 39, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "33d983"));

        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 40, idItem: 40, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "4ea621"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 41, idItem: 41, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "5d0969"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 42, idItem: 42, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "ad1a20"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 43, idItem: 43, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "b5b38d"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 44, idItem: 44, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "e14a1f"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 45, idItem: 45, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "008c9a"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 46, idItem: 46, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "18204f"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 47, idItem: 47, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "f27f8f"));

        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 48, idItem: 48, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "d043dc"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 49, idItem: 49, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "4469dc"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 50, idItem: 50, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "3b303b"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 51, idItem: 51, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "33e195"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 52, idItem: 52, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "00dafd"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 53, idItem: 53, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "670071"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 54, idItem: 54, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "af3112"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 55, idItem: 55, nameItem: "", namePrefab: "", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "000000"));
        listItems.Add(new ColorCatalogItem(itemType: ItemType.COLOR, idItemWebClient: 56, idItem: 56, nameItem: "", namePrefab: "hair9", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, color: "1A627E"));
        return listItems;
    }

    private List<GenericCatalogItem> GetShoesCatalogItems()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "fancyshoes_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "fancyshoes_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "fancyshoes_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "fancyshoes_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "fancyshoes_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "fancyshoes_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "fancyshoes_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "sleepers_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "sleepers_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "sleepers_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 10, idItem: 10, nameItem: "", namePrefab: "sleepers_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 11, idItem: 11, nameItem: "", namePrefab: "sleepers_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 12, idItem: 12, nameItem: "", namePrefab: "sleepers_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 13, idItem: 13, nameItem: "", namePrefab: "sleepers_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 14, idItem: 14, nameItem: "", namePrefab: "highheels_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 15, idItem: 15, nameItem: "", namePrefab: "highheels_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 16, idItem: 16, nameItem: "", namePrefab: "highheels_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 17, idItem: 17, nameItem: "", namePrefab: "highheels_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 18, idItem: 18, nameItem: "", namePrefab: "highheels_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 19, idItem: 19, nameItem: "", namePrefab: "highheels_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 20, idItem: 20, nameItem: "", namePrefab: "highheels_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 21, idItem: 21, nameItem: "", namePrefab: "loafers_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 22, idItem: 22, nameItem: "", namePrefab: "loafers_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 23, idItem: 23, nameItem: "", namePrefab: "loafers_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 24, idItem: 24, nameItem: "", namePrefab: "loafers_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 25, idItem: 25, nameItem: "", namePrefab: "loafers_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 26, idItem: 26, nameItem: "", namePrefab: "loafers_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 27, idItem: 27, nameItem: "", namePrefab: "loafers_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 28, idItem: 28, nameItem: "", namePrefab: "sneakers_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 1000, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 29, idItem: 29, nameItem: "", namePrefab: "sneakers_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 1000, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 30, idItem: 30, nameItem: "", namePrefab: "sneakers_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 1000, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 31, idItem: 31, nameItem: "", namePrefab: "sneakers_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 1000, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 32, idItem: 32, nameItem: "", namePrefab: "sneakers_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 1000, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 33, idItem: 33, nameItem: "", namePrefab: "sneakers_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 1000, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 34, idItem: 34, nameItem: "", namePrefab: "sneakers_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 1000, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 35, idItem: 35, nameItem: "", namePrefab: "elfshoes", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 36, idItem: 36, nameItem: "", namePrefab: "santaboots", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 37, idItem: 37, nameItem: "", namePrefab: "santaheels", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 38, idItem: 38, nameItem: "", namePrefab: "Shoes_Mummy", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null, psbName: "Halloween2022",  psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 39, idItem: 39, nameItem: "", namePrefab: "Shoes_Witch", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));

        //Easter
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.SHOES, idItemWebClient: 40, idItem: 40, nameItem: "", namePrefab: "Shoes_bunnyfeet", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: null, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));

        
        return listItems;
    }

    private List<GenericCatalogItem> GetUpPartCatalogItems()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "tanktop_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 75, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "tanktop_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 75, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "tanktop_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 75, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "tanktop_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 75, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "tanktop_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 75, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "tanktop_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 75, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "tanktop_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 75, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "shortsleeveshirt_black", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "shortsleeveshirt_olive", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "shortsleeveshirt_cream", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 10, idItem: 10, nameItem: "", namePrefab: "shortsleeveshirt_blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 11, idItem: 11, nameItem: "", namePrefab: "shortsleeveshirt_cyan", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 12, idItem: 12, nameItem: "", namePrefab: "shortsleeveshirt_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 13, idItem: 13, nameItem: "", namePrefab: "shortsleeveshirt_white", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 14, idItem: 14, nameItem: "", namePrefab: "polo_pinkcream", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 250, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 15, idItem: 15, nameItem: "", namePrefab: "polo_blackwhite", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 250, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 16, idItem: 16, nameItem: "", namePrefab: "polo_olivebrown", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 250, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 17, idItem: 17, nameItem: "", namePrefab: "polo_bluegreen", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 250, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 18, idItem: 18, nameItem: "", namePrefab: "polo_cyanblue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 250, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 19, idItem: 19, nameItem: "", namePrefab: "polo_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 250, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 20, idItem: 20, nameItem: "", namePrefab: "polo_whitered", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 250, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 21, idItem: 21, nameItem: "", namePrefab: "polo_whiteblue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 250, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 22, idItem: 22, nameItem: "", namePrefab: "longsleeveshirt_white", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 300, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 23, idItem: 23, nameItem: "", namePrefab: "longsleeveshirt_cream", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 300, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 24, idItem: 24, nameItem: "", namePrefab: "longsleeveshirt_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 300, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 25, idItem: 25, nameItem: "", namePrefab: "longsleeveshirt_olive", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 300, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 26, idItem: 26, nameItem: "", namePrefab: "longsleeveshirt_pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 300, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 27, idItem: 27, nameItem: "", namePrefab: "longsleeveshirt_purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 300, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 28, idItem: 28, nameItem: "", namePrefab: "longsleeveshirt_black", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 300, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 29, idItem: 29, nameItem: "", namePrefab: "bowtieshirt_bluered", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 30, idItem: 30, nameItem: "", namePrefab: "bowtieshirt_creamblue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 31, idItem: 31, nameItem: "", namePrefab: "bowtieshirt_cyanred", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 32, idItem: 32, nameItem: "", namePrefab: "bowtieshirt_blackpurple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 33, idItem: 33, nameItem: "", namePrefab: "bowtieshirt_blackred", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 34, idItem: 34, nameItem: "", namePrefab: "bowtieshirt_whiteblack", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 35, idItem: 35, nameItem: "", namePrefab: "bowtieshirt_whitered", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 36, idItem: 36, nameItem: "", namePrefab: "tshirt_bluered", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 150, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 37, idItem: 37, nameItem: "", namePrefab: "tshirt_gray", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 150, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 38, idItem: 38, nameItem: "", namePrefab: "tshirt_white", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 150, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 39, idItem: 39, nameItem: "", namePrefab: "tshirt_whiteblue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 150, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 40, idItem: 40, nameItem: "", namePrefab: "tshirt_yelloworange", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 150, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 41, idItem: 41, nameItem: "", namePrefab: "tshirt_pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 150, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 42, idItem: 42, nameItem: "", namePrefab: "tshirt_greenblue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 150, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 43, idItem: 43, nameItem: "", namePrefab: "tshirt_friendbase", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 1000, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 44, idItem: 44, nameItem: "", namePrefab: "snowman_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 45, idItem: 45, nameItem: "", namePrefab: "snowman_green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 46, idItem: 46, nameItem: "", namePrefab: "snowman_blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 47, idItem: 47, nameItem: "", namePrefab: "santasuit_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 48, idItem: 48, nameItem: "", namePrefab: "santasuit_green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 49, idItem: 49, nameItem: "", namePrefab: "santasuit_blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 50, idItem: 50, nameItem: "", namePrefab: "elfsuit", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 51, idItem: 51, nameItem: "", namePrefab: "uglysweater_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 52, idItem: 52, nameItem: "", namePrefab: "uglysweater_green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 53, idItem: 53, nameItem: "", namePrefab: "uglysweater_blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 54, idItem: 54, nameItem: "", namePrefab: "Top_Mummy", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        //San Valentine
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 55, idItem: 55, nameItem: "", namePrefab: "valentinestshirt", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 56, idItem: 56, nameItem: "", namePrefab: "valentinestanktop", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        //Easter
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 57, idItem: 57, nameItem: "", namePrefab: "tshirt_eggshirt", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 58, idItem: 58, nameItem: "", namePrefab: "tshirt_redbunny", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 59, idItem: 59, nameItem: "", namePrefab: "tshirt_yellowchick", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.UP_PART, idItemWebClient: 60, idItem: 60, nameItem: "", namePrefab: "eggshirt_green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));


        return listItems;
    }

    private List<GenericCatalogItem> GetBootomPartCatalogItems()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "pants_white", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "pants_blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "pants_retro", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "pants_earth", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "pants_olive", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "pants_brown", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "pants_black", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "tennisshirt_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 3 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "tennisshirt_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 3 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "tennisshirt_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 3 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 10, idItem: 10, nameItem: "", namePrefab: "tennisshirt_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 3 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 11, idItem: 11, nameItem: "", namePrefab: "tennisshirt_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 3 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 12, idItem: 12, nameItem: "", namePrefab: "tennisshirt_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 3 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 13, idItem: 13, nameItem: "", namePrefab: "tennisshirt_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 3 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 14, idItem: 14, nameItem: "", namePrefab: "shorts_black", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 15, idItem: 15, nameItem: "", namePrefab: "shorts_brown", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 16, idItem: 16, nameItem: "", namePrefab: "shorts_olive", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 17, idItem: 17, nameItem: "", namePrefab: "shorts_earth", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 18, idItem: 18, nameItem: "", namePrefab: "shorts_pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 19, idItem: 19, nameItem: "", namePrefab: "shorts_jeans", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 20, idItem: 20, nameItem: "", namePrefab: "shorts_white", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 21, idItem: 21, nameItem: "", namePrefab: "elfpants", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 22, idItem: 22, nameItem: "", namePrefab: "santapants_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 23, idItem: 23, nameItem: "", namePrefab: "santapants_green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 24, idItem: 24, nameItem: "", namePrefab: "santapants_blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 25, idItem: 25, nameItem: "", namePrefab: "Pants_Mummy", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));

        //Easter
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.BOTTOM_PART, idItemWebClient: 26, idItem: 26, nameItem: "", namePrefab: "eggpants_green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1 }, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));

        
        return listItems;
    }

    private List<GenericCatalogItem> GetDressesCatalogItems()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "shortdress_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "shortdress_purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "shortdress_pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "shortdress_cyan", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "shortdress_gold", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "shortdress_wine", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "shortdress_black", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "mediumdress_pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "mediumdress_black", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "mediumdress_yellow", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 10, idItem: 10, nameItem: "", namePrefab: "mediumdress_purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 11, idItem: 11, nameItem: "", namePrefab: "mediumdress_cyan", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 12, idItem: 12, nameItem: "", namePrefab: "mediumdress_brown", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 13, idItem: 13, nameItem: "", namePrefab: "mediumdress_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 14, idItem: 14, nameItem: "", namePrefab: "longdress_white", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 15, idItem: 15, nameItem: "", namePrefab: "longdress_black", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 16, idItem: 16, nameItem: "", namePrefab: "longdress_purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 17, idItem: 17, nameItem: "", namePrefab: "longdress_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 18, idItem: 18, nameItem: "", namePrefab: "longdress_cream", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 19, idItem: 19, nameItem: "", namePrefab: "longdress_lime", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 20, idItem: 20, nameItem: "", namePrefab: "longdress_pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 21, idItem: 21, nameItem: "", namePrefab: "xmas_tree", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 22, idItem: 22, nameItem: "", namePrefab: "santadress_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 23, idItem: 26, nameItem: "", namePrefab: "santadress_green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));
        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 24, idItem: 27, nameItem: "", namePrefab: "santadress_blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }));

        listItems.Add(new AvatarGenericCatalogItem(itemType: ItemType.DRESSES, idItemWebClient: 25, idItem: 28, nameItem: "", namePrefab: "Dress_WItch", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 1, 2 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));

        return listItems;
    }

    private List<GenericCatalogItem> GetGlassesCatalogItems()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "wonka_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "wonka_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "wonka_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "wonka_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "wonka_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "wonka_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "wonka_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "lenon_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "lenon_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "lenon_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 10, idItem: 10, nameItem: "", namePrefab: "lenon_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 11, idItem: 11, nameItem: "", namePrefab: "lenon_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 12, idItem: 12, nameItem: "", namePrefab: "lenon_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 13, idItem: 13, nameItem: "", namePrefab: "lenon_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 14, idItem: 14, nameItem: "", namePrefab: "future_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 15, idItem: 15, nameItem: "", namePrefab: "future_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 16, idItem: 16, nameItem: "", namePrefab: "future_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 17, idItem: 17, nameItem: "", namePrefab: "future_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 18, idItem: 18, nameItem: "", namePrefab: "future_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 19, idItem: 19, nameItem: "", namePrefab: "future_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 20, idItem: 20, nameItem: "", namePrefab: "future_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 21, idItem: 21, nameItem: "", namePrefab: "Raybansun_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 22, idItem: 22, nameItem: "", namePrefab: "Raybansun_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 23, idItem: 23, nameItem: "", namePrefab: "Raybansun_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 24, idItem: 24, nameItem: "", namePrefab: "Raybansun_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 25, idItem: 25, nameItem: "", namePrefab: "Raybansun_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 26, idItem: 26, nameItem: "", namePrefab: "Raybansun_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 27, idItem: 27, nameItem: "", namePrefab: "Raybansun_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 28, idItem: 28, nameItem: "", namePrefab: "smallglasses_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 29, idItem: 29, nameItem: "", namePrefab: "smallglasses_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 30, idItem: 30, nameItem: "", namePrefab: "smallglasses_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 31, idItem: 31, nameItem: "", namePrefab: "smallglasses_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 32, idItem: 32, nameItem: "", namePrefab: "smallglasses_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 33, idItem: 33, nameItem: "", namePrefab: "smallglasses_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 34, idItem: 34, nameItem: "", namePrefab: "smallglasses_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 35, idItem: 35, nameItem: "", namePrefab: "rayban_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 36, idItem: 36, nameItem: "", namePrefab: "rayban_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 37, idItem: 37, nameItem: "", namePrefab: "rayban_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 38, idItem: 38, nameItem: "", namePrefab: "rayban_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 39, idItem: 39, nameItem: "", namePrefab: "rayban_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 40, idItem: 40, nameItem: "", namePrefab: "rayban_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 41, idItem: 41, nameItem: "", namePrefab: "rayban_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 42, idItem: 42, nameItem: "", namePrefab: "deb_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 43, idItem: 43, nameItem: "", namePrefab: "deb_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 44, idItem: 44, nameItem: "", namePrefab: "deb_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 45, idItem: 45, nameItem: "", namePrefab: "deb_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 46, idItem: 46, nameItem: "", namePrefab: "deb_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 47, idItem: 47, nameItem: "", namePrefab: "deb_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 48, idItem: 48, nameItem: "", namePrefab: "deb_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 49, idItem: 49, nameItem: "", namePrefab: "potter_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 50, idItem: 50, nameItem: "", namePrefab: "potter_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 51, idItem: 51, nameItem: "", namePrefab: "potter_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 52, idItem: 52, nameItem: "", namePrefab: "potter_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 53, idItem: 53, nameItem: "", namePrefab: "potter_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 54, idItem: 54, nameItem: "", namePrefab: "potter_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 55, idItem: 55, nameItem: "", namePrefab: "potter_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 56, idItem: 56, nameItem: "", namePrefab: "retro_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 57, idItem: 57, nameItem: "", namePrefab: "retro_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 58, idItem: 58, nameItem: "", namePrefab: "retro_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 59, idItem: 59, nameItem: "", namePrefab: "retro_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 60, idItem: 60, nameItem: "", namePrefab: "retro_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 61, idItem: 61, nameItem: "", namePrefab: "retro_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 62, idItem: 62, nameItem: "", namePrefab: "retro_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 63, idItem: 63, nameItem: "", namePrefab: "mistletoe", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 50, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 64, idItem: 64, nameItem: "", namePrefab: "snowman_hat", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 65, idItem: 65, nameItem: "", namePrefab: "rudolph_nose", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 100, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 66, idItem: 66, nameItem: "", namePrefab: "santa_beard", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 67, idItem: 67, nameItem: "", namePrefab: "santa_hat_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 68, idItem: 68, nameItem: "", namePrefab: "santa_hat_green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 69, idItem: 69, nameItem: "", namePrefab: "santa_hat_blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 70, idItem: 70, nameItem: "", namePrefab: "santabeard_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 0, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 71, idItem: 71, nameItem: "", namePrefab: "santabeard_green", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 72, idItem: 72, nameItem: "", namePrefab: "santabeard_blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 73, idItem: 73, nameItem: "", namePrefab: "elfhat", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 74, idItem: 74, nameItem: "", namePrefab: "Glasses_Mummy", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 75, idItem: 75, nameItem: "", namePrefab: "Glasses_PumpkinMask1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 76, idItem: 76, nameItem: "", namePrefab: "Glasses_PumpkinMask2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 77, idItem: 77, nameItem: "", namePrefab: "Glasses_PumpkinMask3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 78, idItem: 78, nameItem: "", namePrefab: "Glasses_ZombieMask1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 79, idItem: 79, nameItem: "", namePrefab: "Glasses_ZombieMask2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 80, idItem: 80, nameItem: "", namePrefab: "Glasses_ZombieMask3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 81, idItem: 81, nameItem: "", namePrefab: "Glasses_CatrinaMask1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 82, idItem: 82, nameItem: "", namePrefab: "Glasses_CatrinaMask2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 83, idItem: 83, nameItem: "", namePrefab: "Glasses_CatrinaMask3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 84, idItem: 84, nameItem: "", namePrefab: "Glasses_WItchHatNose", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 85, idItem: 85, nameItem: "", namePrefab: "Glasses_WItchHat", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 86, idItem: 86, nameItem: "", namePrefab: "newyearpartycone", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 87, idItem: 87, nameItem: "", namePrefab: "newyearpartyhat", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 88, idItem: 88, nameItem: "", namePrefab: "newyearhat", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 89, idItem: 89, nameItem: "", namePrefab: "newyearsunglasses", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 90, idItem: 90, nameItem: "", namePrefab: "newyearglasses", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        //San Valentin
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 91, idItem: 91, nameItem: "", namePrefab: "HeartGlassesPink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 92, idItem: 92, nameItem: "", namePrefab: "HeartGlasses", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        //Easter
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 93, idItem: 93, nameItem: "", namePrefab: "Hat_BunnyHeadpiece", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 94, idItem: 94, nameItem: "", namePrefab: "Hat_eggpiece", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 95, idItem: 95, nameItem: "", namePrefab: "Glasses_bunnymouth", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.GLASSES, idItemWebClient: 96, idItem: 96, nameItem: "", namePrefab: "Hat_Bunny_ears", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));


        return listItems;
    }

    private List<GenericCatalogItem> GetAccessoriesCatalogItems()
    {
        List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 0, idItem: 0, nameItem: "", namePrefab: "fannypack2_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 1, idItem: 1, nameItem: "", namePrefab: "fannypack2_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 2, idItem: 2, nameItem: "", namePrefab: "fannypack2_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 3, idItem: 3, nameItem: "", namePrefab: "fannypack2_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 4, idItem: 4, nameItem: "", namePrefab: "fannypack2_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 5, idItem: 5, nameItem: "", namePrefab: "fannypack2_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 800, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 6, idItem: 6, nameItem: "", namePrefab: "fannypack2_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 1000, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 7, idItem: 7, nameItem: "", namePrefab: "business_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 8, idItem: 8, nameItem: "", namePrefab: "business_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 9, idItem: 9, nameItem: "", namePrefab: "business_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 10, idItem: 10, nameItem: "", namePrefab: "business_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 11, idItem: 11, nameItem: "", namePrefab: "business_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 12, idItem: 12, nameItem: "", namePrefab: "business_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 13, idItem: 13, nameItem: "", namePrefab: "business_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 14, idItem: 14, nameItem: "", namePrefab: "purse_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 15, idItem: 15, nameItem: "", namePrefab: "purse_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 16, idItem: 16, nameItem: "", namePrefab: "purse_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 17, idItem: 17, nameItem: "", namePrefab: "purse_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 18, idItem: 18, nameItem: "", namePrefab: "purse_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 19, idItem: 19, nameItem: "", namePrefab: "purse_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 20, idItem: 20, nameItem: "", namePrefab: "purse_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 400, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 21, idItem: 21, nameItem: "", namePrefab: "fannypack_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 22, idItem: 22, nameItem: "", namePrefab: "fannypack_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 23, idItem: 23, nameItem: "", namePrefab: "fannypack_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 24, idItem: 24, nameItem: "", namePrefab: "fannypack_4", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 25, idItem: 25, nameItem: "", namePrefab: "fannypack_5", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 26, idItem: 26, nameItem: "", namePrefab: "fannypack_6", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 27, idItem: 27, nameItem: "", namePrefab: "fannypack_7", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 700, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 28, idItem: 28, nameItem: "", namePrefab: "backpack_black", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2, 3 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 29, idItem: 29, nameItem: "", namePrefab: "backpack_white", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2, 3 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 30, idItem: 30, nameItem: "", namePrefab: "backpack_pink", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2, 3 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 31, idItem: 31, nameItem: "", namePrefab: "backpack_purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2, 3 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 32, idItem: 32, nameItem: "", namePrefab: "backpack_orange", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2, 3 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 33, idItem: 33, nameItem: "", namePrefab: "backpack_red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2, 3 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 34, idItem: 34, nameItem: "", namePrefab: "backpack_blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 600, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 2, 3 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 35, idItem: 35, nameItem: "", namePrefab: "Laptop", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 1500, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 36, idItem: 36, nameItem: "", namePrefab: "Mug", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 75, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 37, idItem: 37, nameItem: "", namePrefab: "Thermos", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 100, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 38, idItem: 38, nameItem: "", namePrefab: "Bouquet", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 150, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 39, idItem: 39, nameItem: "", namePrefab: "TeddyBear", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 25, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 40, idItem: 40, nameItem: "", namePrefab: "Football", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 100, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 41, idItem: 41, nameItem: "", namePrefab: "BasketBall", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 100, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 42, idItem: 42, nameItem: "", namePrefab: "Tablet", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 1000, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 43, idItem: 43, nameItem: "", namePrefab: "gingerbread", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 25, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 44, idItem: 44, nameItem: "", namePrefab: "gift_1", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 45, idItem: 45, nameItem: "", namePrefab: "gift_2", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 46, idItem: 46, nameItem: "", namePrefab: "gift_3", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 47, idItem: 47, nameItem: "", namePrefab: "candy_cane", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 25, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));

        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 48, idItem: 48, nameItem: "", namePrefab: "Accessory_VampireCape_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 25, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 3 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 49, idItem: 49, nameItem: "", namePrefab: "Accessory_VampireCape_Purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 25, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 3 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 50, idItem: 50, nameItem: "", namePrefab: "Accessory_VampireCape_BlueWine", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 25, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 1, 3 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 51, idItem: 51, nameItem: "", namePrefab: "Accessory_DevilWings_Red", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 25, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 3 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 52, idItem: 52, nameItem: "", namePrefab: "Accessory_DevilWings_Blue", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 25, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 3 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 53, idItem: 53, nameItem: "", namePrefab: "Accessory_DevilWings_Purple", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 25, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 3 }, psbName: "Halloween2022", psbNameUI: "Halloween2022 UI", ""));
        //San Valentin

        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 54, idItem: 54, nameItem: "", namePrefab: "Daisy", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 55, idItem: 55, nameItem: "", namePrefab: "Rose", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 56, idItem: 56, nameItem: "", namePrefab: "Chocolatebox", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0 }));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 57, idItem: 57, nameItem: "", namePrefab: "KawaiiWings", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 3 }));

        //Easter
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 58, idItem: 58, nameItem: "", namePrefab: "Accessory_Bunnyplush", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 200, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4 }, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 59, idItem: 59, nameItem: "", namePrefab: "Accessory_eggbasket", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 25, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 0, 4 }, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 60, idItem: 60, nameItem: "", namePrefab: "accessory_bunnypaw", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 25, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 4, 5 }, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));
        listItems.Add(new AvatarGenericCatalogItem(ItemType.ACCESORIES, idItemWebClient: 61, idItem: 61, nameItem: "", namePrefab: "accessory_bunnytail", orderInCatalog: 0, activeInCatalog: true, gemsPrice: 25, goldPrice: 0, currencyType: CurrencyType.GOLD_PRICE, layers: new int[] { 3 }, psbName: "Avatar_Easter", psbNameUI: "Avatar_Easter_UI", ""));


        return listItems;
    }
}

[System.Serializable]
public class SerializableCatalogItemList
{
    public List<SerializableCatalogItem> data;

    public SerializableCatalogItemList()
    {
        data = new List<SerializableCatalogItem>();
    }
}

[System.Serializable]
public class SerializableCatalogItem
{
    public string type;
    public int id;
    public string name;
    public string name_prefab;
    public int gold;
    public int gems;
    public string layers;
    public string psbName;
    public string psbNameUI;

    private readonly IItemTypeUtils ItemTypeUtils = Injection.Get<IItemTypeUtils>();

    public SerializableCatalogItem(GenericCatalogItem genericCatalogItem)
    {
        type = ItemTypeUtils.GetNameItemType(genericCatalogItem.ItemType);
        id = genericCatalogItem.IdItem;
        name = genericCatalogItem.NameItem;
        name_prefab = genericCatalogItem.NamePrefab;
        gold = genericCatalogItem.GoldPrice;
        gems = genericCatalogItem.GemsPrice;
        layers = "";
        psbName = null;
        psbNameUI = null;

        AvatarGenericCatalogItem avatarGenericCatalogItem = genericCatalogItem as AvatarGenericCatalogItem;

        if (avatarGenericCatalogItem != null)
        {
            psbName = avatarGenericCatalogItem.PsbName;
            psbNameUI = avatarGenericCatalogItem.PsbNameUI;

            if (avatarGenericCatalogItem.Layers != null)
            {
                layers = string.Join(",", avatarGenericCatalogItem.Layers);
            }
        }
    }
}

[System.Serializable]
public class SerializableBodyCatalogItemList
{
    public List<SerializableBodyCatalogItem> data;

    public SerializableBodyCatalogItemList()
    {
        data = new List<SerializableBodyCatalogItem>();
    }
}

[System.Serializable]
public class SerializableBodyCatalogItem : SerializableCatalogItem
{
    public bool UseBoobs;

    public SerializableBodyCatalogItem(BodyCatalogItem bodyCatalogItem) : base(bodyCatalogItem)
    {
        UseBoobs = bodyCatalogItem.UseBoobs;
    }
}
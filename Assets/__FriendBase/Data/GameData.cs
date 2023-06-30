using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Catalog;
using Data.Bag;
using System.IO;
using System.Linq;
using System;
using Data.Users;
using Data.Rooms;
using FriendsView.Core.Domain;
using System.Threading.Tasks;
using FriendsView.Core.Services;
using Architecture.Injector.Core;
using Firebase.Auth;
using System.Text.RegularExpressions;

namespace Data
{
    public class GameData : IGameData
    {
        public static ItemType[] AvatarItemsType =
        {
            ItemType.BODY, ItemType.EYE, ItemType.EAR, ItemType.EYEBROW, ItemType.HAIR, ItemType.FACE, ItemType.MOUTH,
            ItemType.NOSE, ItemType.UP_PART, ItemType.BOTTOM_PART, ItemType.DRESSES, ItemType.SHOES, ItemType.GLASSES,
            ItemType.ACCESORIES
        };

        public static ItemType[] RoomItemsType =
            { ItemType.CHAIR, ItemType.FLOOR, ItemType.LAMP, ItemType.TABLE, ItemType.FURNITURES_INVENTORY };

        private Dictionary<ItemType, GenericCatalog> catalogsDictionary;
        private Dictionary<ItemType, GenericBag> bagsDictionary;
        private Dictionary<ItemType, AvatarCustomizationRule> avatarCustomizationRules;
        private Dictionary<Game, MinigameInformation> minigameDictionary;
        private UserInformation userInformation = new UserInformation();
        private RoomInformation roomInformation;
        private RoomInformation myHouseInformation;
        private List<FriendData> friendList = new List<FriendData>();
        private bool WasGuest;

        public GameData()
        {
            CreateMinigameMenu();
            CreateCatalogs();
            CreateBags();
            CreateAvatarCustomizationData();
            new LocalCatalog(catalogsDictionary); //TODO delete this line and make and endpoint
            new LocalBag(catalogsDictionary, bagsDictionary); //TODO delete this line and make and endpoint
        }

        void CreateMinigameMenu()
        {
            minigameDictionary = new Dictionary<Game, MinigameInformation>();

            List<Game> miniGames = Enum.GetValues(typeof(Game)).Cast<Game>().ToList();
            foreach (Game gameType in miniGames)
            {
                minigameDictionary.Add(gameType, new MinigameInformation(gameType, userInformation, null, false));
            }
        }

        public Dictionary<Game, MinigameInformation> GetAllMinigamesForMenu()
        {
            return minigameDictionary;
        }

        void CreateBags()
        {
            bagsDictionary = new Dictionary<ItemType, GenericBag>();
            foreach (ItemType itemType in AvatarItemsType)
            {
                bagsDictionary.Add(itemType, new GenericBag(itemType));
            }

            foreach (ItemType itemType in RoomItemsType)
            {
                bagsDictionary.Add(itemType, new FurnituresBag(itemType));
            }

            bagsDictionary.Add(ItemType.ROOM, new GenericBag(ItemType.ROOM));
            bagsDictionary.Add(ItemType.PETS, new GenericBag(ItemType.PETS));
        }

        void CreateCatalogs()
        {
            catalogsDictionary = new Dictionary<ItemType, GenericCatalog>();
            foreach (ItemType itemType in AvatarItemsType)
            {
                catalogsDictionary.Add(itemType, new GenericCatalog(itemType));
            }

            foreach (ItemType itemType in RoomItemsType)
            {
                catalogsDictionary.Add(itemType, new GenericCatalog(itemType));
            }

            catalogsDictionary.Add(ItemType.PETS, new GenericCatalog(ItemType.PETS));
            catalogsDictionary.Add(ItemType.COLOR, new GenericCatalog(ItemType.COLOR));
            catalogsDictionary.Add(ItemType.ROOM, new GenericCatalog(ItemType.ROOM));
        }

        public AvatarGenericCatalogItem GetDefaultAvatarCatalogItem(ItemType itemType)
        {
            if (Array.IndexOf(AvatarItemsType, itemType) < 0)
            {
                return null;
            }

            int idItemDefault = avatarCustomizationRules[itemType].IdItemDefault;
            return catalogsDictionary[itemType].GetItem(idItemDefault) as AvatarGenericCatalogItem;
        }

        public ColorCatalogItem GetDefaultColorCatalogItem(ItemType itemType)
        {
            if (Array.IndexOf(AvatarItemsType, itemType) < 0)
            {
                return null;
            }

            int idItemDefault = avatarCustomizationRules[itemType].IdColorDefault;
            return catalogsDictionary[itemType].GetItem(idItemDefault) as ColorCatalogItem;
        }

        void CreateAvatarCustomizationData()
        {
            avatarCustomizationRules = new Dictionary<ItemType, AvatarCustomizationRule>();

            avatarCustomizationRules.Add(ItemType.BODY,
                new AvatarCustomizationRule(itemType: ItemType.BODY, psbName: "Avatar-body", psbNameUI: "DNA_UI",
                    gameObjectNames: new string[] { "front_arm", "back_arm", "front_foot", "back_foot", "torso" },
                    subfixForGameObjects: new string[] { "front_arm", "back_arm", "front_leg", "back_leg", "torso" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.FIX,
                    applyColorOnGameObject: new bool[] { true, true, true, true, true }, isBoobMaster: true,
                    useBoobs: -1, forceEnableItemTypes: null, forceDisableItemTypes: null,
                    colorIdsAvailable: new int[] { 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 },
                    dependencyColor: ItemType.NONE, idItemDefault: 0, idColorDefault: 24, deselectable: false,
                    canDisableColor: false));
            avatarCustomizationRules.Add(ItemType.EYE,
                new AvatarCustomizationRule(itemType: ItemType.EYE, psbName: "Avatar-eyes", psbNameUI: "DNA_UI",
                    gameObjectNames: new string[] { "eyes_bg", "eye_outline", "pupil_L", "pupil_R", "eyes_closed" },
                    subfixForGameObjects: new string[] { "bg", "outline", "L", "R", "closed" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.FIX,
                    applyColorOnGameObject: new bool[] { false, false, true, true, false }, isBoobMaster: false,
                    useBoobs: -1, forceEnableItemTypes: null, forceDisableItemTypes: null,
                    colorIdsAvailable: new int[] { 1, 2, 3, 4, 5, 6, 7 }, dependencyColor: ItemType.NONE,
                    idItemDefault: 0, idColorDefault: 4, deselectable: false, canDisableColor: false));
            avatarCustomizationRules.Add(ItemType.EAR,
                new AvatarCustomizationRule(itemType: ItemType.EAR, psbName: "Avatar-ear", psbNameUI: "DNA_UI",
                    gameObjectNames: new string[] { "ear" }, subfixForGameObjects: new string[] { "" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.FIX,
                    applyColorOnGameObject: new bool[] { true }, isBoobMaster: false, useBoobs: -1,
                    forceEnableItemTypes: null, forceDisableItemTypes: null, colorIdsAvailable: new int[] { },
                    dependencyColor: ItemType.BODY, idItemDefault: 0, idColorDefault: 1, deselectable: false,
                    canDisableColor: false));
            avatarCustomizationRules.Add(ItemType.EYEBROW,
                new AvatarCustomizationRule(itemType: ItemType.EYEBROW, psbName: "Avatar-eyebrows", psbNameUI: "DNA_UI",
                    gameObjectNames: new string[] { "eyebrows" }, subfixForGameObjects: new string[] { "" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.FIX,
                    applyColorOnGameObject: new bool[] { true }, isBoobMaster: false, useBoobs: -1,
                    forceEnableItemTypes: null, forceDisableItemTypes: null, colorIdsAvailable: new int[] { },
                    dependencyColor: ItemType.HAIR, idItemDefault: 0, idColorDefault: 1, deselectable: false,
                    canDisableColor: false));
            avatarCustomizationRules.Add(ItemType.HAIR,
                new AvatarCustomizationRule(itemType: ItemType.HAIR, psbName: "Avatar-hairstyles", psbNameUI: "DNA_UI",
                    gameObjectNames: new string[] { "hair_1", "hair_2", "hair_3" },
                    subfixForGameObjects: new string[] { "1", "2", "3" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.SEQUENCE,
                    applyColorOnGameObject: new bool[] { true, true, true }, isBoobMaster: false, useBoobs: -1,
                    forceEnableItemTypes: null, forceDisableItemTypes: null,
                    colorIdsAvailable: new int[] { 8, 9, 10, 11, 12, 13, 14, 15, 56 }, dependencyColor: ItemType.NONE,
                    idItemDefault: 0, idColorDefault: 15, deselectable: false, canDisableColor: false));
            avatarCustomizationRules.Add(ItemType.FACE,
                new AvatarCustomizationRule(itemType: ItemType.FACE, psbName: "Avatar-faceshape", psbNameUI: "DNA_UI",
                    gameObjectNames: new string[] { "faceshape" }, subfixForGameObjects: new string[] { "" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.FIX,
                    applyColorOnGameObject: new bool[] { true }, isBoobMaster: false, useBoobs: -1,
                    forceEnableItemTypes: null, forceDisableItemTypes: null, colorIdsAvailable: new int[] { },
                    dependencyColor: ItemType.BODY, idItemDefault: 0, idColorDefault: 1, deselectable: false,
                    canDisableColor: false));
            avatarCustomizationRules.Add(ItemType.MOUTH,
                new AvatarCustomizationRule(itemType: ItemType.MOUTH, psbName: "Avatar-mouth", psbNameUI: "DNA_UI",
                    gameObjectNames: new string[]
                        { "mouth_closed", "mouth_closed_lips", "mouth_open", "mouth_open_lips" },
                    subfixForGameObjects: new string[] { "closed_NC", "closed", "open", "open_lips" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.FIX,
                    applyColorOnGameObject: new bool[] { false, true, false, true }, isBoobMaster: false, useBoobs: -1,
                    forceEnableItemTypes: null, forceDisableItemTypes: null,
                    colorIdsAvailable: new int[] { 0, 16, 17, 18, 19, 20, 21, 22 }, dependencyColor: ItemType.NONE,
                    idItemDefault: 0, idColorDefault: 0, deselectable: false, canDisableColor: true));
            avatarCustomizationRules.Add(ItemType.NOSE,
                new AvatarCustomizationRule(itemType: ItemType.NOSE, psbName: "Avatar-nose", psbNameUI: "DNA_UI",
                    gameObjectNames: new string[] { "nose" }, subfixForGameObjects: new string[] { "" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.FIX,
                    applyColorOnGameObject: new bool[] { true }, isBoobMaster: false, useBoobs: -1,
                    forceEnableItemTypes: null, forceDisableItemTypes: null, colorIdsAvailable: new int[] { },
                    dependencyColor: ItemType.BODY, idItemDefault: 0, idColorDefault: 1, deselectable: false,
                    canDisableColor: false));
            avatarCustomizationRules.Add(ItemType.UP_PART,
                new AvatarCustomizationRule(itemType: ItemType.UP_PART, psbName: "Tops", psbNameUI: "Tops_UI",
                    gameObjectNames: new string[] { "top_A", "sleeve_R", "sleeve_L" },
                    subfixForGameObjects: new string[] { "T", "R", "L" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.SEQUENCE,
                    applyColorOnGameObject: new bool[] { false, false, false }, isBoobMaster: false, useBoobs: 0,
                    forceEnableItemTypes: new ItemType[] { ItemType.BOTTOM_PART },
                    forceDisableItemTypes: new ItemType[] { ItemType.DRESSES }, colorIdsAvailable: new int[] { },
                    dependencyColor: ItemType.NONE, idItemDefault: 0, idColorDefault: 1, deselectable: false,
                    canDisableColor: false));
            avatarCustomizationRules.Add(ItemType.BOTTOM_PART,
                new AvatarCustomizationRule(itemType: ItemType.BOTTOM_PART, psbName: "Bottoms", psbNameUI: "Bottoms_UI",
                    gameObjectNames: new string[] { "pants_hips", "pants_L", "pants_R", "pants_skirt" },
                    subfixForGameObjects: new string[] { "hips", "L", "R", "hips" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.SEQUENCE,
                    applyColorOnGameObject: new bool[] { false, false, false, false }, isBoobMaster: false,
                    useBoobs: -1, forceEnableItemTypes: new ItemType[] { ItemType.UP_PART },
                    forceDisableItemTypes: new ItemType[] { ItemType.DRESSES }, colorIdsAvailable: new int[] { },
                    dependencyColor: ItemType.NONE, idItemDefault: 0, idColorDefault: 1, deselectable: false,
                    canDisableColor: false));
            avatarCustomizationRules.Add(ItemType.DRESSES,
                new AvatarCustomizationRule(itemType: ItemType.DRESSES, psbName: "Dress", psbNameUI: "Dress_UI",
                    gameObjectNames: new string[] { "dress", "sleeve_R", "sleeve_L" },
                    subfixForGameObjects: new string[] { "T", "R", "L" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.SEQUENCE,
                    applyColorOnGameObject: new bool[] { false, false, false }, isBoobMaster: false, useBoobs: 0,
                    forceEnableItemTypes: null,
                    forceDisableItemTypes: new ItemType[] { ItemType.UP_PART, ItemType.BOTTOM_PART },
                    colorIdsAvailable: new int[] { }, dependencyColor: ItemType.NONE, idItemDefault: -1,
                    idColorDefault: 1, deselectable: false, canDisableColor: false));
            avatarCustomizationRules.Add(ItemType.SHOES,
                new AvatarCustomizationRule(itemType: ItemType.SHOES, psbName: "Shoes", psbNameUI: "Shoes_UI",
                    gameObjectNames: new string[] { "shoes_L", "shoes_R" },
                    subfixForGameObjects: new string[] { "L", "R" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.FIX,
                    applyColorOnGameObject: new bool[] { false, false }, isBoobMaster: false, useBoobs: -1,
                    forceEnableItemTypes: null, forceDisableItemTypes: null, colorIdsAvailable: new int[] { },
                    dependencyColor: ItemType.NONE, idItemDefault: 0, idColorDefault: 1, deselectable: false,
                    canDisableColor: false));
            avatarCustomizationRules.Add(ItemType.GLASSES,
                new AvatarCustomizationRule(itemType: ItemType.GLASSES, psbName: "Glasses", psbNameUI: "Glasses_UI",
                    gameObjectNames: new string[] { "glasses", "glasses_top" },
                    subfixForGameObjects: new string[] { "", "" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.SEQUENCE,
                    applyColorOnGameObject: new bool[] { false, false }, isBoobMaster: false, useBoobs: -1,
                    forceEnableItemTypes: null, forceDisableItemTypes: null, colorIdsAvailable: new int[] { },
                    dependencyColor: ItemType.NONE, idItemDefault: -1, idColorDefault: 1, deselectable: true,
                    canDisableColor: false));
            avatarCustomizationRules.Add(ItemType.ACCESORIES,
                new AvatarCustomizationRule(itemType: ItemType.ACCESORIES, psbName: "Accessories",
                    psbNameUI: "Accessories_UI",
                    gameObjectNames: new string[]
                        { "accessory_L", "accessory_A", "accessory_B", "accessory_C", "accessory_R", "accessory_L2" },
                    subfixForGameObjects: new string[] { "L", "A", "B", "C", "R", "L2" },
                    fillGameObjectType: AvatarCustomizationRule.FillGameObjectsType.SEQUENCE,
                    applyColorOnGameObject: new bool[] { false, false, false, false, false, false },
                    isBoobMaster: false, useBoobs: -1, forceEnableItemTypes: null, forceDisableItemTypes: null,
                    colorIdsAvailable: new int[] { }, dependencyColor: ItemType.NONE, idItemDefault: -1,
                    idColorDefault: 1, deselectable: true, canDisableColor: false));
        }

        public Dictionary<ItemType, AvatarCustomizationRule> GetAvatarCustomizationRules()
        {
            return avatarCustomizationRules;
        }

        public AvatarCustomizationRule GetAvatarCustomizationRuleByItemType(ItemType itemType)
        {
            if (avatarCustomizationRules.ContainsKey(itemType))
            {
                return avatarCustomizationRules[itemType];
            }

            return null;
        }

        public GenericCatalog GetCatalogByItemType(ItemType itemType)
        {
            if (catalogsDictionary.ContainsKey(itemType))
            {
                return catalogsDictionary[itemType];
            }

            return null;
        }

        public GenericBag GetBagByItemType(ItemType itemType)
        {
            if (bagsDictionary.ContainsKey(itemType))
            {
                return bagsDictionary[itemType];
            }

            return null;
        }

        public bool IsGuest()
        {
            if (FirebaseAuth.DefaultInstance.CurrentUser == null)
            {
                return false;
            }

            Debug.Log("--IsAnonymous: " + FirebaseAuth.DefaultInstance.CurrentUser.IsAnonymous);
            return FirebaseAuth.DefaultInstance.CurrentUser.IsAnonymous;
        }

        public void SetFriendlist(List<FriendData> friendList)
        {
            Debug.Log("FRINEDSSDASD");
            this.friendList = friendList;
        }

        public void SetWasGuest(bool wasGuest)
        {
            WasGuest = wasGuest;
        }

        public bool GetWasGuest()
        {
            return WasGuest;
        }

        public void AddFriend(FriendData friend)
        {
            this.friendList.Add(friend);
        }

        public void AddFriend(FriendRequestData friend)
        {
            FriendData newFriend = new FriendData(friend.requesterUserID, friend.username, friend.realName, "", "",
                friend.friendCount, 0, 0, 0, "", "", avatarCustomizationData: friend.avatarCustomizationData);
            this.friendList.Add(newFriend);
        }

        public List<FriendData> GetFriendList()
        {
            return this.friendList;
        }

        public List<FriendData> GetDummyFriendList()
        {
            Dictionary<ItemType, AvatarCustomizationRule> rules;
            List<FriendData> dummyFriendList = new List<FriendData>();
            rules = this.GetAvatarCustomizationRules();
            int friendTestAmount = 50;
            if (this.friendList.Count == friendTestAmount)
                return friendList;

            List<string> randomNames = new List<string>()
            {
                "Marta",
                "Sol",
                "Jenniffer",
                "Fernando",
                "Jorge",
                "Carmen",
                "Matias",
                "Sebastian",
                "Aurelia",
                "Valentina",
                "Agustin",
                "Alfa",
                "Marcos",
                "Catalina",
                "Tomas",
                "Dana",
                "Cristian"
            };

            for (int i = 0; i < friendTestAmount; i++)
            {
                //CREATE RANDOM SKIN
                var amountBodyParts = GameData.AvatarItemsType.Length;

                var dataUnits = new Dictionary<ItemType, AvatarCustomizationDataUnit>();

                for (var x = 0; x < amountBodyParts; x++)
                {
                    var itemType = GameData.AvatarItemsType[x];
                    var idItem = GetCatalogByItemType(itemType).GetRandomItem().IdItem;

                    //If the item is deselectable we make a random and see if we disable it
                    if (rules[itemType].Deselectable || itemType == ItemType.DRESSES)
                    {
                        if (UnityEngine.Random.Range(0, 1000) > 500)
                        {
                            idItem = -1;
                        }
                    }

                    int idColor = GetCatalogByItemType(ItemType.COLOR).GetRandomItem().IdItem;

                    int[] colorsAvalable = rules[itemType].ColorIdsAvailable;
                    if (colorsAvalable.Length > 0)
                    {
                        idColor = colorsAvalable[UnityEngine.Random.Range(0, colorsAvalable.Length)];
                    }

                    ColorCatalogItem color = GetCatalogByItemType(ItemType.COLOR).GetItem(idColor) as ColorCatalogItem;
                    AvatarGenericCatalogItem item =
                        GetCatalogByItemType(itemType).GetItem(idItem) as AvatarGenericCatalogItem;

                    dataUnits.Add(itemType, new AvatarCustomizationDataUnit(itemType, item, color));
                }

                string Name = randomNames[UnityEngine.Random.Range(0, randomNames.Count)] + " " + i;
                FriendData newFriend = new FriendData(i, Name, Name, "", "", 0, 0, 0, 0, "", "",
                    avatarCustomizationData: new AvatarCustomizationData(dataUnits));
                dummyFriendList.Add(newFriend);
            }

            this.friendList = dummyFriendList;

            return dummyFriendList;
        }

        public void AddItemsToBag(List<GenericBagItem> items)
        {
            foreach (GenericBagItem item in items)
            {
                if (item.ItemType == ItemType.PETS)
                {
                }

                GenericBag bag = GetBagByItemType(item.ItemType);
                if (bag != null)
                {
                    bag.AddItem(item);
                }
            }
        }

        public void AddItemToBag(GenericBagItem item)
        {
            GenericBag bag = GetBagByItemType(item.ItemType);
            if (bag != null)
            {
                bag.AddItem(item);
            }
        }

        public void InitializeCatalogs(List<GenericCatalogItem> listItems)
        {
            var itemsByItemType = listItems.GroupBy(t => t.ItemType);

            foreach (var grouping in itemsByItemType)
            {
                GenericCatalog catalog = GetCatalogByItemType(grouping.Key);
                if (catalog != null)
                {
                    List<GenericCatalogItem> items = new List<GenericCatalogItem>();
                    foreach (var item in grouping)
                    {
                        items.Add(item);
                    }

                    catalog.Initialize(items);
                }
            }
        }

        public UserInformation GetUserInformation()
        {
            return userInformation;
        }

        public RoomInformation GetRoomInformation()
        {
            return roomInformation;
        }

        public void SetRoomInformation(RoomInformation roomInformation)
        {
            this.roomInformation = roomInformation.Duplicate();
        }

        public void SetBlockedList(List<string> blockedPlayers)
        {
        }

        public void SetMyHouseInformation(RoomInformation roomInformation)
        {
            this.myHouseInformation = roomInformation.Duplicate();
        }

        public RoomInformation GetMyHouseInformation()
        {
            return myHouseInformation;
        }

        public void AddSkinToInventory(AvatarCustomizationData avatarCustomizationData)
        {
            foreach (ItemType itemType in GameData.AvatarItemsType)
            {
                GenericCatalogItem genericCatalogItem = avatarCustomizationData.GetDataUnit(itemType).AvatarObjCat;
                if (genericCatalogItem != null)
                {
                    AddItemToBag(new GenericBagItem(itemType, genericCatalogItem.IdItem, 1, genericCatalogItem));
                }
            }
        }
    }
}

using Data.Catalog;
using Data.Bag;
using System.Collections.Generic;
using Data.Users;
using Data.Rooms;
using FriendsView.Core.Domain;

namespace Data
{
    public interface IGameData
    {
        GenericCatalog GetCatalogByItemType(ItemType itemType);
        Dictionary<ItemType, AvatarCustomizationRule> GetAvatarCustomizationRules();
        AvatarCustomizationRule GetAvatarCustomizationRuleByItemType(ItemType itemType);
        AvatarGenericCatalogItem GetDefaultAvatarCatalogItem(ItemType itemType);
        ColorCatalogItem GetDefaultColorCatalogItem(ItemType itemType);
        GenericBag GetBagByItemType(ItemType itemType);
        void AddItemsToBag(List<GenericBagItem> items);
        void AddItemToBag(GenericBagItem item);
        void InitializeCatalogs(List<GenericCatalogItem> listItems);
        Dictionary<Game, MinigameInformation> GetAllMinigamesForMenu();
        UserInformation GetUserInformation();
        void AddSkinToInventory(AvatarCustomizationData avatarCustomizationData);
        RoomInformation GetRoomInformation();
        void SetRoomInformation(RoomInformation roomInformation);
        void SetMyHouseInformation(RoomInformation roomInformation);
        RoomInformation GetMyHouseInformation();

        void SetFriendlist(List<FriendData> friendList);
        void AddFriend(FriendData friend);
        void AddFriend(FriendRequestData friend);
        List<FriendData> GetFriendList();
        List<FriendData> GetDummyFriendList();

        bool IsGuest();

        void SetWasGuest(bool wasGuest);

        bool GetWasGuest();
    }
}
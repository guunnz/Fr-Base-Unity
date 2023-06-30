using Data.Users;
using UnityEngine;

namespace FriendsView.Core.Domain
{
    public class FriendData
    {
        public readonly string username;
        public readonly string realName;
        public readonly string fireBaseUID;
        public readonly int userID;
        public readonly int gems;
        public readonly int gold;
        //public readonly bool online;
        public readonly int friendCount;
        public readonly int friendRequestsCount;
        public readonly string roomType;
        public readonly string roomInstanceId;
        //public readonly string roomDisplayName;
        public readonly string roomNamePrefab;
        public readonly AvatarCustomizationData avatarCustomizationData;
        public Sprite playerHeadSprite;

        public FriendData(int userID, string fireBaseUid, string username, string realName, string roomType,
            int friendCount, int gems, int gold, int friendRequestsCount, string roomInstanceId, string roomNamePrefab,
            AvatarCustomizationData avatarCustomizationData)
        {
            this.username = username;
            this.realName = realName;
            this.userID = userID;
            //this.online = online;
            this.roomType = roomType;
            this.friendCount = friendCount;
            this.gems = gems;
            this.gold = gold;
            this.friendRequestsCount = friendRequestsCount;
            this.avatarCustomizationData = avatarCustomizationData;
            this.roomNamePrefab = roomNamePrefab;
            this.roomInstanceId = roomInstanceId;
            fireBaseUID = fireBaseUid;
        }

        public bool IsInPublicRoom => roomType.Equals(RoomType.PUBLIC);

        public void SetHeadSprite(Sprite headSprite)
        {
            this.playerHeadSprite = headSprite;
        }
        public Sprite GetHeadSprite()
        {
            return playerHeadSprite;
        }
        public UserData ToUserData()
        {
            return new UserData(fireBaseUID, userID, gems, gold, realName, username, friendCount, friendRequestsCount,
                roomType);
        }
    }
}
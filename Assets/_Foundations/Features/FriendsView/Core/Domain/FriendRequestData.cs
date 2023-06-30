using Data.Users;

namespace FriendsView.Core.Domain
{
    public class FriendRequestData

    {
        public readonly string username;
        public readonly string realName;
        public readonly string requestStatus;
        public readonly int friendRequestID;
        public readonly int requesterUserID;
        public readonly int friendCount;
        public readonly string fireBaseUid;
        public readonly AvatarCustomizationData avatarCustomizationData;

        public FriendRequestData(int friendRequestID, string username, string realName, int requesterUserID,
            string fireBaseUid, RequestStatusOptions requestStatus, int friendCount, AvatarCustomizationData avatarCustomizationData)
        {
            this.friendRequestID = friendRequestID;
            this.username = username;
            this.realName = realName;
            this.requesterUserID = requesterUserID;
            this.requestStatus = requestStatus.ToString().ToLower();
            this.fireBaseUid = fireBaseUid;
            this.friendCount = friendCount;
            this.avatarCustomizationData = avatarCustomizationData;
        }
    }
}
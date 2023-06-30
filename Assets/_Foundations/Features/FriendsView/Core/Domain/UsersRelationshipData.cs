namespace FriendsView.Core.Domain
{
    public class UsersRelationshipData
    {
        public readonly UserRelationship relationship;
        public readonly FriendData friendData;
        public readonly FriendRequestData requestData;

        public UsersRelationshipData(UserRelationship relationship)
        {
            this.relationship = relationship;
        }

        public UsersRelationshipData(FriendData friendData, UserRelationship relationship)
        {
            this.friendData = friendData;
            this.relationship = relationship;
        }

        public UsersRelationshipData(FriendRequestData friendRequestData, UserRelationship relationship)
        {
            this.requestData = friendRequestData;
            this.relationship = relationship;
        }
    }
}
//Todo: Unified constructors
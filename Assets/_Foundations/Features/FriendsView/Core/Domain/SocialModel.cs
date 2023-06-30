namespace FriendsView.Core.Domain
{
    public enum RequestStatusOptions
    {
        Accepted,
        Pending,
        Rejected
    }

    public enum UserRelationship
    {
        Friends,
        RequestSent,
        RequestReceived,
        Strangers,
        Unknown
    }
}
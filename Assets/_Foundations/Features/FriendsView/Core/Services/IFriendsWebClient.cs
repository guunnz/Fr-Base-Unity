using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FriendsView.Core.Domain;
using UniRx;
using Web;

namespace FriendsView.Core.Services
{
    public interface IFriendsWebClient
    {
        Task<List<FriendData>> GetFriendsList();
        Task<FriendData> GetFriend(string id);
        Task <List<FriendRequestData>> GetFriendRequestsList();
        IObservable<Unit> CreateFriend(FriendRequestData friendData);
        IObservable<Unit> UpdateFriendRequest(FriendRequestData data, bool sent);
        IObservable<Unit> AddFriendRequest(UserData userData);
        IObservable<RequestInfo> DeleteFriend(FriendData friendData);
        IObservable<UsersRelationshipData> GetUsersRelationship(UserData id);
        IObservable<UserData> GetPersonalUserData();
        Task ReportUser(UserData userData, string reason, string description);
        IObservable<Unit> BlockUser(UserData userData);
        Task <UserData> GetUser(string userFirebaseUid);
    }
}

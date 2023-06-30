using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FriendsView.Core.Domain;

namespace FriendsView.Core.Services
{
    public interface IFriendsNotificationsServices
    {
        Task<List<FriendRequestData>> RetrieveRequestsList();
        void OpenServices();
        List<FriendRequestData> RequestsList { get; }
        List<FriendData> FriendsList { get; }
        int GetPendingRequestsNumber(List<FriendRequestData> list);
        IEnumerator CoroutineUpdateData();
        void CloseServices();
        Task<List<FriendData>> RetrieveFriendsList();
        event Action<string, List<FriendRequestData>> OnNewRequest;
        event Action<int> OnRequestsNumberChanged;
        event Action<string, string, List<FriendData>> OnNewFriend;
        void InsertTrimNewFriend(int userId, string username, string firebaseUid);
        event Action<string> OnRequestConfirmed;
    }
}
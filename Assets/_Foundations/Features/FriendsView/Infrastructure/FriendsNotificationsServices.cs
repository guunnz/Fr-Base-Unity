using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using Data.Users;
using FriendsView.Core.Domain;
using FriendsView.Core.Services;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace FriendsView.Infrastructure
{
    [UsedImplicitly]
    public class FriendsNotificationsServices : IFriendsNotificationsServices
    {
        public event Action<string, List<FriendRequestData>> OnNewRequest;
        public event Action<string, string, List<FriendData>> OnNewFriend;
        public event Action<int> OnRequestsNumberChanged;
        public event Action<string> OnRequestConfirmed;

        List<FriendRequestData> friendRequestsList = new List<FriendRequestData>();
        List<FriendData> friendsList = new List<FriendData>();

        readonly IFriendsWebClient webClient;

        private CompositeDisposable UpdateDisposables = new CompositeDisposable();
        private CompositeDisposable disposables = new CompositeDisposable();

        const float updateTime = 10f;

        public FriendsNotificationsServices()
        {
            webClient = Injection.Get<IFriendsWebClient>();
            UpdateDisposables.AddTo(disposables);
        }

        public List<FriendRequestData> RequestsList => friendRequestsList;
        public List<FriendData> FriendsList => friendsList;

        public async void OpenServices()
        {
            friendRequestsList = await RetrieveRequestsList();

            friendsList = await RetrieveFriendsList();

            CoroutineUpdateData().ToObservable().Subscribe().AddTo(disposables);

            var requestsNumber = GetPendingRequestsNumber(friendRequestsList);
            if (requestsNumber > 0) OnRequestsNumberChanged?.Invoke(requestsNumber);
        }

        public void CloseServices()
        {
            disposables.Clear();
        }

        public IEnumerator CoroutineUpdateData()
        {
            while (true)
            {
                yield return new WaitForSeconds(updateTime);
                UpdateData();
                UpdateDisposables.Clear();
            }
        }

        async void UpdateData()
        {
            var newRequestList = await RetrieveRequestsList();
            var newFriendList = await RetrieveFriendsList();

            CheckChanges(FilterPendingRequest(newRequestList), newFriendList);

            friendRequestsList.Clear();
            friendRequestsList.AddRange(newRequestList);

            friendsList.Clear();
            friendsList.AddRange(newFriendList);
        }


        void CheckChanges(List<FriendRequestData> newRequestList, List<FriendData> newFriendList)
        {
            //Check new requests ************************************************************

            var newRequestsNames = newRequestList.Select(x => x.username);
            var currentRequestsNames = FilterPendingRequest(friendRequestsList).Select(x => x.username);

            IEnumerable<string> newRequests = newRequestsNames.Except(currentRequestsNames);

            var requestsNumber = GetPendingRequestsNumber(newRequestList);
            OnRequestsNumberChanged?.Invoke(requestsNumber);

            foreach (var requesterUserName in newRequests)
            {
                OnNewRequest?.Invoke(requesterUserName, newRequestList);
            }

            //Check new friends *************************************************************

            var newFriendsNames = newFriendList.Select(x => x.username);
            var currentFriendsNames = friendsList.Select(x => x.username);

            IEnumerable<string> resultNames = newFriendsNames.Except(currentFriendsNames);

            foreach (var newFriendUsername in resultNames)
            {
                var friendData = newFriendList.Find(x => x.username.Equals(newFriendUsername));
                OnNewFriend?.Invoke(newFriendUsername, friendData.fireBaseUID, newFriendList);
            }
        }

        public void InsertTrimNewFriend(int userId, string username, string firebaseUid)
        {
            var temporalFriendData = new FriendData(userId, firebaseUid, username, "", "",
                0, 0, 0, 0, null, null, new AvatarCustomizationData());

            friendsList.Add(temporalFriendData);
            OnRequestConfirmed?.Invoke(firebaseUid);
        }

        public async Task<List<FriendRequestData>> RetrieveRequestsList()
        {
            return await webClient.GetFriendRequestsList();
        }

        public async Task<List<FriendData>> RetrieveFriendsList()
        {
            return await webClient.GetFriendsList();
        }

        public int GetPendingRequestsNumber(List<FriendRequestData> requestList)
        {
            return FilterPendingRequest(requestList).Count;
        }

        List<FriendRequestData> FilterPendingRequest(List<FriendRequestData> list)
        {
            return list.FindAll(x => string.Equals(x.requestStatus, "pending"));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthFlow.EndAuth.Repo;
using AuthFlow.Firebase.Core.Actions;
using Data;
using Data.Users;
using FriendsView.Core.Domain;
using FriendsView.Core.Services;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UniRx;
using Web;
using WebClientTools.Core.Services;

namespace FriendsView.Infrastructure
{
    [UsedImplicitly]
    public class FriendsWebClient : IFriendsWebClient
    {
        readonly GetFirebaseUid getFirebaseUid;
        readonly IWebHeadersBuilder headersBuilder;
        readonly IGameData gameData;


        public FriendsWebClient(GetFirebaseUid getFirebaseUid, ILocalUserInfo localUserInfo,
            IWebHeadersBuilder headersBuilder, IGameData gameData)
        {
            this.getFirebaseUid = getFirebaseUid;

            this.headersBuilder = headersBuilder;

            this.gameData = gameData;
        }

        string GetPersonalID()
        {
            return gameData.GetUserInformation().UserId.ToString();
        }

        public async Task<FriendData> GetFriend(string id)
        {
            var endpoint = await GetEndpoint(EndPointComplement.Friend, id);
            var header = await headersBuilder.BearerToken;
            var resp = await WebClient.Get(endpoint, false, header);

            return ToFriendsModel(resp.json);
        }

        public async Task ReportUser(UserData userData, string reason, string description)
        {
            description ??= "";
            var report = new JObject
            {
                ["reason"] = reason,
                ["user_id"] = userData.userID,
                ["user_created_id"] = int.Parse(GetPersonalID()),
                ["description"] = description
            };
            var json = new JObject { ["report"] = report };

            var endpoint = await GetEndpoint(EndPointComplement.ReportUser, GetPersonalID());
            var header = await headersBuilder.BearerToken;
            await WebClient.Post(endpoint, json, true, header);
        }

        public async Task<UserData> GetUser(string userFirebaseUid)
        {
            var endpoint = await GetEndpoint(EndPointComplement.User, userFirebaseUid);
            var header = await headersBuilder.BearerToken;
            var resp = await WebClient.Get(endpoint, false, header);

            return ToUserModel(resp.json);
        }

        public async Task<List<FriendData>> GetFriendsList()
        {
            var endpoint = await GetEndpoint(EndPointComplement.FriendsList, GetPersonalID());
            var header = await headersBuilder.BearerToken;
            var resp = await WebClient.Get(endpoint, false, header);
            gameData.SetFriendlist(ToFriendsListModel(resp.json));
            return ToFriendsListModel(resp.json);
        }


        public async Task<List<FriendRequestData>> GetFriendRequestsList()
        {
            var endPoint = await GetEndpoint(EndPointComplement.FriendRequestsList, GetPersonalID());
            var header = await headersBuilder.BearerToken;
            var resp = await WebClient.Get(endPoint, false, header);

            return ToFriendRequestsListModel(resp.json);
        }

        public IObservable<UsersRelationshipData> GetUsersRelationship(UserData otherUserData)
        {
            return GetEndpoint(EndPointComplement.Relationship, otherUserData.userID.ToString())
                .SelectMany(selector: endpoint => headersBuilder.BearerToken.Select(token => (token, endpoint)))
                .SelectMany(pair =>
                    WebClient
                        .Get(pair.endpoint, false, pair.token)
                        .Select(r => r.json)
                        .Select(ToUserRelationshipModel)
                        .ObserveOnMainThread());
        }

        public IObservable<Unit> CreateFriend(FriendRequestData friendData)
        {
            var friend = new JObject();

            friend["friend_id"] = friendData.requesterUserID;
            var json = new JObject { ["friend"] = friend };
            gameData.AddFriend(friendData);
            return GetEndpoint(EndPointComplement.AddFriend, friendData.requesterUserID.ToString())
                .SelectMany(selector: endpoint => headersBuilder.BearerToken.Select(token => (token, endpoint)))
                .SelectMany(pair =>
                    WebClient
                        .Post(pair.endpoint, json, true, pair.token))
                .AsUnitObservable();
        }

        public IObservable<Unit> UpdateFriendRequest(FriendRequestData data, bool sent)
        {
            if (sent)
            {
                var friendRequest = new JObject
                {
                    ["requester_user_id"] = int.Parse(GetPersonalID()),
                    ["user_id"] = data.requesterUserID,
                    ["status"] = data.requestStatus.ToLower()
                };

                var json = new JObject { ["friend_request"] = friendRequest };

                return GetEndpoint(EndPointComplement.UpdateSentFriendRequest, data.requesterUserID.ToString(),
                        data.friendRequestID.ToString())
                    .SelectMany(selector: endpoint => headersBuilder.BearerToken.Select(token => (token, endpoint)))
                    .SelectMany(pair =>
                        WebClient
                            .Put(pair.endpoint, json, true, pair.token))
                    .AsUnitObservable();
            }
            else
            {
                var friendRequest = new JObject
                {
                    ["requester_user_id"] = data.requesterUserID,
                    ["user_id"] = int.Parse(GetPersonalID()),
                    ["status"] = data.requestStatus.ToLower()
                };

                var json = new JObject { ["friend_request"] = friendRequest };

                return GetEndpoint(EndPointComplement.UpdateReceivedFriendRequest, data.friendRequestID.ToString())
                    .SelectMany(selector: endpoint => headersBuilder.BearerToken.Select(token => (token, endpoint)))
                    .SelectMany(pair =>
                        WebClient
                            .Put(pair.endpoint, json, true, pair.token))
                    .AsUnitObservable();
            }
        }

        public IObservable<Unit> AddFriendRequest(UserData userData)
        {
            var friendRequest = new JObject
            {
                ["requester_user_id"] = GetPersonalID(),
                ["id"] = userData.userID,
                ["status"] = "pending"
            };
            var json = new JObject { ["friend_request"] = friendRequest };

            return GetEndpoint(EndPointComplement.AddFriendRequest, userData.userID.ToString())
                .SelectMany(selector: endpoint => headersBuilder.BearerToken.Select(token => (token, endpoint)))
                .SelectMany(pair =>
                    WebClient
                        .Post(pair.endpoint, json, true, pair.token))
                .AsUnitObservable();
        }

        public IObservable<Unit> SimulateFriendRequest(FriendRequestData friendData)
        {
            var friendRequest = new JObject
            {
                ["requester_user_id"] = friendData.requesterUserID,
                ["id"] = int.Parse(GetPersonalID()),
                ["status"] = "pending"
            };
            var json = new JObject { ["friend_request"] = friendRequest };

            return GetEndpoint(EndPointComplement.SimulateFriendRequest, GetPersonalID())
                .SelectMany(selector: endpoint => headersBuilder.BearerToken.Select(token => (token, endpoint)))
                .SelectMany(pair =>
                    WebClient
                        .Post(pair.endpoint, json, true, pair.token))
                .AsUnitObservable();
        }

        public IObservable<RequestInfo> DeleteFriend(FriendData friendData)
        {
            return GetEndpoint(EndPointComplement.DeleteFriend, friendData.userID.ToString())
                .SelectMany(selector: endpoint => headersBuilder.BearerToken.Select(token => (token, endpoint)))
                .SelectMany(pair =>
                    WebClient
                        .Delete(pair.endpoint, false, pair.token)
                        .ObserveOnMainThread());
        }

        public IObservable<UserData> GetPersonalUserData()
        {
            return GetEndpoint(EndPointComplement.PersonalData, "")
                .SelectMany(selector: endpoint => headersBuilder.BearerToken.Select(token => (token, endpoint)))
                .SelectMany(pair =>
                    WebClient
                        .Get(pair.endpoint, false, pair.token)
                        .Select(r => r.json)
                        .Select(ToUserModel)
                        .ObserveOnMainThread());
        }

        public IObservable<Unit> BlockUser(UserData userData)
        {
            var blockedUser = new JObject { ["user_blocked_id"] = userData.userID };
            var json = new JObject { ["blocked_user"] = blockedUser };

            gameData.GetUserInformation().AddBlockedPlayer(userData.userID);
            CurrentRoom.Instance.AvatarsManager.RemoveAvatar(userData.firebaseUid);
            return GetEndpoint(EndPointComplement.BlockUser, GetPersonalID())
                .SelectMany(
                    endpoint =>
                        headersBuilder.BearerToken.Select(token => (token, enpoint: endpoint)))
                .SelectMany(
                    pair =>
                        WebClient.Post(pair.enpoint, json, true, pair.token))
                .AsUnitObservable();


        }

        UserData ToUserModel(JObject jObject)
        {
            var userJson = jObject["data"];

            var firebaseUid = userJson["firebase_uid"].Value<string>();

            var userID = userJson["id"].Value<int>();

            var gold = userJson["gold"].Value<int>();

            var realName = userJson["first_name"].Value<string>() + " " + userJson["last_name"].Value<string>();

            var gems = userJson["gems"].Value<int>();

            var username = userJson["username"].Value<string>();

            var email = userJson["email"].Value<string>();

            var friendCount = userJson["friend_count"].Value<int>();

            var friendRequestsCount = userJson["friend_request_count"].Value<int>();

            var roomType = userJson["room_type"].Value<string>();

            if (string.IsNullOrEmpty(roomType))
            {
                roomType = "";
            }

            return new UserData(firebaseUid, userID, gems, gold, realName, username, friendCount, friendRequestsCount,
                roomType);
        }


        List<FriendData> ToFriendsListModel(JObject jObject)
        {
            return jObject["data"].Values<JObject>().Select(friendsJson =>
            {
                var fJson = friendsJson["friend"];

                var roomType = friendsJson["room_type"].Value<string>();

                if (string.IsNullOrEmpty(roomType))
                {
                    roomType = "";
                }

                var roomInstanceId = friendsJson["room_instance_id"].Value<string>();

                if (string.IsNullOrEmpty(roomInstanceId))
                {
                    roomInstanceId = "";
                }

                // var roomDisplayName = friendsJson["room_display_name"].Value<string>();
                //
                // if (string.IsNullOrEmpty(roomDisplayName))
                // {
                //     roomDisplayName = "";
                // }


                var roomNamePrefab = friendsJson["room_name_prefab"].Value<string>();

                if (string.IsNullOrEmpty(roomNamePrefab))
                {
                    roomNamePrefab = "";
                }

                AvatarCustomizationData avatarCustomizationData = new AvatarCustomizationData();
                avatarCustomizationData.SetDataFriend(friendsJson["friend"].Value<JObject>());

                var realName = fJson["first_name"].Value<string>() + " " + fJson["last_name"].Value<string>();

                var username = fJson["username"].Value<string>();

                var email = fJson["email"].Value<string>();

                var userID = fJson["id"].Value<int>();

                var fireBaseUid = fJson["firebase_uid"].Value<string>();

                var friendCount = fJson["friend_count"].Value<int>();

                var gold = fJson["gold"].Value<int>();

                var gems = fJson["gems"].Value<int>();

                var friendRequestsCount = fJson["friend_request_count"].Value<int>();

                //var connectionStatus = fJson["connection_status"].Value<bool>();

                return new FriendData(userID, fireBaseUid, username, realName, roomType, friendCount, gems, gold,
                    friendRequestsCount, roomInstanceId, roomNamePrefab, avatarCustomizationData);
            }).ToList();
        }


        private FriendData FromRelationshipToFriendsModel(JObject jObject)
        {
            var data = jObject["data"];

            var friendJ = data["friend"].Value<JObject>();

            var roomType = friendJ["room_type"].Value<string>();

            if (string.IsNullOrEmpty(roomType))
            {
                roomType = "";
            }

            var roomInstanceId = friendJ["room_instance_id"].Value<string>();

            if (string.IsNullOrEmpty(roomInstanceId))
            {
                roomInstanceId = "";
            }

            // var roomDisplayName = friendJ["room_display_name"].Value<string>();
            //
            // if (string.IsNullOrEmpty(roomDisplayName))
            // {
            //     roomDisplayName = "";
            // }

            var roomNamePrefab = friendJ["room_name_prefab"].Value<string>();

            if (string.IsNullOrEmpty(roomNamePrefab))
            {
                roomNamePrefab = "";
            }

            AvatarCustomizationData avatarCustomizationData = new AvatarCustomizationData();
            avatarCustomizationData.SetDataFriend(friendJ);

            var friend = friendJ["friend"];

            var realName = friend["first_name"].Value<string>() + " " + friend["last_name"].Value<string>();

            var username = friend["username"].Value<string>();

            var gold = friend["gold"].Value<int>();

            var gems = friend["gems"].Value<int>();

            var email = friend["email"].Value<string>();

            var fireBaseUid = friend["firebase_uid"].Value<string>();

            var userID = friend["id"].Value<int>();

            var friendCount = friend["friend_count"].Value<int>();

            var friendRequestsCount = friend["friend_request_count"].Value<int>();

            //var connectionStatus = fJson["connection_status"].Value<bool>();

            return new FriendData(userID, fireBaseUid, username, realName, roomType, friendCount, gems, gold,
                friendRequestsCount, roomInstanceId, roomNamePrefab, avatarCustomizationData);
        }

        private FriendData ToFriendsModel(JObject jObject)
        {
            var data = jObject["data"];

            var friend = data["friend"].Value<JObject>();

            var roomType = data["room_type"].Value<string>();

            if (string.IsNullOrEmpty(roomType))
            {
                roomType = "";
            }

            var roomInstanceId = data["room_instance_id"].Value<string>();

            if (string.IsNullOrEmpty(roomInstanceId))
            {
                roomInstanceId = "";
            }

            // var roomDisplayName = data["room_display_name"].Value<string>();
            //
            // if (string.IsNullOrEmpty(roomDisplayName))
            // {
            //     roomDisplayName = "";
            // }

            var roomNamePrefab = data["room_name_prefab"].Value<string>();

            if (string.IsNullOrEmpty(roomNamePrefab))
            {
                roomNamePrefab = "";
            }

            AvatarCustomizationData avatarCustomizationData = new AvatarCustomizationData();
            avatarCustomizationData.SetDataFriend(friend);

            var realName = friend["first_name"].Value<string>() + " " + friend["last_name"].Value<string>();

            var username = friend["username"].Value<string>();

            var gold = friend["gold"].Value<int>();

            var gems = friend["gems"].Value<int>();

            var email = friend["email"].Value<string>();

            var fireBaseUid = friend["firebase_uid"].Value<string>();

            var userID = friend["id"].Value<int>();

            var friendCount = friend["friend_count"].Value<int>();

            var friendRequestsCount = friend["friend_request_count"].Value<int>();

            //var connectionStatus = fJson["connection_status"].Value<bool>();

            return new FriendData(userID, fireBaseUid, username, realName, roomType, friendCount, gems, gold,
                friendRequestsCount, roomInstanceId, roomNamePrefab, avatarCustomizationData);
        }

        List<FriendRequestData> ToFriendRequestsListModel(JObject jObject)
        {
            return jObject["data"].Values<JObject>().Select(friendRequest =>
            {
                var friendRequestID = friendRequest["id"].Value<int>();

                var requesterUser = friendRequest["requester_user"];

                AvatarCustomizationData avatarCustomizationData = new AvatarCustomizationData();
                avatarCustomizationData.SetDataFriend(friendRequest["requester_user"].Value<JObject>());

                var requesterUserID = requesterUser["id"].Value<int>();

                var requestStatusData = friendRequest["status"].Value<string>();

                RequestStatusOptions requestStatus = RequestStatusOptions.Pending;

                if (requestStatusData.Equals(RequestStatusOptions.Accepted.ToString().ToLower()))
                {
                    requestStatus = RequestStatusOptions.Accepted;
                }
                else if (requestStatusData.Equals(RequestStatusOptions.Rejected.ToString().ToLower()))
                {
                    requestStatus = RequestStatusOptions.Rejected;
                }

                var username = requesterUser["username"].Value<string>();

                var realName = requesterUser["first_name"].Value<string>() + " " +
                               requesterUser["last_name"].Value<string>();

                var email = requesterUser["email"].Value<string>();

                var firebaseUid = requesterUser["firebase_uid"].Value<string>();

                var friendCount = requesterUser["friend_count"].Value<int>();

                return new FriendRequestData(friendRequestID, username, realName, requesterUserID, firebaseUid,
                    requestStatus, friendCount, avatarCustomizationData);
            }).ToList();
        }

        FriendRequestData ToFriendRequestModel(JObject friendRequest)
        {
            var friendRequestID = friendRequest["id"].Value<int>();

            var requesterUser = friendRequest["requester_user"];

            AvatarCustomizationData avatarCustomizationData = new AvatarCustomizationData();
            avatarCustomizationData.SetDataFriend(requesterUser.Value<JObject>());

            var requesterUserID = requesterUser["id"].Value<int>();

            var requestStatusData = friendRequest["status"].Value<string>();

            RequestStatusOptions requestStatus = RequestStatusOptions.Pending;

            if (requestStatusData.Equals(RequestStatusOptions.Accepted.ToString().ToLower()))
            {
                requestStatus = RequestStatusOptions.Accepted;
            }
            else if (requestStatusData.Equals(RequestStatusOptions.Rejected.ToString().ToLower()))
            {
                requestStatus = RequestStatusOptions.Rejected;
            }

            var username = requesterUser["username"].Value<string>();

            var realName = requesterUser["first_name"].Value<string>() + " " +
                           requesterUser["last_name"].Value<string>();

            var email = requesterUser["email"].Value<string>();

            var firebaseUid = requesterUser["firebase_uid"].Value<string>();

            var friendCount = requesterUser["friend_count"].Value<int>();

            return new FriendRequestData(friendRequestID, username, realName, requesterUserID, firebaseUid,
                requestStatus, friendCount, avatarCustomizationData);
        }

        UsersRelationshipData ToUserRelationshipModel(JObject jObject)
        {
            UserRelationship relationship = UserRelationship.Strangers;

            var userJson = jObject["data"];
            var friend = userJson["friend"];
            var friendRequest = userJson["friend_request"];

            //If not null users are friends
            if (friend != null && friend.Value<JObject>() != null)
            {
                relationship = UserRelationship.Friends;

                var friendData = FromRelationshipToFriendsModel(jObject);
                return new UsersRelationshipData(friendData, relationship);
            }

            //If exist means one of the user received a request at some moment
            if (friendRequest != null && friendRequest.Value<JObject>() != null)
            {
                //Check if the request is still unanswered  
                var status = friendRequest["status"].Value<string>();

                if (status.Equals("pending"))
                {
                    //Check if user received or sent a request
                    var requesterID = friendRequest["requester_user"]["id"].Value<int>().ToString();

                    if (requesterID.Equals(GetPersonalID()))
                    {
                        relationship = UserRelationship.RequestSent;
                        return new UsersRelationshipData(ToFriendRequestModel(friendRequest.Value<JObject>()),
                            relationship);
                    }

                    relationship = UserRelationship.RequestReceived;
                    return new UsersRelationshipData(ToFriendRequestModel(friendRequest.Value<JObject>()),
                        relationship);
                }
            }

            return new UsersRelationshipData(relationship);
        }

        //Params "param" comes in order according to the endpoint url
        private IObservable<string> GetEndpoint(EndPointComplement endPointComplement, params string[] param)
        {
            var firebaseUidObservable = getFirebaseUid.Execute();

            return endPointComplement switch
            {
                EndPointComplement.User => firebaseUidObservable.Select(uid =>
                    $"{Constants.UsersEndpoint}/{param[0]}"),

                EndPointComplement.PersonalData => firebaseUidObservable.Select(uid =>
                    $"{Constants.UsersEndpoint}/{uid}"),

                EndPointComplement.Relationship => firebaseUidObservable.Select(uid =>
                    $"{Constants.UserRelationshipEndpoint}/{GetPersonalID()}/{param[0]}"),

                EndPointComplement.AddFriend => firebaseUidObservable.Select(uid =>
                    $"{Constants.UsersEndpoint}/{GetPersonalID()}/friends/"),

                EndPointComplement.FriendsList => firebaseUidObservable.Select(uid =>
                    $"{Constants.UsersEndpoint}/{GetPersonalID()}/friends/"),

                EndPointComplement.Friend => firebaseUidObservable.Select(uid =>
                    $"{Constants.UsersEndpoint}/{GetPersonalID()}/friends/{param[0]}"),

                EndPointComplement.FriendRequestsList => firebaseUidObservable.Select(uid =>
                    $"{Constants.UsersEndpoint}/{GetPersonalID()}/friend-requests/"),

                EndPointComplement.AddFriendRequest => firebaseUidObservable.Select(uid =>
                    $"{Constants.UsersEndpoint}/{param[0]}/friend-requests/"),

                EndPointComplement.SimulateFriendRequest => firebaseUidObservable.Select(uid =>
                    $"{Constants.UsersEndpoint}/{GetPersonalID()}/friend-requests/"),

                EndPointComplement.UpdateReceivedFriendRequest => firebaseUidObservable.Select(uid =>
                    $"{Constants.UsersEndpoint}/{GetPersonalID()}/friend-requests/{param[0]}"),

                EndPointComplement.UpdateSentFriendRequest => firebaseUidObservable.Select(uid =>
                    $"{Constants.UsersEndpoint}/{param[0]}/friend-requests/{param[1]}"),

                EndPointComplement.DeleteFriend => firebaseUidObservable.Select(uid =>
                    $"{Constants.UsersEndpoint}/{GetPersonalID()}/friends/{param[0]}"),

                EndPointComplement.BlockUser => firebaseUidObservable.Select(uid =>
                    $"{Constants.UsersEndpoint}/{GetPersonalID()}/blocked-user"),

                EndPointComplement.ReportUser => firebaseUidObservable.Select(uid =>
                    $"{Constants.ReportUserEndPoint}"),

                _ => firebaseUidObservable.Select(uid => $"{Constants.UsersEndpoint}/")
            };
        }

        enum EndPointComplement
        {
            PersonalData,
            AddFriend,
            FriendsList,
            FriendRequestsList,
            AddFriendRequest,
            DeleteFriend,
            UpdateReceivedFriendRequest,
            UpdateSentFriendRequest,
            SimulateFriendRequest,
            ReportUser,
            Relationship,
            Friend,
            BlockUser,
            User,
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Functional.Maybe;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayerRoom.Core.Domain;
using PlayerRoom.Core.Services;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Web;
using WebClientTools.Core.Services;

namespace PlayerRoom.Infrastructure
{
    public class HttpRoomsService : IRoomsService
    {
        const int MAXInstancesPerRoomType = 10;

        readonly IWebHeadersBuilder webHeadersBuilder;

        public HttpRoomsService(IWebHeadersBuilder webHeadersBuilder)
        {
            this.webHeadersBuilder = webHeadersBuilder;
        }


        public IObservable<List<RoomInfo>> GetRoomsIDs()
        {
            return GetRoomsIDsAsync().ToObservable().ObserveOnMainThread();
        }

        public IObservable<List<RoomInfo>> GetRoomInstances(string roomId)
        {
            return GetInstanceInfos(roomId).ToObservable().ObserveOnMainThread();
        }


        async Task<List<RoomInfo>> GetRoomsIDsAsync()
        {
            var bearerTokenHeader = await webHeadersBuilder.BearerToken.ObserveOnMainThread();

            var response = await WebClient.Get(Constants.RoomTypesEndpoint, headers: bearerTokenHeader).ObserveOnMainThread();

            var responseJSON = response.json;

            Debug.Log(responseJSON);

            var dto = JsonUtility.FromJson<RoomsDto.Types>(responseJSON.ToString());

            var types = dto.data
                .Where(data => data.enabled)
                .Select(data => new RoomInfo
                {
                    AreaId = data.room_name,
                    RoomName = data.room_name,
                    PlayersOnArea = data.total_members
                }).ToList();

            return types;
        }

        async Task<List<RoomInfo>> GetInstanceInfos(string roomName)
        {
            var bearerTokenHeader = await webHeadersBuilder.BearerToken.ObserveOnMainThread();

            var instancesResponse = await WebClient
                .Get(GetRoomInstancesEndpoint(roomName), headers: bearerTokenHeader)
                .ObserveOnMainThread();
            var instancesJson = instancesResponse.json;

            var instancesDTO = JsonUtility.FromJson<RoomsDto.Rooms>(instancesJson.ToString());

            var newInfos = instancesDTO.data.Select(token =>
            {
                var instanceRoomInfo = new RoomInfo
                {
                    AreaId = roomName,
                    RoomName = roomName,
                    InstanceId = token.id,
                    PlayersOnRoom = token.members_count
                };
                return instanceRoomInfo;
            });
            return newInfos.ToList();
        }

        string GetRoomInstancesEndpoint(string roomName) => Constants.ChatRoomsEndpoint + "?room_name=" + roomName;
    }

    internal class RoomsDto
    {
        [Serializable]
        public struct RoomType
        {
            public bool enabled;
            public string room_name;
            public int total_members;
        }

        [Serializable]
        public struct Types
        {
            public List<RoomType> data;
        }

        [Serializable]
        public struct RoomData
        {
            public string id;

            [JsonProperty("members_count")] [FormerlySerializedAs("members_count")]
            public int members_count;
        }

        [Serializable]
        public struct Rooms
        {
            public List<RoomData> data;
        }
    }
}

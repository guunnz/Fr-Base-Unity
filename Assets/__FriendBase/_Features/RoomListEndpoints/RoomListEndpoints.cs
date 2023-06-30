using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Data.Catalog;
using AuthFlow.Firebase.Core.Actions;
using WebClientTools.Core.Services;
using UniRx;
using Newtonsoft.Json.Linq;
using Web;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Bag;
using Data.Catalog.Items;
using Architecture.Injector.Core;
using Data.Users;
using DebugConsole;
using Data.Rooms;

public class RoomListEndpoints : IRoomListEndpoints
{
    readonly GetFirebaseUid getFirebaseUid;
    readonly IWebHeadersBuilder headersBuilder;
    readonly IGameData gameData;

    private IDebugConsole debugConsole;

    public RoomListEndpoints(GetFirebaseUid getFirebaseUid, IWebHeadersBuilder headersBuilder, IGameData gameData)
    {
        debugConsole = Injection.Get<IDebugConsole>();
        this.getFirebaseUid = getFirebaseUid;
        this.headersBuilder = headersBuilder;
        this.gameData = gameData;
    }

    public IObservable<List<RoomInformation>> GetPublicRoomsList() => GetPublicRoomsListAsync().ToObservable().ObserveOnMainThread();
    async Task<List<RoomInformation>> GetPublicRoomsListAsync()
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;

        var endpoint = $"{Constants.ApiRoot}/rooms";
        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        return ToPublicRoomsList(response.json);
    }

    List<RoomInformation> ToPublicRoomsList(JObject jObject)
    {
        List<RoomInformation> listRoomInformation = new List<RoomInformation>();

        foreach (JObject roomData in jObject["data"])
        {
            try
            {
                RoomInformation roomInformation = new RoomInformation();

                roomInformation.RoomName = roomData["display_name"].Value<string>();
                roomInformation.AmountUsers = roomData["total_members"].Value<int>();
                roomInformation.RoomId = roomData["id"].Value<int>();
                roomInformation.NamePrefab = roomData["name_prefab"].Value<string>();
                roomInformation.IsEnable = roomData["enabled"].Value<bool>();
                roomInformation.PlayerLimit = roomData["player_limit"].Value<int>();
                //roomInformation.RoomRank = roomData["room_rank"].Value<int>();
                roomInformation.RoomType = roomData["type"].Value<string>();

                listRoomInformation.Add(roomInformation);
            }
            catch (Exception e)
            {
                debugConsole.ErrorLog("RoomListEndpoints:ToPublicRoomsList", "Exception", "Invalid Json Data");
            }
        }
        return listRoomInformation;
    }

    public IObservable<List<RoomInformation>> GetPublicRoomsListInside(int idRoom) => GetPublicRoomsListInsideAsync(idRoom).ToObservable().ObserveOnMainThread();
    async Task<List<RoomInformation>> GetPublicRoomsListInsideAsync(int idRoom)
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;

        var endpoint = $"{Constants.ApiRoot}/rooms/{idRoom}/room-instances";
        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        Debug.Log("INSIDE====" + response.json);
        return ToPublicRoomsListInside(response.json);
    }

    List<RoomInformation> ToPublicRoomsListInside(JObject jObject)
    {
        List<RoomInformation> listRoomInformation = new List<RoomInformation>();

        foreach (JObject roomData in jObject["data"])
        {
            try
            {
                RoomInformation roomInformation = new RoomInformation();

                roomInformation.RoomName = roomData["display_name"].Value<string>();
                roomInformation.RoomIdInstance = roomData["id"].Value<string>();
                roomInformation.AmountUsers = roomData["members_count"].Value<int>();
                roomInformation.RoomId = roomData["room_id"].Value<int>();

                roomInformation.NamePrefab = roomData["name_prefab"].Value<string>();
                //roomInformation.IsEnable = roomData["enabled"].Value<bool>();
                roomInformation.PlayerLimit = roomData["player_limit"].Value<int>();
                roomInformation.RoomRank = roomData["instance_rank"].Value<int>();
                roomInformation.RoomType = roomData["type"].Value<string>();

                roomInformation.RoomType = RoomType.PUBLIC;

                listRoomInformation.Add(roomInformation);
            }
            catch (Exception e)
            {
                debugConsole.ErrorLog("RoomListEndpoints:ToPublicRoomsList", "Exception", "Invalid Json Data");
            }
        }
        return listRoomInformation;
    }

    public IObservable<RoomInformation> GetMyIdHouse() => GetMyIdHouseAsync().ToObservable().ObserveOnMainThread();
    async Task<RoomInformation> GetMyIdHouseAsync()
    {
        RoomInformation roomInformation = new RoomInformation();

        var bearerTokenHeader = await headersBuilder.BearerToken;
        var userId = await getFirebaseUid.Execute();
        int idUser = gameData.GetUserInformation().UserId;

        var endpoint = $"{Constants.ApiRoot}/users/{idUser}/room";
        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        JObject jsonRoomInformation = response.json;
        JObject roomData = jsonRoomInformation["data"].Value<JObject>();

        roomInformation.RoomName = roomData["display_name"].Value<string>();
        roomInformation.RoomId = roomData["id"].Value<int>();
        roomInformation.RoomIdInstance = roomData["instance_id"].Value<string>();
        roomInformation.NamePrefab = roomData["name_prefab"].Value<string>();
        roomInformation.IdUser = roomData["owner_id"].Value<int>();
        roomInformation.RoomType = RoomType.PRIVATE;

        return roomInformation;
    }

    public IObservable<RoomInformation> ChangeTheme(int idTheme) => ChangeThemeAsync(idTheme).ToObservable().ObserveOnMainThread();
    async Task<RoomInformation> ChangeThemeAsync(int idTheme)
    {
        RoomInformation roomInformation = new RoomInformation();

        var bearerTokenHeader = await headersBuilder.BearerToken;

        JObject jsonPurchases = new JObject();
        jsonPurchases["inventory_item_id"] = idTheme;

        var endpoint = $"{Constants.ApiRoot}/room";
        var response = await WebClient.Post(endpoint, jsonPurchases, true, bearerTokenHeader);

        JObject jsonRoomInformation = response.json;
        JObject roomData = jsonRoomInformation["data"].Value<JObject>();

        roomInformation.RoomName = roomData["display_name"].Value<string>();
        roomInformation.RoomId = roomData["id"].Value<int>();
        roomInformation.RoomIdInstance = roomData["instance_id"].Value<string>();
        roomInformation.NamePrefab = roomData["name_prefab"].Value<string>();
        roomInformation.IdUser = roomData["owner_id"].Value<int>();
        roomInformation.RoomType = RoomType.PRIVATE;

        return roomInformation;
    }

    public IObservable<RoomInformation> GetFreePublicRoomByType(int idRoom) => GetFreePublicRoomByTypeAsync(idRoom).ToObservable().ObserveOnMainThread();
    async Task<RoomInformation> GetFreePublicRoomByTypeAsync(int idRoom)
    {
        RoomInformation roomInformation = new RoomInformation();

        var bearerTokenHeader = await headersBuilder.BearerToken;
        var userId = await getFirebaseUid.Execute();
        int idUser = gameData.GetUserInformation().UserId;

        JObject jsonRoot = new JObject
        {
            ["user_id"] = idUser,
            ["room_id"] = idRoom
        };

        var endpoint = $"{Constants.ApiRoot}/get-room-instance";
        var response = await WebClient.Post(endpoint, jsonRoot, true, bearerTokenHeader);

        JObject jsonRoomInformation = response.json;

        roomInformation.RoomName = "";
        roomInformation.RoomId = idRoom;
        roomInformation.RoomIdInstance = jsonRoomInformation["instance_id"].Value<string>();
        roomInformation.NamePrefab = jsonRoomInformation["name_prefab"].Value<string>();
        roomInformation.IdUser = 0;
        roomInformation.RoomType = RoomType.PUBLIC;

        return roomInformation;
    }
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------
    //------------------------   E V E N T S  ----------------------------
    //--------------------------------------------------------------------
    //--------------------------------------------------------------------

    public IObservable<RoomInformation> CreateEvent(int eventType) => CreateEventAsync(eventType).ToObservable().ObserveOnMainThread();
    async Task<RoomInformation> CreateEventAsync(int eventType)
    {
        RoomInformation roomInformation = new RoomInformation();

        var bearerTokenHeader = await headersBuilder.BearerToken;

        JObject jsonParameters = new JObject();
        jsonParameters["type"] = eventType;

        JObject jsonRoot = new JObject();
        jsonRoot["event"] = jsonParameters;

        int idUser = gameData.GetUserInformation().UserId;

        var endpoint = $"{Constants.ApiRoot}/users/{idUser}/start-event";
        var response = await WebClient.Post(endpoint, jsonRoot, true, bearerTokenHeader);

        JObject jsonRoomInformation = response.json;

        try
        {
            JObject roomData = jsonRoomInformation["data"].Value<JObject>();

            roomInformation.RoomName = roomData["display_name"].Value<string>();
            roomInformation.RoomId = roomData["id"].Value<int>();
            roomInformation.RoomIdInstance = roomData["instance_id"].Value<string>();
            roomInformation.NamePrefab = roomData["name_prefab"].Value<string>();
            roomInformation.IdUser = roomData["owner_id"].Value<int>();
            roomInformation.RoomType = RoomType.PRIVATE;
        }
        catch(Exception e)
        {
            return null;
        }

        return roomInformation;
    }

    public IObservable<List<RoomInformation>> GetEventList(bool addMyEventRow) => GetEventListAsync(addMyEventRow).ToObservable().ObserveOnMainThread();
    async Task<List<RoomInformation>> GetEventListAsync(bool addMyEventRow)
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;

        var endpoint = $"{Constants.ApiRoot}/events";
        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        Debug.Log("**GetEventList: " + response.json);

        return ToEventList(response.json, addMyEventRow);
    }

    List<RoomInformation> ToEventList(JObject jObject, bool addMyEventRow)
    {
        List<RoomInformation> listRoomInformation = new List<RoomInformation>();
        int idUser = gameData.GetUserInformation().UserId;

        RoomInformation myEventInformation = new RoomInformation(roomIdInstance: "1", roomName: "Mini", amountUsers: 10, roomId: 1, namePrefab: "Rio", isEnable: true, playerLimit: 10, roomRank: 10, roomType: "private", idUser: 400, hostUserName: "Matutes", eventState: RoomInformation.EVENT_STATE.MY_EVENT_CARD_TO_HOST);
        if (addMyEventRow)
        {
            //Force first position as my event
            listRoomInformation.Add(myEventInformation);
        }

        foreach (JObject roomData in jObject["data"])
        {
            try
            {
                RoomInformation roomInformation = new RoomInformation();

                roomInformation.RoomName = roomData["display_name"].Value<string>();
                roomInformation.RoomIdInstance = roomData["instance_id"].Value<string>();
                roomInformation.AmountUsers = roomData["total_members"].Value<int>();
                roomInformation.NamePrefab = roomData["name_prefab"].Value<string>();
                roomInformation.RoomId = roomData["id"].Value<int>();
                roomInformation.PlayerLimit = roomData["player_limit"].Value<int>();
                roomInformation.EventType = roomData["event_type"].Value<int?>() ?? 0;
                roomInformation.IdUser = roomData["owner_id"].Value<int?>() ?? 0;
                //roomInformation.IsEnable = roomData["enabled"].Value<bool>();
                roomInformation.HostUserName = roomData["owner"].Value<string>();
                roomInformation.EventState = RoomInformation.EVENT_STATE.EVENT;
                roomInformation.RoomType = roomData["type"].Value<string>();

                if (idUser == roomInformation.IdUser && addMyEventRow)
                {
                    //Change my event information so we can END 
                    myEventInformation.EventState = RoomInformation.EVENT_STATE.MY_EVENT_CARD_TO_END;
                    //listRoomInformation.Add(roomInformation);
                }
                else
                {
                    listRoomInformation.Add(roomInformation);
                }
            }
            catch (Exception e)
            {
                debugConsole.ErrorLog("GetEventList:ToEventList", "Exception", "Invalid Json Data");
            }
        }
        return listRoomInformation;
    }

    public IObservable<RoomInformation> FinishEvent() => FinishEventAsync().ToObservable().ObserveOnMainThread();
    async Task<RoomInformation> FinishEventAsync()
    {
        RoomInformation roomInformation = new RoomInformation();

        var bearerTokenHeader = await headersBuilder.BearerToken;

        JObject jsonRoot = new JObject();

        int idUser = gameData.GetUserInformation().UserId;

        var endpoint = $"{Constants.ApiRoot}/users/{idUser}/finish-event";
        var response = await WebClient.Post(endpoint, jsonRoot, true, bearerTokenHeader);

        JObject jsonRoomInformation = response.json;

        try
        {
            JObject roomData = jsonRoomInformation["data"].Value<JObject>();

            roomInformation.RoomName = roomData["display_name"].Value<string>();
            roomInformation.RoomId = roomData["id"].Value<int>();
            roomInformation.RoomIdInstance = roomData["instance_id"].Value<string>();
            roomInformation.NamePrefab = roomData["name_prefab"].Value<string>();
            roomInformation.IdUser = roomData["owner_id"].Value<int>();
            roomInformation.RoomType = RoomType.PRIVATE;
        }
        catch (Exception e)
        {
            return null;
        }

        return roomInformation;
    }
}

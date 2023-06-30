using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using AuthFlow.EndAuth.Repo;
using Data;
using DebugConsole;
using Firebase.Auth;
using UniRx;
using UnityEngine;
using WebClientTools.Core.Services;
using Web;
using Newtonsoft.Json.Linq;
using Data.Users;
using Data.Catalog;
using System.Linq;
using Data.Rooms;

public class NauthFlowEndpoints : IAuthFlowEndpoint
{
    private IGameData gameData;
    private IDebugConsole debugConsole;
    private IWebHeadersBuilder headersBuilder;
    private IAvatarEndpoints avatarEndpoints;
    private IRoomListEndpoints roomListEndpoints;
    
    public NauthFlowEndpoints()
    {
        gameData = Injection.Get<IGameData>();
        debugConsole = Injection.Get<IDebugConsole>();
        headersBuilder = Injection.Get<IWebHeadersBuilder>();
        avatarEndpoints = Injection.Get<IAvatarEndpoints>();
        roomListEndpoints = Injection.Get<IRoomListEndpoints>();
    }

    public enum UserNameResult { ALLOWED, EXISTS, NOT_ALLOW, ERROR }
    public async Task<UserNameResult> IsAvailableUserName(string userName)
    {
        try
        {
            var bearerTokenHeader = await headersBuilder.BearerToken;

            var json = new JObject
            {
                ["username"] = userName
            }.ToString();

            var response = await WebClient.Request(
                WebMethod.Post,
                Constants.UsernameValidationEndpoint,
                json,
                true,
                bearerTokenHeader
            );

            var jsonResult = response.json;
            UserNameResult result = UserNameResult.ALLOWED;

            bool flagExist = jsonResult["exists"].Value<bool>();
            if (flagExist)
            {
                result = UserNameResult.EXISTS;
            }
            bool allowed = jsonResult["allowed"].Value<bool>();
            if (!allowed)
            {
                result = UserNameResult.NOT_ALLOW;
            }
            return result;
        }
        catch (Exception e)
        {
            return UserNameResult.ERROR;
        }
    }

    public async Task<string> IsUserLoggedIn()
    {
        string Result = null;

        try
        {
            var firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
            if (firebaseUser != null)
            {
                ILocalUserInfo userInfo = Injection.Get<ILocalUserInfo>();
                string loginToken = userInfo["firebase-login-token"];

                if (!string.IsNullOrEmpty(loginToken))
                {
                    await FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync();
                    firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
                    if (firebaseUser != null)
                    {
                        loginToken = await firebaseUser.TokenAsync(true);
                        userInfo["firebase-login-token"] = loginToken;
                        Result = loginToken;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception IsUserLoggedIn: " + e + " Message:" + e.Message);
            return null;
        }

        return Result;
    }

    public async Task<bool> GetInitialAvatarEndpoints()
    {
        bool Result = false;

        try
        {
            //Get User Information
            UserInformation usInformation = await avatarEndpoints.GetUserInformation();
            gameData.GetUserInformation().Initialize(usInformation);

            //Get ALL Catalog of items
            List<AvatarGenericCatalogItem> listItems = await avatarEndpoints.GetAvatarCatalogItemsList();

            //Initialize Catalof of items
            gameData.InitializeCatalogs(listItems.ToList<GenericCatalogItem>());

            //After we get the items we can ask for my avatar skin
            var json = await avatarEndpoints.GetAvatarSkin();

            //string jsonString = json.ToString(Formatting.Indented);
            //Debug.Log("GetAvatarSkin <color=green>" + jsonString + "</color>");
            AvatarCustomizationData avatarData = new AvatarCustomizationData();
            avatarData.SetDataFromUserSkin(json);
            gameData.GetUserInformation().GetAvatarCustomizationData().SetData(avatarData);

            //Get User Inventory
            var listBagItems = await avatarEndpoints.GetPlayerInventory();
            gameData.AddItemsToBag(listBagItems);

            //Get MY Room information
            RoomInformation myRoomInformation = await roomListEndpoints.GetMyIdHouse();
            gameData.SetMyHouseInformation(myRoomInformation);
            gameData.SetRoomInformation(myRoomInformation);

            //Get blocked users list
            var blockedUsers = await avatarEndpoints.GetAvatarBlockedList();
            JArray blockedUsersObject = blockedUsers["data"].Value<JArray>();

            List<int> blockedUsersIdList = new List<int>();

            foreach (var token in blockedUsersObject.Children())
            {
                blockedUsersIdList.Add(token["user_blocked"]["id"].Value<int>());
            }
            gameData.GetUserInformation().SetBlockedPlayers(blockedUsersIdList);

            ////Get blocked users list
            //var blockerUsers = await Injection.Get<IAvatarEndpoints>().GetAvatarBlockerList();
            //JArray blockerUsersObject = blockerUsers["data"].Value<JArray>();

            //foreach (var token in blockerUsersObject.Children())
            //{
            //    blockedUsersIdList.Add(token["user_blocked"]["id"].Value<int>());
            //}

            //User Account Status
            UserAccountStatus userStatus = await avatarEndpoints.GetUserStatus();
            gameData.GetUserInformation().UserStatus = userStatus;

            //TRACK VER
            await Injection.Get<IAvatarEndpoints>().TrackUserSession();

            Result = true;
        }
        catch (Exception e)
        {
            Debug.LogError("Something went wrong during GetInitialAvatarEndpoints()");
        }
        return Result;
    }

    public async Task<bool> SendEmailResetPassword(string email)
    {
        try
        {
            JObject eventData = new JObject
            {
                ["email"] = email,
            };

            var endpoint = Constants.ResetPasswordEndpoint;

            var result = await WebClient.Post(endpoint, eventData, false, ("Content-Type", "application/json"));

            return true;
        }
        catch (Exception e)
        {
            //If the mail does not exist
            return false;
        }
    }

    public async Task<UserInformation> CreatePhoenixUser(string mail, string userId, string loginToken, ProviderType providerType, string username, string birthday)
    {
        try
        {
            var json = new JObject
            {
                ["user"] = new JObject
                {
                    ["email"] = mail,
                    ["firebase_uid"] = userId,
                    ["registration_type"] = providerType.ToString().ToLower(),
                    ["username"] = username,
                    ["birthday"] = birthday
                }
            }.ToString();

            var response = await WebClient.Request(
                WebMethod.Post,
                Constants.CreateEmailURL,
                json,
                false,
                ("Content-Type", "application/json"),
                ("Authorization", "Bearer " + loginToken)
            );

            UserInformation userInformation = avatarEndpoints.ToUserInformation(response.json);
            return userInformation;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<UserInformation> CreatePhoenixGuestUser(string userId, string loginToken)
    {
        try
        {
            var json = new JObject
            {
                ["user"] = new JObject
                {
                    ["firebase_uid"] = userId
                }
            }.ToString();

            var response = await WebClient.Request(
                WebMethod.Post,
                Constants.GuestUser,
                json,
                false,
                ("Content-Type", "application/json"),
                ("Authorization", "Bearer " + loginToken)
            );

            UserInformation userInformation = avatarEndpoints.ToUserInformation(response.json);
            return userInformation;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<UserInformation> LinkPhoenixGuestUser(string mail, string userId, string loginToken, ProviderType providerType, string username, string birthday)
    {
        try
        {
            var json = new JObject
            {
                ["user"] = new JObject
                {
                    ["email"] = mail,
                    ["firebase_uid"] = userId,
                    ["registration_type"] = providerType.ToString().ToLower(),
                    ["username"] = username,
                    ["birthday"] = birthday,
                    ["terms_accepted"] = true
                }
            }.ToString();
           
            var response = await WebClient.Request(
                WebMethod.Post,
                Constants.GuestUser + "/" + userId + "/convert_to_registered_user",
                json,
                false,
                ("Content-Type", "application/json"),
                ("Authorization", "Bearer " + loginToken)
            );

            UserInformation userInformation = avatarEndpoints.ToUserInformation(response.json);
            return userInformation;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}

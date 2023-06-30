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

public class AvatarEndpoints : IAvatarEndpoints
{
    readonly GetFirebaseUid getFirebaseUid;
    readonly IWebHeadersBuilder headersBuilder;

    private IItemTypeUtils itemTypeUtils;
    private IGameData gameData;
    private IDebugConsole debugConsole;

    public AvatarEndpoints(GetFirebaseUid getFirebaseUid, IWebHeadersBuilder headersBuilder)
    {
        this.getFirebaseUid = getFirebaseUid;
        this.headersBuilder = headersBuilder;

        gameData = Injection.Get<IGameData>();
        debugConsole = Injection.Get<IDebugConsole>();
        itemTypeUtils = Injection.Get<IItemTypeUtils>();
    }

    public IObservable<List<GenericBagItem>> GetPlayerInventory() => GetPlayerInventoryAsync().ToObservable().ObserveOnMainThread();
    async Task<List<GenericBagItem>> GetPlayerInventoryAsync()
    {
        var userId = await getFirebaseUid.Execute();
        var bearerTokenHeader = await headersBuilder.BearerToken;

        var endpoint = $"{Constants.ApiRoot}/users/{userId}/player-inventory-items";

        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        return ToGenericBagItem(response.json);
    }

    async Task<List<GenericBagItem>> SetAvatarPetAsync()
    {
        var userId = await getFirebaseUid.Execute();
        var bearerTokenHeader = await headersBuilder.BearerToken;

        var endpoint = $"{Constants.ApiRoot}/user/{userId}/pet";

        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        return ToGenericBagItem(response.json);
    }

    async Task<JObject> SetAvatarPet(JObject jObject, string petId)
    {
        try
        {
            var userId = await getFirebaseUid.Execute();
            var bearerTokenHeader = await headersBuilder.BearerToken;
            var endpoint = $"{Constants.ApiRoot}/user/{userId}/{petId}";

            var response = await WebClient.Put(endpoint, jObject, true, bearerTokenHeader);

            return response.json;
        }
        catch (Exception e)
        {
            debugConsole.ErrorLog("AvatarEndpoints:SetAvatarSkinAsync", "Endpoint Failed", "");
        }
        return new JObject();
    }

    async Task<JObject> AddAvatarPets(JObject jObject, string petItemId)
    {
        try
        {
            var userId = await getFirebaseUid.Execute();
            var bearerTokenHeader = await headersBuilder.BearerToken;
            var endpoint = $"{Constants.ApiRoot}/user/{userId}/pet/{petItemId}";

            var response = await WebClient.Put(endpoint, jObject, true, bearerTokenHeader);

            return response.json;
        }
        catch (Exception e)
        {
            debugConsole.ErrorLog("AvatarEndpoints:SetAvatarSkinAsync", "Endpoint Failed", "");
        }
        return new JObject();
    }

    List<GenericBagItem> ToGenericBagItem(JObject jObject)
    {
        List<GenericBagItem> listItemBags = new List<GenericBagItem>();

        foreach (JObject itemData in jObject["data"])
        {
            try
            {
                int idInstance = itemData["id"].Value<int>();

                int count = itemData["count"].Value<int>();

                JObject itemJson = itemData["item"].Value<JObject>();

                int idItem = itemJson["id_in_game"].Value<int>();

                string sItemType = itemJson["type"].Value<string>();

                ItemType itemType = itemTypeUtils.GetItemTypeByName(sItemType);

                GenericCatalog catalog = gameData.GetCatalogByItemType(itemType);

                if (catalog != null)
                {
                    GenericCatalogItem objCat = catalog.GetItem(idItem);
                    if (objCat != null)
                    {
                        listItemBags.Add(new GenericBagItem(itemType, idInstance, count, objCat));
                    }
                    else
                    {
                        debugConsole.ErrorLog("AvatarEndpoints:ToGenericBagItem", "idItem Not Found", $"itemType:{itemType} idItem:{idItem}");
                    }
                }
            }
            catch (Exception e)
            {
                debugConsole.ErrorLog("AvatarEndpoints:ToGenericBagItem", "Exception", "Invalid Json Data");
            }
        }
        return listItemBags;
    }

    public IObservable<List<AvatarGenericCatalogItem>> GetAvatarCatalogItemsList() => GetAvatarCatalogItemsListAsync().ToObservable().ObserveOnMainThread();
    async Task<List<AvatarGenericCatalogItem>> GetAvatarCatalogItemsListAsync()
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = Constants.ItemsEndPoint;
        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        return ToAvatarGenericCatalogItem(response.json);
    }

    public IObservable<bool> SetPlayerCoins(int coins) => SetPlayerCoinsAsync(coins).ToObservable().ObserveOnMainThread();

    async Task<bool> SetPlayerCoinsAsync(int coins)
    {
        try
        {
            var userId = await getFirebaseUid.Execute();
            var bearerTokenHeader = await headersBuilder.BearerToken;
            var endpoint = Constants.UsersEndpoint + $"/{userId}/gold-earned";

            var eventData = new JObject
            {
                ["gold"] = coins,
            };
            var response = await WebClient.Post(endpoint, eventData, true, bearerTokenHeader);

            return true;
        }
        catch (Exception e)
        {
            debugConsole.ErrorLog("AvatarEndpoints:SetPlayerCoins", "Endpoint Failed", "");
        }
        return true;
    }

    List<AvatarGenericCatalogItem> ToAvatarGenericCatalogItem(JObject jObject)
    {
        List<AvatarGenericCatalogItem> listItems = new List<AvatarGenericCatalogItem>();

        foreach (JObject catalogItem in jObject["data"])
        {
            try
            {
                //Item
                int id = catalogItem["id"].Value<int>();
                int idItem = catalogItem["id_in_game"].Value<int>();
                string nameItem = catalogItem["name"].Value<string>();
                string namePrefab = catalogItem["name_prefab"].Value<string>();
                string sItemType = catalogItem["type"].Value<string>();
                ItemType itemType = itemTypeUtils.GetItemTypeByName(sItemType);
                string sLayers = catalogItem["layers"].Value<string>();
                string saleCurrency = catalogItem["sale_currency"].Value<string>();
                int[] layers = null;
                if (!string.IsNullOrEmpty(sLayers))
                {
                    layers = System.Array.ConvertAll(sLayers.Split(','), int.Parse);
                }

                string psbName = "";
                string PsbNameUI = "";
                string limitedEdition = LimitedEditionType.NONE;

                if (catalogItem["psb_name"] != null)
                {
                    psbName = catalogItem["psb_name"].Value<string>();
                }
                if (catalogItem["psb_name_ui"] != null)
                {
                    PsbNameUI = catalogItem["psb_name_ui"].Value<string>();
                }

                if (catalogItem["limited_edition"] != null && !string.IsNullOrEmpty(catalogItem["limited_edition"].Value<string>()))
                {
                    limitedEdition = catalogItem["limited_edition"].Value<string>();
                }

                string gender = catalogItem["gender"].Value<string>();

                int orderInCatalog = 0;
                bool activeInCatalog = catalogItem["active_in_catalog"].Value<bool>();

                CurrencyType currencyType = CurrencyType.GEM_PRICE;
                int gemsPrice = 10;
                int goldPrice = 10;

                goldPrice = catalogItem["gold"].Value<int>();
                gemsPrice = catalogItem["gems"].Value<int>();
                currencyType = saleCurrency == "gold" ? CurrencyType.GOLD_PRICE : CurrencyType.GEM_PRICE;

                if (itemType == ItemType.BODY)
                {
                    listItems.Add(new BodyCatalogItem(itemType, id, idItem, nameItem, namePrefab, orderInCatalog, activeInCatalog,
                        gemsPrice, goldPrice, currencyType, layers, gender.Equals("female")));
                }
                else
                {
                    listItems.Add(new AvatarGenericCatalogItem(itemType, id, idItem, nameItem, namePrefab, orderInCatalog,
                        activeInCatalog, gemsPrice, goldPrice, currencyType, layers, psbName, PsbNameUI, limitedEdition));
                }
            }
            catch (Exception e)
            {
                debugConsole.ErrorLog("AvatarEndpoints:ToAvatarGenericCatalogItem", "Exception", "Invalid Json Data");
            }
        }
        return listItems;
    }

    public IObservable<JObject> GetAvatarSkin() => GetAvatarSkinAsync().ToObservable().ObserveOnMainThread();
    async Task<JObject> GetAvatarSkinAsync()
    {
        var userId = await getFirebaseUid.Execute();
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = $"{Constants.ApiRoot}/avatar/{userId}";
        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        return response.json;
    }
    public IObservable<JObject> GetAvatarBlockedList() => GetAvatarBlockedListAsync().ToObservable().ObserveOnMainThread();
    async Task<JObject> GetAvatarBlockedListAsync()
    {
        var userId = gameData.GetUserInformation().UserId.ToString();
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = $"{Constants.UsersEndpoint}/{userId}/blocked-user";
        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        return response.json;
    }

    public IObservable<JObject> GetAvatarBlockerList() => GetAvatarBlockerListAsync().ToObservable().ObserveOnMainThread();

    async Task<JObject> GetAvatarBlockerListAsync()
    {
        var userId = gameData.GetUserInformation().UserId.ToString();
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = $"{Constants.UsersEndpoint}/{userId}/blocker-user";
        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        return response.json;
    }

    public IObservable<JObject> SetAvatarSkin(JObject json) => SetAvatarSkinAsync(json).ToObservable().ObserveOnMainThread();
    async Task<JObject> SetAvatarSkinAsync(JObject json)
    {
        try
        {
            var userId = await getFirebaseUid.Execute();
            var bearerTokenHeader = await headersBuilder.BearerToken;
            var endpoint = $"{Constants.ApiRoot}/avatar/{userId}";

            var response = await WebClient.Put(endpoint, json, true, bearerTokenHeader);

            return response.json;
        }
        catch (Exception e)
        {
            debugConsole.ErrorLog("AvatarEndpoints:SetAvatarSkinAsync", "Endpoint Failed", "");
        }
        return new JObject();
    }

    public IObservable<List<GenericBagItem>> PurchaseItem(List<GenericCatalogItem> listItems) => PurchaseItemAsync(listItems).ToObservable().ObserveOnMainThread();
    async Task<List<GenericBagItem>> PurchaseItemAsync(List<GenericCatalogItem> listItems)
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = Constants.PurchaseEndpoint;

        var userId = gameData.GetUserInformation().UserId;

        JObject jsonPurchases = new JObject();
        jsonPurchases["user_id"] = userId;

        JArray jArray = new JArray();
        foreach (GenericCatalogItem item in listItems)
        {
            JObject jItem = new JObject();
            jItem["cost"] = item.GemsPrice;
            jItem["price_type"] = "gems";
            jItem["item_id"] = item.IdItemWebClient;

            jArray.Add(jItem);
        }

        JProperty listPurchases = new JProperty("purchases", jArray);
        jsonPurchases.Add(listPurchases);

        var response = await WebClient.Post(endpoint, jsonPurchases, true, bearerTokenHeader);

        Debug.Log("ITEMPURCHASE:" + response.json);

        return GetBagItemFromJson(response.json);
    }

    List<GenericBagItem> GetBagItemFromJson(JObject jObject)
    {
        List<GenericBagItem> listBagItems = new List<GenericBagItem>();
        foreach (JObject itemJson in jObject["data"])
        {
            try
            {
                int idItem = itemJson["item_id_in_game"].Value<int>();
                int idInstance = itemJson["inventory_item_id"].Value<int>();

                string sItemType = itemJson["item_type"].Value<string>();
                ItemType itemType = itemTypeUtils.GetItemTypeByName(sItemType);

                GenericCatalog catalog = gameData.GetCatalogByItemType(itemType);

                if (catalog != null)
                {
                    GenericCatalogItem objCat = catalog.GetItem(idItem);
                    if (objCat != null)
                    {
                        listBagItems.Add(new GenericBagItem(itemType, idInstance, 1, objCat));
                    }
                    else
                    {
                        debugConsole.ErrorLog("AvatarEndpoints:GetBagItemFromJson", "idItem Not Found", $"itemType:{itemType} idItem:{idItem}");
                    }
                }
                else
                {
                    debugConsole.ErrorLog("AvatarEndpoints:GetBagItemFromJson", "invalid Item Type", $"itemType:{itemType}");
                }
            }
            catch (Exception e)
            {
                debugConsole.ErrorLog("AvatarEndpoints:GetBagItemFromJson", "Error Parsing Json", "");
            }
        }

        return listBagItems;
    }

    public IObservable<UserInformation> GetUserInformation() => GetUserInformationAsync().ToObservable().ObserveOnMainThread();
    async Task<UserInformation> GetUserInformationAsync()
    {
        try
        {
            var userId = await getFirebaseUid.Execute();
            var bearerTokenHeader = await headersBuilder.BearerToken;
            var endpoint = $"{Constants.ApiRoot}/users/{userId}";

            var response = await WebClient.Get(endpoint, false, bearerTokenHeader);
            return ToUserInformation(response.json);
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public UserInformation ToUserInformation(JObject jObject)
    {
        try
        {
            UserInformation userInformation = new UserInformation();
            JObject userJson = jObject["data"].Value<JObject>();

            Debug.Log("** ToUserInformation:" + userJson);

            int idUser = userJson["id"].Value<int>();
            string idFirebase = userJson["firebase_uid"].Value<string>();
            string username = userJson["username"].Value<string>();
            string firstName = userJson["first_name"].Value<string>();
            string lastName = userJson["last_name"].Value<string>();
            int friendCount = userJson["friend_count"].Value<int>();
            int friendRequestsCount = userJson["friend_request_count"].Value<int>();
            int gold = userJson["gold"].Value<int>();
            int gems = userJson["gems"].Value<int>();
            string country = userJson["country"].Value<string>();
            string email = userJson["email"].Value<string>();
            string gender = userJson["gender"].Value<string>();
            string photo_url = userJson["photo_url"].Value<string>();
            bool doAvatarCustomization = !userJson.Value<bool>("custom_avatar");

            userInformation.UserId = idUser;
            userInformation.UserName = username;
            userInformation.FirstName = firstName;
            userInformation.LastName = lastName;
            userInformation.FriendCount = friendCount;
            userInformation.FriendRequestsCount = friendRequestsCount;
            userInformation.FirebaseId = idFirebase;
            userInformation.SetGems(gems);
            userInformation.SetGold(gold);
            userInformation.Country = country;
            userInformation.Email = email;
            userInformation.Gender = gender;
            userInformation.Photo_url = photo_url;
            userInformation.Do_avatar_customization = doAvatarCustomization;

            return userInformation;
        }
        catch (Exception e)
        {
            debugConsole.ErrorLog("AvatarEndpoints:ToAvatarGenericCatalogItem", "Exception", "Invalid Json Data");
        }

        return null;
    }

    public IObservable<bool> PurchaseValidation(string receiptValidation, string transactionId) => PurchaseValidationAsync(receiptValidation, transactionId).ToObservable().ObserveOnMainThread();
    async Task<bool> PurchaseValidationAsync(string receiptValidation, string transactionId)
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = Constants.VerifyApplePurchaseEndpoint;

        var userId = await getFirebaseUid.Execute();

        JObject jsonPurchases = new JObject();
        JObject jsonReceipt = new JObject();
        jsonReceipt["receipt"] = receiptValidation;
        jsonReceipt["user_id"] = userId;
        jsonReceipt["transaction_id"] = transactionId;

        jsonPurchases["purchase"] = jsonReceipt;

        var response = await WebClient.Post(endpoint, jsonPurchases, true, bearerTokenHeader);

        JObject jsonResponse = response.json;
        int status = jsonResponse["status"].Value<int>();

        return status == 0;
    }

    public IObservable<bool> PurchaseValidationGoogle(string productId, string token) => PurchaseValidationGoogleAsync(productId, token).ToObservable().ObserveOnMainThread();
    async Task<bool> PurchaseValidationGoogleAsync(string productId, string token)
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = Constants.VerifyGooglePurchaseEndpoint;

        var userId = await getFirebaseUid.Execute();

        JObject jsonPurchases = new JObject();
        JObject jsonReceipt = new JObject();
        jsonReceipt["product_id"] = productId;
        jsonReceipt["user_id"] = userId;
        jsonReceipt["token"] = token;

        jsonPurchases["purchase"] = jsonReceipt;

        var response = await WebClient.Post(endpoint, jsonPurchases, true, bearerTokenHeader);

        JObject jsonResponse = response.json;

        int status = jsonResponse["purchaseState"].Value<int>();

        return status == 0;
    }

    public async Task<bool> GameFriendInviteAsync(int userId, int guestUserId, Game miniGame)
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = Constants.GameInvite;
        JObject jsonMain = new JObject();
        JObject jObject = new JObject();
        jObject["minigame"] = Enum.GetName(typeof(Game), miniGame).ToLower();
        jObject["user_id"] = userId;
        jObject["guest_user_id"] = guestUserId;

        jsonMain["game_invitation"] = jObject;
        var response = await WebClient.Post(endpoint, jsonMain, true, bearerTokenHeader);

        JObject jsonResponse = response.json;

        int status = 0;

        return status == 0;
    }


    public async Task<bool> GameFriendInviteInteractAsync(string gameInvitationId, bool accept)
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;
        string acceptOrReject = accept ? "accept" : "reject";
        var endpoint = Constants.GameInvite + "/" + gameInvitationId.ToString() + "/" + acceptOrReject;
   
        JObject jObject = new JObject();

        var response = await WebClient.Put(endpoint, jObject, true, bearerTokenHeader);

        JObject jsonResponse = response.json;

        int status = 0;

        return status == 0;
    }

    public IObservable<bool> TrackUserSession() => OnTrackUserSession().ToObservable().ObserveOnMainThread();
    async Task<bool> OnTrackUserSession()
    {
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = Constants.SessionsEndpoint;

        var userId = gameData.GetUserInformation().UserId;
        JObject jsonTrackUserSession = new JObject();
        JObject jsonReceipt = new JObject();
        jsonReceipt["user_id"] = userId;

        jsonTrackUserSession["user_session"] = jsonReceipt;

        var response = await WebClient.Post(endpoint, jsonTrackUserSession, true, bearerTokenHeader);

        JObject jsonResponse = response.json;

        return true;
    }

    public IObservable<bool> UpdateLanguage(string languageKey) => UpdateLanguageAsync(languageKey).ToObservable().ObserveOnMainThread();
    async Task<bool> UpdateLanguageAsync(string languageKey)
    {
        var userId = gameData.GetUserInformation().UserId;

        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = Constants.UsersRootEndPoint + "/" + userId;

        JObject jsonUpdateLanguage = new JObject();
        JObject jsonReceipt = new JObject();
        jsonReceipt["language"] = languageKey;

        jsonUpdateLanguage["user"] = jsonReceipt;

        var response = await WebClient.Put(endpoint, jsonUpdateLanguage, true, bearerTokenHeader);

        JObject jsonResponse = response.json;

        return true;
    }

    public IObservable<UserAccountStatus> GetUserStatus() => GetUserStatusAsync().ToObservable().ObserveOnMainThread();
    async Task<UserAccountStatus> GetUserStatusAsync()
    {
        UserAccountStatus userStatus = new UserAccountStatus();

        var userId = gameData.GetUserInformation().UserId;
        var bearerTokenHeader = await headersBuilder.BearerToken;
        var endpoint = $"{Constants.ApiRoot}/users/{userId}/status";

        var response = await WebClient.Get(endpoint, false, bearerTokenHeader);

        JObject jsonResponse = response.json;
        JObject data = jsonResponse["data"].Value<JObject>();
        string status = data["status"].Value<string>();


        string timeSuspensionStart = "";
        string timeSuspensionEnd = "";
        string timeSuspensionLeft = "";

        try
        {
            timeSuspensionStart = data["suspension_start"].Value<string>();
            timeSuspensionEnd = data["suspension_end"].Value<string>();
            timeSuspensionLeft = data["time_left_in_seconds"].Value<string>();
        }
        catch (Exception e)
        {
        }
        userStatus.SetStatus(status, userId, timeSuspensionStart, timeSuspensionEnd, timeSuspensionLeft);

        return userStatus;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Data.Catalog;
using Newtonsoft.Json.Linq;
using Data.Bag;
using Data.Users;
using System.Threading.Tasks;

public interface IAvatarEndpoints
{
    IObservable<List<AvatarGenericCatalogItem>> GetAvatarCatalogItemsList();
    IObservable<List<GenericBagItem>> GetPlayerInventory();
    IObservable<JObject> GetAvatarSkin();
    IObservable<JObject> GetAvatarBlockedList();
    IObservable<bool> SetPlayerCoins(int coins);
    IObservable<JObject> GetAvatarBlockerList();
    IObservable<JObject> SetAvatarSkin(JObject json);
    IObservable<UserInformation> GetUserInformation();
    IObservable<List<GenericBagItem>> PurchaseItem(List<GenericCatalogItem> listItems);
    IObservable<bool> PurchaseValidation(string receiptValidation, string transactionId);
    IObservable<bool> PurchaseValidationGoogle(string productId, string token);
    IObservable<bool> TrackUserSession();
    IObservable<bool> UpdateLanguage(string languageKey);
    IObservable<UserAccountStatus> GetUserStatus();

    public Task<bool> GameFriendInviteAsync(int userId, int guestUserId, Game miniGame);
    public Task<bool> GameFriendInviteInteractAsync(string gameInvitationId, bool accept);

    UserInformation ToUserInformation(JObject jObject);
}

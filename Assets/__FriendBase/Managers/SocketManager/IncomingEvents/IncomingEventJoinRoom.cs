using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System;
using Data.Users;
using Data.Catalog.Items;
using Architecture.Injector.Core;
using Data;
using Data.Catalog;

namespace Socket
{
    public class IncomingEventJoinRoom : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.JOIN_ROOM;

        public List<AvatarRoomData> ListAvatarData { get; private set; }
        public List<FurnitureRoomData> ListFurnitureData { get; private set; }
        public int ownerUserId;
        public int eventType = -1;

        public IncomingEventJoinRoom(JObject message) : base(message)
        {
            IItemTypeUtils itemTypeUtils = Injection.Get<IItemTypeUtils>();
            IGameData gameData = Injection.Get<IGameData>();

            try
            {
                ListAvatarData = new List<AvatarRoomData>();
                ListFurnitureData = new List<FurnitureRoomData>();

                JObject response = Payload[SocketTags.RESPONSE].Value<JObject>();

                if (!string.IsNullOrEmpty(response[SocketTags.ROOM_OWNER_ID].Value<string>()))
                {
                    ownerUserId = response[SocketTags.ROOM_OWNER_ID].Value<int>();
                }

                JObject eventData = response[SocketTags.EVENT_DATA].Value<JObject>();
                if (eventData != null)
                {
                    string eventTypeString = eventData[SocketTags.EVENT_TYPE].Value<string>();
                    if (!eventTypeString.Equals("none"))
                    {
                        eventType = int.Parse(eventTypeString);
                    }
                }

                foreach (JObject avatarData in response[SocketTags.MEMBER_POSITIONS])
                {
                    float positionx = avatarData[SocketTags.POSITION_X].Value<float>();
                    float positiony = avatarData[SocketTags.POSITION_Y].Value<float>();
                    string avatarState = avatarData[SocketTags.AVATAR_STATE].Value<string>();
                    int orientation = 1;

                    JObject userAvatar = avatarData[SocketTags.AVATAR_DATA].Value<JObject>();

                    AvatarCustomizationData avatarCustomizationData = new AvatarCustomizationData();
                    avatarCustomizationData.SetDataFromJoinRoom(userAvatar);

                    string firebaseId = avatarData[SocketTags.FIREBASE_ID].Value<string>();
                    string username = avatarData[SocketTags.USER_NAME].Value<string>();
                    string userId = avatarData[SocketTags.USER_ID].Value<string>();

                    JObject petData = avatarData[SocketTags.PET_DATA].Value<JObject>();
                    int? petId = petData[SocketTags.PET_ID].Value<int?>();
                    int? petIdInGame = petData[SocketTags.PET_ID_IN_GAME].Value<int?>();
                    string PetPrefabName = petData[SocketTags.PET_PREFAB].Value<string>();
                    ListAvatarData.Add(new AvatarRoomData(firebaseId, userId, username, avatarState, positionx, positiony, orientation, avatarCustomizationData, petId, petIdInGame, PetPrefabName));
                }

                foreach (JObject furnitureData in response[SocketTags.FURNITURE])
                {
                    JObject itemData = furnitureData[SocketTags.ITEM_DATA].Value<JObject>();
                    int idItem = itemData[SocketTags.ID_IN_GAME].Value<int>();
                    string itemTypeString = itemData[SocketTags.TYPE].Value<string>();
                    ItemType itemType = itemTypeUtils.GetItemTypeByName(itemTypeString);

                    float positionx = furnitureData[SocketTags.POSITION_X].Value<float>();
                    float positiony = furnitureData[SocketTags.POSITION_Y].Value<float>();
                    int orientation = furnitureData[SocketTags.ORIENTATION].Value<int>();
                    int furnitureIdInstance = furnitureData[SocketTags.ID].Value<int>();
                    int idInventoryItem = furnitureData[SocketTags.INVENTORY_ITEM_ID].Value<int>();

                    if (itemTypeUtils.IsFurnitureType(itemType))
                    {
                        GenericCatalogItem objCat = gameData.GetCatalogByItemType(itemType).GetItem(idItem);
                        if (objCat != null)
                        {
                            Vector2 position = new Vector2(positionx, positiony);
                            ListFurnitureData.Add(new FurnitureRoomData(objCat, furnitureIdInstance, idInventoryItem, position, orientation));
                        }
                    }
                }
                State = SocketEventResult.OPERATION_SUCCEED;
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}


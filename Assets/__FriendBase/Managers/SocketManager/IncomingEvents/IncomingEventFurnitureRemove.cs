using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Data.Catalog;
using Data.Catalog.Items;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Socket
{
    public class IncomingEventFurnitureRemove : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.REMOVE_FURNITURE;

        public int IdRoomInstance { get; private set; }
        public int IdInventoryItem { get; private set; }
        public GenericCatalogItem ObjCat;

        public IncomingEventFurnitureRemove(JObject message) : base(message)
        {
            IItemTypeUtils itemTypeUtils = Injection.Get<IItemTypeUtils>();
            IGameData gameData = Injection.Get<IGameData>();

            try
            {
                JObject furnitureData = Payload[SocketTags.REMOVED_FURNITURE].Value<JObject>();
                //item_data
                IdRoomInstance = furnitureData[SocketTags.ID].Value<int>();


                //item_data
                IdInventoryItem = furnitureData[SocketTags.INVENTORY_ITEM_ID].Value<int>();

                JObject itemData = furnitureData[SocketTags.ITEM_DATA].Value<JObject>();

                int idItem = itemData[SocketTags.ID_IN_GAME].Value<int>();
                string itemTypeString = itemData[SocketTags.TYPE].Value<string>();
                ItemType itemType = itemTypeUtils.GetItemTypeByName(itemTypeString);

                if (itemTypeUtils.IsFurnitureType(itemType))
                {
                    ObjCat = gameData.GetCatalogByItemType(itemType).GetItem(idItem);
                    if (ObjCat != null)
                    {
                        State = SocketEventResult.OPERATION_SUCCEED;
                    }
                    else
                    {
                        State = SocketEventResult.OPERATION_ERROR;
                    }
                }
                else
                {
                    State = SocketEventResult.OPERATION_ERROR;
                }
            }
            catch (Exception e)
            {
                State = SocketEventResult.OPERATION_PARSING_ERROR;
            }
        }
    }
}
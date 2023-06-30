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
    public class IncomingEventFurnitureMove : AbstractIncomingSocketEvent
    {
        public override string EventType => SocketEventTypes.MOVE_FURNITURE;

        public int IdInstance { get; private set; }
        public ItemType ItemType { get; private set; }
        public float Positionx { get; private set; }
        public float Positiony { get; private set; }
        public int Orientation { get; private set; }

        public IncomingEventFurnitureMove(JObject message) : base(message)
        {
            IItemTypeUtils itemTypeUtils = Injection.Get<IItemTypeUtils>();
            IGameData gameData = Injection.Get<IGameData>();

            try
            {
                JObject furnitureData = Payload[SocketTags.MOVED_FURNITURE].Value<JObject>();
                //item_data
                IdInstance = furnitureData[SocketTags.ID].Value<int>();
                Positionx = furnitureData[SocketTags.POSITION_X].Value<float>();
                Positiony = furnitureData[SocketTags.POSITION_Y].Value<float>();
                Orientation = furnitureData[SocketTags.ORIENTATION].Value<int>();

                JObject itemData = furnitureData[SocketTags.ITEM_DATA].Value<JObject>();

                int idItem = itemData[SocketTags.ID_IN_GAME].Value<int>();
                string itemTypeString = itemData[SocketTags.TYPE].Value<string>();
                ItemType = itemTypeUtils.GetItemTypeByName(itemTypeString);

                if (itemTypeUtils.IsFurnitureType(ItemType))
                {
                    State = SocketEventResult.OPERATION_SUCCEED;
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Catalog;
using Architecture.Injector.Core;
using DebugConsole;
using Socket;

namespace Data.Bag
{
    public class FurnituresBag : GenericBag
    {
        public FurnituresBag(ItemType itemType):base(itemType)
        {
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.ADD_FURNITURE, OnAddFurnitureToRoom);
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.REMOVE_FURNITURE, OnRemoveFurnitureFromRoom);
        }

        void OnAddFurnitureToRoom(AbstractIncomingSocketEvent incomingSocketEvent)
        {
            //Aswe add a furniture to the room => We remove it from the inventory
            IncomingEventFurnitureAdd incomingEventAddFurniture = incomingSocketEvent as IncomingEventFurnitureAdd;
            if (incomingEventAddFurniture == null || incomingSocketEvent.State != SocketEventResult.OPERATION_SUCCEED)
            {
                return;
            }
            //TODO Check if it is my user -> Need for Gus

            //Check if it the type of the current Bag
            if (incomingEventAddFurniture.ObjCat.ItemType != ItemType)
            {
                return;
            }

            RemoveItem(incomingEventAddFurniture.ObjCat.IdItem);
        }

        void OnRemoveFurnitureFromRoom(AbstractIncomingSocketEvent incomingSocketEvent)
        {
            IncomingEventFurnitureRemove incomingEventRemoveFurniture = incomingSocketEvent as IncomingEventFurnitureRemove;
            if (incomingEventRemoveFurniture == null || incomingSocketEvent.State != SocketEventResult.OPERATION_SUCCEED)
            {
                return;
            }

            //TODO Check if it is my user -> Need for Gus

            //Check if it the type of the current Bag
            if (incomingEventRemoveFurniture.ObjCat.ItemType != ItemType)
            {
                return;
            }

            GenericBagItem bagItem = new GenericBagItem(incomingEventRemoveFurniture.ObjCat.ItemType, incomingEventRemoveFurniture.IdInventoryItem, 1, incomingEventRemoveFurniture.ObjCat);
            AddItem(bagItem);
        }
    }
}


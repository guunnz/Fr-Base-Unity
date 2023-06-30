using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using DebugConsole;
using UnityEngine;
using Socket;
using Data.Catalog;
using Data;
using Data.Catalog.Items;

public class FurnituresRoomManager 
{
    public enum PATHFINDING_REASON { ADD, MOVE, REMOVE};

    private GameObject furnituresContainer;
    private List<FurnitureRoomController> listFurnituresRoomControllers;
    private IDebugConsole debugConsole;
    private IGameData gameData;
    private IItemTypeUtils itemTypeUtils;

    private FurnitureRoomController furniturePrefab;

    public delegate void FurnitureStatusNotify(FurnitureRoomController furnitureRoomController, FurnitureRoomController.LOADING_STATE loadingState);
    public event FurnitureStatusNotify OnFurnitureStatusNotify;

    public delegate void UpdatePathfinding(FurnitureRoomController furnitureRoomController, PATHFINDING_REASON pathfindingReason);
    public event UpdatePathfinding OnUpdatePathfinding;

    public FurnituresRoomManager(FurnitureRoomController furniturePrefab, GameObject furnituresContainer)
    {
        this.furniturePrefab = furniturePrefab;
        this.furnituresContainer = furnituresContainer;
        listFurnituresRoomControllers = new List<FurnitureRoomController>();

        debugConsole = Injection.Get<IDebugConsole>();
        gameData = Injection.Get<IGameData>();
        itemTypeUtils = Injection.Get<IItemTypeUtils>();

        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.ADD_FURNITURE, OnAddFurniture);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.MOVE_FURNITURE, OnMoveFurniture);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.REMOVE_FURNITURE, OnRemoveFurniture);
    }

    public FurnitureRoomController AddFurniture(FurnitureRoomData furnitureRoomData)
    {
        FurnitureRoomController furnitureRoomController = GetFurnitureById(furnitureRoomData.IdInstance);
        if (furnitureRoomController!=null)
        {
            debugConsole.ErrorLog("FurnituresRoomManager:AddFurniture", "Furniture already created", "furnitureId:" + furnitureRoomData.IdInstance);
            return null;
        }

        furnitureRoomController = UnityEngine.Object.Instantiate(furniturePrefab, Vector3.zero, furniturePrefab.transform.rotation);
        furnitureRoomController.name = "Furniture_" + furnitureRoomData.ObjCat.NamePrefab + "_" + furnitureRoomData.IdInstance;
        furnitureRoomController.transform.SetParent(furnituresContainer.transform, true);
        furnitureRoomController.Init(furnitureRoomData);
        listFurnituresRoomControllers.Add(furnitureRoomController);

        furnitureRoomController.OnStatusNotify += OnFurnitureStatus;

        return furnitureRoomController;
    }

    void OnFurnitureStatus(FurnitureRoomController furnitureRoomController, FurnitureRoomController.LOADING_STATE loadingState)
    {
        furnitureRoomController.OnStatusNotify -= OnFurnitureStatus;
        if (OnFurnitureStatusNotify!=null)
        {
            OnFurnitureStatusNotify(furnitureRoomController, loadingState);
        }
        if (OnUpdatePathfinding!=null)
        {
            OnUpdatePathfinding(furnitureRoomController, PATHFINDING_REASON.ADD);
        }
    }

    public bool RemoveFurniture(int idInstance)
    {
        int amountFurnitures = listFurnituresRoomControllers.Count;
        for (int i = amountFurnitures - 1; i >= 0; i--)
        {
            if (listFurnituresRoomControllers[i].FurnitureRoomData.IdInstance == idInstance)
            {
                FurnitureRoomController currentFurniture = listFurnituresRoomControllers[i];

                if (OnUpdatePathfinding != null)
                {
                    OnUpdatePathfinding(currentFurniture, PATHFINDING_REASON.REMOVE);
                }
                currentFurniture.Destroy();
                listFurnituresRoomControllers.RemoveAt(i);

                return true;
            }
        }
        return false;
    }

    public void MoveFurniture(int idInstance, float positionx, float positiony, int orientation)
    {
        FurnitureRoomController furnitureRoomController = GetFurnitureById(idInstance);
        if (furnitureRoomController == null)
        {
            debugConsole.ErrorLog("FurnituresRoomManager:MoveFurniture", "Furniture not found", "furnitureId:" + idInstance);
            return;
        }

        furnitureRoomController.Show();
        furnitureRoomController.SetPosition(new Vector2(positionx, positiony));
        furnitureRoomController.SetOrientation(orientation);

        if (OnUpdatePathfinding != null)
        {
            OnUpdatePathfinding(furnitureRoomController, PATHFINDING_REASON.MOVE);
        }
    }

    public int GetAmountOfFurnitures()
    {
        return listFurnituresRoomControllers.Count;
    }

    public FurnitureRoomController GetFurnitureById(int idFurniture)
    {
        foreach (FurnitureRoomController furnitureRoomController in listFurnituresRoomControllers)
        {
            if (furnitureRoomController.FurnitureRoomData.IdInstance == idFurniture)
            {
                return furnitureRoomController;
            }
        }
        return null;
    }

    public FurnitureRoomController GetFurnitureByIndex(int index)
    {
        if (index < listFurnituresRoomControllers.Count)
        {
            return listFurnituresRoomControllers[index];
        }
        return null;
    }

    void OnRemoveFurniture(AbstractIncomingSocketEvent incomingSocketEvent)
    {
        IncomingEventFurnitureRemove incomingEventRemoveFurniture = incomingSocketEvent as IncomingEventFurnitureRemove;
        if (incomingEventRemoveFurniture == null || incomingSocketEvent.State != SocketEventResult.OPERATION_SUCCEED)
        {
            return;
        }
        RemoveFurniture(incomingEventRemoveFurniture.IdRoomInstance);
    }

    void OnMoveFurniture(AbstractIncomingSocketEvent incomingSocketEvent)
    {
        IncomingEventFurnitureMove incomingEventMoveFurniture = incomingSocketEvent as IncomingEventFurnitureMove;
        if (incomingEventMoveFurniture == null || incomingSocketEvent.State != SocketEventResult.OPERATION_SUCCEED)
        {
            return;
        }
        MoveFurniture(incomingEventMoveFurniture.IdInstance, incomingEventMoveFurniture.Positionx, incomingEventMoveFurniture.Positiony, incomingEventMoveFurniture.Orientation);
    }

    void OnAddFurniture(AbstractIncomingSocketEvent incomingSocketEvent)
    {
        IncomingEventFurnitureAdd incomingEventAddFurniture = incomingSocketEvent as IncomingEventFurnitureAdd;
        if (incomingEventAddFurniture == null || incomingSocketEvent.State != SocketEventResult.OPERATION_SUCCEED)
        {
            return;
        }

        Vector2 position = new Vector2(incomingEventAddFurniture.Positionx, incomingEventAddFurniture.Positiony);
        FurnitureRoomData furnitureRoomData = new FurnitureRoomData(incomingEventAddFurniture.ObjCat, incomingEventAddFurniture.IdInstance, incomingEventAddFurniture.IdInventoryItem, position, incomingEventAddFurniture.Orientation);
        AddFurniture(furnitureRoomData);
    }

    public void Destroy()
    {
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.ADD_FURNITURE, OnAddFurniture);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.MOVE_FURNITURE, OnMoveFurniture);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.REMOVE_FURNITURE, OnRemoveFurniture);

        foreach (FurnitureRoomController furnitureRoomController in listFurnituresRoomControllers)
        {
            furnitureRoomController.Destroy();
        }
    }
}

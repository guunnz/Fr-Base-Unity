using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AddressablesSystem;
using Architecture.Injector.Core;
using System.Linq;

public class FurnitureRoomController : MonoBehaviour, IReceiveLoadedItem
{
    [SerializeField] private GameObject container;
    [SerializeField] private SpriteSortingGroupController spriteSortingGroupController;

    public enum LOADING_STATE { NONE, LOADING, COMPLETE, FAILED };

    public FurnitureRoomData FurnitureRoomData { get; private set; }
    private ILoader loader;
    public LOADING_STATE loadingState { get; private set; }
    public GameObject FurnitureGameObject { get; private set; }

    public delegate void StatusNotify(FurnitureRoomController furnitureRoomController, LOADING_STATE loadingState);
    public event StatusNotify OnStatusNotify;

    public void Init(FurnitureRoomData furnitureRoomData)
    {
        CleanSprite();

        loader = Injection.Get<ILoader>();

        FurnitureRoomData = furnitureRoomData;
        SetPosition(FurnitureRoomData.Position);
        SetOrientation(FurnitureRoomData.Orientation);
        loadingState = LOADING_STATE.NONE;
        LoadFurniturePrefab();

        //Set Sorting Layer on Floor Items so they are always behind
        spriteSortingGroupController.pivotOffset = Vector2.zero;
        if (IsFloorItem())
        {
            spriteSortingGroupController.pivotOffset = new Vector2(0, 10);
        }
    }

    void CleanSprite()
    {
        foreach (Transform child in container.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void SetPosition(Vector2 position)
    {
        FurnitureRoomData.Position = position;
        this.transform.position = position;
    }

    public void ChangeOrientation()
    {
        SetOrientation(-FurnitureRoomData.Orientation);
    }

    public void SetOrientation(int orientation)
    {
        FurnitureRoomData.Orientation = orientation;
        if (FurnitureGameObject != null)
        {
            Vector3 scale = FurnitureGameObject.transform.localScale;
            if (orientation == 1)
            {
                scale.x = 1;
            }
            else
            {
                scale.x = -1;
            }
            FurnitureGameObject.transform.localScale = scale;
        }
    }

    public void Destroy()
    {
        OnStatusNotify = null;
        loader.Unsuscribe(this, FurnitureRoomData.ObjCat.GetNameFurniturePrefabForRoom());
        Destroy(this.gameObject);
    }

    void LoadFurniturePrefab()
    {
        FurnitureGameObject = null;

        loadingState = LOADING_STATE.LOADING;
        loader.Suscribe(this, FurnitureRoomData.ObjCat.GetNameFurniturePrefabForRoom());

        loader.LoadItem(new LoaderItemModel(FurnitureRoomData.ObjCat.GetNameFurniturePrefabForRoom()));
        if (OnStatusNotify != null)
        {
            OnStatusNotify(this, LOADING_STATE.LOADING);
        }
    }

    public void ReceiveLoadedItem(LoaderAbstractItem item)
    {
        if (item.State == LoaderItemState.SUCCEED)
        {
            if (!FurnitureRoomData.ObjCat.GetNameFurniturePrefabForRoom().Equals(item.Id))
            {
                //Prevent to receive another snapshot
                return;
            }
            loader.Unsuscribe(this, FurnitureRoomData.ObjCat.GetNameFurniturePrefabForRoom());

            FurnitureGameObject = loader.GetModel(FurnitureRoomData.ObjCat.GetNameFurniturePrefabForRoom());
            if (FurnitureGameObject != null)
            {
                FurnitureGameObject.transform.SetParent(container.transform);
                FurnitureGameObject.transform.localPosition = Vector3.zero;
                FurnitureGameObject.transform.localScale = Vector3.one;
                SetOrientation(FurnitureRoomData.Orientation);

                FurnitureLayersSelfManagment layersSelfManagment = FurnitureGameObject.GetComponent<FurnitureLayersSelfManagment>();
                if (layersSelfManagment != null)
                {
                    layersSelfManagment.ManageLayers(this);
                }
            }
            loadingState = LOADING_STATE.COMPLETE;
            if (OnStatusNotify != null)
            {
                OnStatusNotify(this, LOADING_STATE.COMPLETE);
            }
        }
        else
        {
            loadingState = LOADING_STATE.FAILED;
            if (OnStatusNotify != null)
            {
                OnStatusNotify(this, LOADING_STATE.FAILED);
            }
        }
    }

    public void Hide()
    {
        container.SetActive(false);
    }

    public void Show()
    {
        container.SetActive(true);
    }

    public bool IsChair()
    {
        return FurnitureGameObject.GetComponent<FurnitureChairController>() != null;
    }

    public bool IsFloorItem()
    {
        return FurnitureRoomData.ObjCat.ItemType == Data.Catalog.ItemType.FLOOR;
    }

    public Chair GetSitPoint(Vector3 hitPoint)
    {
        return FurnitureGameObject.GetComponent<FurnitureChairController>().GetSitPoint(hitPoint);
    }

    public void DeactiveMainSortLayer()
    {
        spriteSortingGroupController.Deactive();
    }

    public Collider2D GetPathfindingCollider()
    {
        if (FurnitureGameObject == null)
        {
            return null;
        }

        GameObject collider = FurnitureGameObject.transform.Find("Collider").gameObject;

        if (collider != null)
        {
            return collider.GetComponent<Collider2D>();
        }
        return null;
    }

    public Collider2D GetClickAreaCollider()
    {
        if (FurnitureGameObject == null)
        {
            return null;
        }

        return FurnitureGameObject.GetComponent<Collider2D>();
    }
}
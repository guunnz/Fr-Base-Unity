using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureLayersSelfManagment : MonoBehaviour
{
    public void ManageLayers(FurnitureRoomController furnitureRoomController)
    {
        furnitureRoomController.DeactiveMainSortLayer();
    }
}

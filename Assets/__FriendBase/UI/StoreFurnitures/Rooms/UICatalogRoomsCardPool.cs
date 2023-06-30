using System.Collections;
using System.Collections.Generic;
using CustomTools.ObjectPool;
using UnityEngine;

public class UICatalogRoomsCardPool : GenericObjectPool<UICatalogRoomsCardController>
{
    void Start()
    {
        AddObjects(15);
    }
}

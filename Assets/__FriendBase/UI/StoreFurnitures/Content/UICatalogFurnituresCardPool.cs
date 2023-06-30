using System.Collections;
using System.Collections.Generic;
using CustomTools.ObjectPool;
using UnityEngine;

public class UICatalogFurnituresCardPool : GenericObjectPool<UICatalogFurnituresCardController>
{
    void Start()
    {
        AddObjects(15);
    }
}

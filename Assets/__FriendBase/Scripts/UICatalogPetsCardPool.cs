
using System.Collections;
using System.Collections.Generic;
using CustomTools.ObjectPool;
using UnityEngine;

public class UICatalogPetsCardPool : GenericObjectPool<UICatalogPetsCardController>
{
    void Start()
    {
        AddObjects(5);
    }
}

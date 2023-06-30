
using System.Collections;
using System.Collections.Generic;
using CustomTools.ObjectPool;
using UnityEngine;

public class UICatalogAvatarColorCardPool : GenericObjectPool<UICatalogAvatarColorCardController>
{
    void Start()
    {
        AddObjects(10);
    }
}

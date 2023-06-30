using System.Collections;
using System.Collections.Generic;
using CustomTools.ObjectPool;
using UnityEngine;

public class UICatalogAvatarCardPool : GenericObjectPool<UICatalogAvatarCardController>
{
    void Start()
    {
        AddObjects(15);
    }
}

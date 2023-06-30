using System.Collections;
using System.Collections.Generic;
using CustomTools.ObjectPool;
using UnityEngine;

public class UIRoomListCardPool : GenericObjectPool<UIRoomListCardController>
{
    void Start()
    {
        AddObjects(8);
    }
}

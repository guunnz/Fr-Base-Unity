using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTools.ObjectPool;

public class UISelectEventTypeCardPool : GenericObjectPool<UISelectEventTypeCardController>
{
    void Start()
    {
        AddObjects(8);
    }
}

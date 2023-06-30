using System.Collections;
using System.Collections.Generic;
using CustomTools.ObjectPool;
using UnityEngine;

public class UIDataPickerCardPool : GenericObjectPool<UIDataPickerCardController>
{
    void Start()
    {
        AddObjects(15);
    }
}

using System.Collections;
using System.Collections.Generic;
using CustomTools.ObjectPool;
using UnityEngine;

public class UIPrivateChatAvatarPool : GenericObjectPool<UIPrivateChatAvatar>
{
    void Start()
    {
        AddObjects(10);
    }
}

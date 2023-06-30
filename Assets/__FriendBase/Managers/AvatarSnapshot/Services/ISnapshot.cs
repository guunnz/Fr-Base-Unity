using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Snapshots
{
    public interface ISnapshot
    {
        int CreateSnapshot(IGetSnapshotGameObject get3DSnapsotObject, Vector3 position, Quaternion rotate, Vector3 scale, float sizeFactor, int width, int height, Action<bool, int, Sprite> callback);
        Sprite GetSnapshotImage(int id);
        bool RemoveSnapshot(int id);
        Texture2D GetSnapshotTexture(int id);
        bool IsSnapshotReady(int id);
        GameObject GetGameObject();
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snapshots
{
    public interface IGetSnapshotGameObject
    {
        bool IsObjectAvailable();
        void LoadObject();
        GameObject GetObject();
    }
}


using Snapshots;
using UnityEngine;

namespace Managers.AvatarSnapshot.Core
{
    public class InitAvatarSnapshot : MonoBehaviour
    {
        void Start()
        {
            GetComponent<SnapshotAvatar>().CreateSnapshot();
        }

    }
}

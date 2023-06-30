using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSnapshotAvatarManager : MonoBehaviour
{
    private Dictionary<string, Sprite> snapshotDictionary;

    public RoomSnapshotAvatarManager()
    {
        snapshotDictionary = new Dictionary<string, Sprite>();
    }

    public void SetSnapshot(string idUser, Sprite snapshot)
    {
        if (!snapshotDictionary.ContainsKey(idUser))
        {
            snapshotDictionary.Add(idUser, snapshot);
        }
        else
        {
            snapshotDictionary[idUser] = snapshot;
        }
    }

    public Sprite GetSnapshot(string idUser)
    {
        if (snapshotDictionary.ContainsKey(idUser))
        {
            return snapshotDictionary[idUser];
        }
        return null;
    }

    public void RemoveSnapshot(string idUser)
    {
        if (snapshotDictionary.ContainsKey(idUser))
        {
            snapshotDictionary.Remove(idUser);
        }
    }
}

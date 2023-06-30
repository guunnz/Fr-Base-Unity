using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    private static SpawnPointManager _singleton;
    public static SpawnPointManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(SpawnPointManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    private Transform currentSpawnPoint;

    private void Awake()
    {
        Singleton = this;
    }

    public void SetSpawnPoint(Transform spawnPoint)
    {
        currentSpawnPoint = spawnPoint;
    }

    public Transform GetSpawnPoint()
    {
        return currentSpawnPoint;
    }
}
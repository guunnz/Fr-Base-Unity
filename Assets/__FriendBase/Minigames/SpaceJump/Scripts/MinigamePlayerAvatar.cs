using Snapshots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigamePlayerAvatar : MonoBehaviour
{
    private static MinigamePlayerAvatar _singleton;
    public static MinigamePlayerAvatar Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(MinigamePlayerAvatar)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    private Sprite FaceSprite;


    [SerializeField] private SnapshotAvatarSprite HeadSnapshotAvatarSprite;

    private void Awake()
    {
        Singleton = this;
    }


    private void Start()
    {
        HeadSnapshotAvatarSprite.CreateSnapshot();
        StartCoroutine(InitializeFace());
    }

    IEnumerator InitializeFace()
    {
        while (HeadSnapshotAvatarSprite.GetSnapshotState() != SnapshotGenericSprite.StateSnapshot.LOADED)
        {
           
            yield return null;
        }
        FaceSprite = HeadSnapshotAvatarSprite.GetSprite();
    }

    public Sprite GetFace()
    {
        return FaceSprite;
    }
}

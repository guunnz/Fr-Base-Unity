using Architecture.Injector.Core;
using Data;
using Data.Users;
using FriendsView.Core.Domain;
using Snapshots;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DoSnapshotList : MonoBehaviour, IGetSnapshotGameObject
{
    public enum TYPE_SNAPSHOT { ALL, HALF, FACE };

    [SerializeField] private GameObject loaderAnimation;
    [SerializeField] private AvatarCustomizationController avatarPrefab;
    [SerializeField] private TYPE_SNAPSHOT typeSnapshot;

    private SnapshotGeneric snapshotGeneric;
    private AvatarCustomizationData avatarCustomizationData;
    private AvatarCustomizationController avatarCustomizationController;
    private ISnapshot snapshotManager;
    private IGameData gameData;
    private GameObject avatar;


    void Awake()
    {
        snapshotManager = Injection.Get<ISnapshot>();
        snapshotGeneric = GetComponent<SnapshotGeneric>();
    }

    IEnumerator Start()
    {
        yield return null;
        gameData = Injection.Get<IGameData>();
        while (gameData.GetFriendList() != null)
        {
            yield return null;
        }

        StartCoroutine(CreateSnaphotFriendList(gameData.GetDummyFriendList()));
    }

    public IEnumerator CreateSnaphotFriendList(List<FriendData> friendList)
    {
        List<KeyValuePair<string, Data.Users.AvatarCustomizationData>> avatarDataList = friendList.Select(x => new KeyValuePair<string, Data.Users.AvatarCustomizationData>(x.fireBaseUID, x.avatarCustomizationData)).ToList();
        foreach (KeyValuePair<string, AvatarCustomizationData> avatarDataPair in avatarDataList)
        {
            //if (friendList.Single(x => x.fireBaseUID == avatarDataPair.Key).GetHeadSprite() != null)
            //    continue;

            flagAvatarReady = false;
            AvatarCustomizationData avatarData = avatarDataPair.Value;

            avatarCustomizationController = null;
            this.avatarCustomizationData = avatarData;

            //We define the desire position and rotation of the snapshot
            Vector3 position = new Vector3(0, 0, 400);

            int widthTexture = 256;
            int heightTexture = 256;
            float sizeFactor = 1.5f;

            if (typeSnapshot == TYPE_SNAPSHOT.ALL)
            {
                position = new Vector3(0, -200, 400);
                widthTexture = 512;
                heightTexture = 512;
            }
            else if (typeSnapshot == TYPE_SNAPSHOT.FACE)
            {
                position = new Vector3(0, -280, 180);
            }
            else if (typeSnapshot == TYPE_SNAPSHOT.HALF)
            {
                position = new Vector3(0, -250, 230);
            }

            Quaternion rotate = Quaternion.Euler(0, 0, 0);
            Vector3 scale = new Vector3(1, 1, 1);

            //We send a request of snapshot to the object with the empty image
            snapshotGeneric.SetSnapshot(this, position, rotate, scale, sizeFactor, widthTexture, heightTexture, (bool flag, int idSnapshot, Sprite sprite) =>
            {
                friendList.Single(x => x.fireBaseUID == avatarDataPair.Key).SetHeadSprite(snapshotGeneric.GetSprite());
            });

            while (snapshotGeneric.GetState() == SnapshotGeneric.StateSnapshot.LOADING)
            {
                yield return null;
            }
        }
    }


    private bool flagAvatarReady = false;
    public bool IsObjectAvailable()
    {
        if (avatarCustomizationController == null)
        {
            return false;
        }
        if (avatarCustomizationController.IsAvatarReady())
        {
            StartCoroutine(WaitAvatarReady());
        }

        return flagAvatarReady;
    }

    IEnumerator WaitAvatarReady()
    {
        yield return new WaitForEndOfFrame();
        flagAvatarReady = true;
    }

    public void LoadObject()
    {
        const float scaleAvatar = 20;
        avatar = Instantiate(avatarPrefab, transform.position, Quaternion.identity).gameObject;

        avatar.transform.localScale = Vector3.one * scaleAvatar;
        avatarCustomizationController = avatar.GetComponent<AvatarCustomizationController>();
        avatarCustomizationController.SetAvatarCustomizationData(avatarCustomizationData.GetSerializeData());

        avatar.transform.position = snapshotManager.GetGameObject().transform.position + new Vector3(0, 1000, 0);
    }

    public GameObject GetObject()
    {
        return avatarCustomizationController.gameObject;
    }
}
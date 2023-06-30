using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Snapshots;
using UnityEngine;

public class AutomaticSnapshotUnit : IGetSnapshotGameObject
{
    private AvatarRoomData avatarData;
    private RoomSnapshotAvatarManager roomSnapshotAvatarManager;
    private int idSnapshot;
    private ISnapshot snapshotManager;
    private GameObject avatar;
    private AvatarCustomizationController avatarPrefab;
    private AvatarCustomizationController avatarCustomizationController;
    private GameObject container;
    private MonoBehaviour monoBehaviour;

    public AutomaticSnapshotUnit(MonoBehaviour monoBehaviour, GameObject container, RoomSnapshotAvatarManager roomSnapshotAvatarManager, AvatarCustomizationController avatarPrefab, AvatarRoomData avatarData)
    {
        this.monoBehaviour = monoBehaviour;
        this.container = container;
        this.roomSnapshotAvatarManager = roomSnapshotAvatarManager;
        this.avatarPrefab = avatarPrefab;
        this.avatarData = avatarData;
        snapshotManager = Injection.Get<ISnapshot>();

        if (roomSnapshotAvatarManager.GetSnapshot(avatarData.UserId) == null)
        {
            CreateSnapshot();
        }
        else
        {
            //If the Snapshot was already created -> We update it just in case the user has chabged skin
            CreateSnapshot();
        }
    }

    public void CreateSnapshot()
    {
        //We define the desire position and rotation of the snapshot
        Vector3 position = new Vector3(0, -280, 180);

        int widthTexture = 256;
        int heightTexture = 256;
        float sizeFactor = 1.5f;

        Quaternion rotate = Quaternion.Euler(0, 0, 0);
        Vector3 scale = new Vector3(1, 1, 1);
        idSnapshot = -1;
        //Request a snapshot image
        idSnapshot = snapshotManager.CreateSnapshot(this, position, rotate, scale, sizeFactor, widthTexture, heightTexture, (bool flag, int idSnapshot, Sprite sprite) => {
            if (this.idSnapshot == idSnapshot)
            {
                Sprite snapshotImage = snapshotManager.GetSnapshotImage(idSnapshot);
                if (snapshotImage != null && flag)
                {
                    Debug.Log("=----- SET SNAPSHOT AUTO:" + avatarData.Username);
                    roomSnapshotAvatarManager.SetSnapshot(avatarData.UserId, snapshotImage);
                }
                snapshotManager.RemoveSnapshot(idSnapshot);
            }
        }
        );
    }

    private bool flagAvatarReady = false;
    public bool IsObjectAvailable()
    {
        try
        {
            if (avatarCustomizationController == null)
            {
                return false;
            }
            if (avatarCustomizationController.IsAvatarReady())
            {
                monoBehaviour.StartCoroutine(WaitAvatarReady());
            }
        }
        catch
        {
            return false;
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
        
        avatar = MonoBehaviour.Instantiate(avatarPrefab, container.transform.position, Quaternion.identity).gameObject;

        avatar.transform.localScale = Vector3.one * scaleAvatar;
        avatar.transform.SetParent(container.transform);
        avatarCustomizationController = avatar.GetComponent<AvatarCustomizationController>();
        avatarCustomizationController.SetAvatarCustomizationData(avatarData.AvatarCustomizationData.GetSerializeData());

        avatar.transform.position = snapshotManager.GetGameObject().transform.position + new Vector3(0, 1000, 0);
    }

    public GameObject GetObject()
    {
        return avatarCustomizationController.gameObject;
    }
}

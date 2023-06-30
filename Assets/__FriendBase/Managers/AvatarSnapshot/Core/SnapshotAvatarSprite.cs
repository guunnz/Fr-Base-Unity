using System.Collections;
using System.Collections.Generic;
using Data.Users;
using UnityEngine;
using UnityEngine.UI;
using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using Data;
using Data.Catalog;

namespace Snapshots
{
    [RequireComponent(typeof(SnapshotGenericSprite))]

    public class SnapshotAvatarSprite : MonoBehaviour, IGetSnapshotGameObject
    {
        public enum TYPE_SNAPSHOT { ALL, HALF, FACE };

        [SerializeField] private AvatarCustomizationController avatarPrefab;
        [SerializeField] private TYPE_SNAPSHOT typeSnapshot;

        private SnapshotGenericSprite snapshotGenericObject;
        private AvatarCustomizationData avatarCustomizationData;
        private AvatarCustomizationController avatarCustomizationController;
        private ISnapshot snapshotManager;

        void Awake()
        {
            snapshotManager = Injection.Get<ISnapshot>();
            snapshotGenericObject = GetComponent<SnapshotGenericSprite>();
        }

        public void CreateSnapshot()
        {
            AvatarCustomizationData avatarCustomizationData = new AvatarCustomizationData();
            avatarCustomizationData = Injection.Get<IGameData>().GetUserInformation().GetAvatarCustomizationData();
            
            CreateSnaphot(null, avatarCustomizationData);
        }

        public SnapshotGenericSprite.StateSnapshot GetSnapshotState()
        {
           return snapshotGenericObject.GetState();
        }

        public Sprite GetSprite()
        {
            return snapshotGenericObject.GetSprite();
        }

        public void CreateSnaphot(Action<bool, int, Sprite> callback, AvatarCustomizationData avatarCustomizationData)
        {
            flagAvatarReady = false;
            avatarCustomizationController = null;
            this.avatarCustomizationData = avatarCustomizationData;

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
            snapshotGenericObject.SetSnapshot(this, position, rotate, scale, sizeFactor, widthTexture, heightTexture, (bool flag, int idSnapshot, Sprite sprite) =>
            {
                if (callback != null)
                {
                    callback(flag, idSnapshot, sprite);
                }
            });
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
            GameObject avatar = Instantiate(avatarPrefab, transform.position, Quaternion.identity).gameObject;
            avatar.transform.localScale = Vector3.one * scaleAvatar;
            avatarCustomizationController = avatar.GetComponent<AvatarCustomizationController>();
            avatarCustomizationController.SetAvatarCustomizationData(avatarCustomizationData.GetSerializeData());

            avatar.transform.position = snapshotManager.GetGameObject().transform.position + new Vector3(0, 1000, 0);
        }

        public GameObject GetObject()
        {
            return avatarCustomizationController.gameObject;
        }

        void SetAlphaImage(GameObject gameObject, float alpha)
        {
            Color currentColor = gameObject.GetComponent<Image>().color;
            currentColor.a = alpha;
            gameObject.GetComponent<Image>().color = currentColor;
        }
    }
}


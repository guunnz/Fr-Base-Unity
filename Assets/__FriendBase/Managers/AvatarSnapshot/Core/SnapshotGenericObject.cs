using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;

namespace Snapshots
{
    [RequireComponent(typeof(Image))]
    public class SnapshotGenericObject : MonoBehaviour
    {
        public enum StateSnapshot { NOTHING, LOADING, LOADED };

        private StateSnapshot stateSnaphot;
        private Image imageSnapshot;
        private int idSnapshot;
        private Sprite snapshotImage;

        private ISnapshot snapshotManager;

        void Awake()
        {
            snapshotManager = Injection.Get<ISnapshot>();

            idSnapshot = -1;
            imageSnapshot = GetComponent<Image>();
            stateSnaphot = StateSnapshot.NOTHING;
        }

        public void SetSnapshot(IGetSnapshotGameObject get3DSnapsotObject, Vector3 position, Quaternion rotate, Vector3 scale, float sizeFactor, int width, int height, Action<bool, int, Sprite> callback)
        {
            //Request a snapshot image
            stateSnaphot = StateSnapshot.LOADING;
            idSnapshot = snapshotManager.CreateSnapshot(get3DSnapsotObject, position, rotate, scale, sizeFactor, width, height, (bool flag, int idSnapshot, Sprite sprite) => {
                if (this.idSnapshot == idSnapshot)
                {
                    snapshotImage = snapshotManager.GetSnapshotImage(idSnapshot);
                    if (snapshotImage != null && imageSnapshot != null)
                    {
                        SetImage(snapshotImage);
                        stateSnaphot = StateSnapshot.LOADED;
                    }
                    if (callback != null)
                    {
                        callback(flag, idSnapshot, sprite);
                    }
                    snapshotManager.RemoveSnapshot(idSnapshot);
                }
            }
            );
        }

        public void SetImage(Sprite sprite)
        {
            imageSnapshot.sprite = sprite;
        }

        public Sprite GetSprite()
        {
            return snapshotImage;
        }

        public void ResetCurrentSnapshot()
        {
            if (idSnapshot >= 0)
            {
                snapshotManager.RemoveSnapshot(idSnapshot);
                idSnapshot = -1;
            }
        }

        public void Destroy()
        {
            ResetCurrentSnapshot();
        }

        public StateSnapshot GetState()
        {
            return stateSnaphot;
        }
    }
}
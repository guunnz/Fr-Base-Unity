using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture.Context;
using Architecture.Injector.Core;
using System;

namespace Snapshots
{
    [RequireComponent(typeof(CameraSnapshot))]
    public class SnapshotManager : ScriptModule, ISnapshot
    {
        private CameraSnapshot cameraSnapshot;
        private List<GameObjectStateSnapshot> listSnapshots;
        private int idSnapshot;

        public override void Init()
        {
            Injection.Register<ISnapshot>(this);
            idSnapshot = 0;
            cameraSnapshot = GetComponent<CameraSnapshot>();
            listSnapshots = new List<GameObjectStateSnapshot>();
            StartCoroutine(ControlSnapshotsCreation());
        }

        public GameObject GetGameObject()
        {
            return this.gameObject;
        }

        public int CreateSnapshot(IGetSnapshotGameObject get3DSnapsotObject, Vector3 position, Quaternion rotate, Vector3 scale, float sizeFactor, int width, int height, Action<bool, int, Sprite> callback)
        {
            idSnapshot++;
            listSnapshots.Add(new GameObjectStateSnapshot(get3DSnapsotObject, idSnapshot, position, rotate, scale, sizeFactor, width, height, callback));
            return idSnapshot;
        }

        IEnumerator ControlSnapshotsCreation()
        {
            while (true)
            {
                for (int i = 0; i < listSnapshots.Count; i++)
                {
                    GameObjectStateSnapshot currentSnapshot = listSnapshots[i];
                    //First time we read this snapshot
                    if (currentSnapshot.state == GameObjectStateSnapshot.stateSnapshot.NOTHING)
                    {
                        //We check we have the interface for asking asyncronic
                        GameObject currentGameobject = currentSnapshot.gameObject;
                        if (currentGameobject == null && currentSnapshot.getSnapsotObject != null)
                        {
                            //From the interface we ask if the Model is available
                            if (currentSnapshot.getSnapsotObject.IsObjectAvailable())
                            {
                                //If it is available we can create the snapshot
                                GenerateSnapshotImage(currentSnapshot);
                                yield return null;
                            }
                            else
                            {
                                //If ot is not available yet => we send the model to LOAD and change state to LOADING
                                currentSnapshot.state = GameObjectStateSnapshot.stateSnapshot.LOADING;
                                currentSnapshot.getSnapsotObject.LoadObject();
                            }
                        }
                    }
                    else if (currentSnapshot.state == GameObjectStateSnapshot.stateSnapshot.LOADING)
                    {
                        if (currentSnapshot.getSnapsotObject.IsObjectAvailable())
                        {
                            GenerateSnapshotImage(currentSnapshot);
                            yield return null;
                        }
                    }
                }
                yield return null;
            }
        }

        Sprite GenerateSnapshotImage(GameObjectStateSnapshot currentSnapshot)
        {
            if (cameraSnapshot == null)
            {
                if (currentSnapshot.callback != null)
                {
                    currentSnapshot.callback(false, currentSnapshot.idSnaphot, null);
                }
                return null;
            }
            
            GameObject currentGameobject = currentSnapshot.getSnapsotObject.GetObject();
            if (currentGameobject != null)
            {
                Texture2D texture = cameraSnapshot.TakeObjectSnapshot(
                                currentGameobject,
                                Color.clear,
                                currentSnapshot.position,
                                currentSnapshot.rotate,
                                currentSnapshot.scale,
                                currentSnapshot.width,
                                currentSnapshot.height);

                Sprite blankSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                currentSnapshot.state = GameObjectStateSnapshot.stateSnapshot.READY;
                currentSnapshot.snapshotImage = blankSprite;
                currentSnapshot.snapshotTexture = texture;

                if (currentSnapshot.callback != null)
                {
                    currentSnapshot.callback(true, currentSnapshot.idSnaphot, blankSprite);
                }
                return blankSprite;
            }
            else
            {
                if (currentSnapshot.callback != null)
                {
                    currentSnapshot.callback(false, currentSnapshot.idSnaphot, null);
                }
                return null;
            }
        }

        public bool RemoveSnapshot(int id)
        {
            for (int i = 0; i < listSnapshots.Count; i++)
            {
                if (listSnapshots[i].idSnaphot == id)
                {
                    listSnapshots.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool IsSnapshotReady(int id)
        {
            for (int i = 0; i < listSnapshots.Count; i++)
            {
                if (listSnapshots[i].idSnaphot == id)
                {
                    if (listSnapshots[i].state == GameObjectStateSnapshot.stateSnapshot.READY)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Sprite GetSnapshotImage(int id)
        {
            for (int i = 0; i < listSnapshots.Count; i++)
            {
                if (listSnapshots[i].idSnaphot == id)
                {
                    if (listSnapshots[i].state == GameObjectStateSnapshot.stateSnapshot.READY)
                    {
                        return listSnapshots[i].snapshotImage;
                    }
                }
            }
            return null;
        }

        public Texture2D GetSnapshotTexture(int id)
        {
            for (int i = 0; i < listSnapshots.Count; i++)
            {
                if (listSnapshots[i].idSnaphot == id)
                {
                    if (listSnapshots[i].state == GameObjectStateSnapshot.stateSnapshot.READY)
                    {
                        return listSnapshots[i].snapshotTexture;
                    }
                }
            }
            return null;
        }

        private class GameObjectStateSnapshot
        {
            public enum stateSnapshot { NOTHING, LOADING, READY, ERROR };

            public GameObject gameObject;
            public IGetSnapshotGameObject getSnapsotObject;
            public int idSnaphot;
            public Sprite snapshotImage;
            public Texture2D snapshotTexture;
            public stateSnapshot state;
            public Vector3 position;
            public Quaternion rotate;
            public Vector3 scale;
            public float sizeFactor;
            public int width;
            public int height;
            public Action<bool, int, Sprite> callback;

            public GameObjectStateSnapshot(IGetSnapshotGameObject getSnapsotObject, int idSnapshot, Vector3 position, Quaternion rotate, Vector3 scale, float sizeFactor, int width, int height, Action<bool, int, Sprite> callback)
            {
                this.gameObject = null;
                this.getSnapsotObject = getSnapsotObject;
                this.idSnaphot = idSnapshot;
                this.snapshotImage = null;
                this.snapshotTexture = null;
                this.state = stateSnapshot.NOTHING;

                this.position = position;
                this.rotate = rotate;
                this.scale = scale;
                this.sizeFactor = sizeFactor;
                this.width = width;
                this.height = height;
                this.callback = callback;
            }
        }
    }
}


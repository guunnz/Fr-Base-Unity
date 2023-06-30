using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snapshots
{
    [RequireComponent(typeof(Camera))]
    public class CameraSnapshot : MonoBehaviour
    {
        private Camera myCamera;
        private GameObject containerSnapshot;

        private void Awake()
        {
            myCamera = this.GetComponent<Camera>();

            //Create Container
            containerSnapshot = new GameObject();
            containerSnapshot.name = "ContainerSnapshotGameObject";
            containerSnapshot.transform.SetParent(transform, true);
            containerSnapshot.transform.localPosition = Vector3.zero;

            // Configure the Camera
            //myCamera.orthographic = true;
            myCamera.clearFlags = CameraClearFlags.SolidColor;
            myCamera.backgroundColor = Color.clear;
            myCamera.nearClipPlane = 0.1f;
            myCamera.enabled = false;
        }

        public Texture2D TakeObjectSnapshot(GameObject gameObject, Color backgroundColor, Vector3 positionOffset, Quaternion rotation, Vector3 scale, int width = 128, int height = 128)
        {
            if (gameObject == null)
            {
                return null;
            }
            else if (gameObject.scene.name == null)
            {
                return null;
            }

            AligntObjectForSnapshotOnCamera(gameObject, myCamera, containerSnapshot, positionOffset, rotation, scale);

            // Take a snapshot
            Texture2D snapshot = TakeSnapshot(backgroundColor, width, height);

            //Reset container Snapshot
            DestroyAllChilds(containerSnapshot);

            containerSnapshot.transform.rotation = Quaternion.Euler(0, 0, 0);
            containerSnapshot.transform.localPosition = Vector3.zero;
            containerSnapshot.transform.localScale = new Vector3(1, 1, 1);

            return snapshot;
        }

        public void AligntObjectForSnapshotOnCamera(GameObject gameObject, Camera currentCamera, GameObject containerSnapshot, Vector3 positionOffset, Quaternion rotation, Vector3 scale)
        {
            //Clean Snapshot
            DestroyAllChilds(containerSnapshot);

            //Create sub Container
            GameObject subContainerSnapshot = new GameObject();
            subContainerSnapshot.name = "subContainerSnapshot";
            subContainerSnapshot.transform.SetParent(containerSnapshot.transform, true);
            subContainerSnapshot.transform.localPosition = Vector3.zero + positionOffset;
            subContainerSnapshot.transform.localRotation = Quaternion.Euler(0, 0, 0);

            gameObject.transform.SetParent(subContainerSnapshot.transform, true);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

            containerSnapshot.transform.position = currentCamera.transform.position;
            containerSnapshot.transform.localRotation = rotation;
            containerSnapshot.transform.localScale = scale;
        }

        private Texture2D TakeSnapshot(Color backgroundColor, int width, int height)
        {
            // Set the background color of the camera
            myCamera.backgroundColor = backgroundColor;

            // Get a temporary render texture and render the camera
            myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 24);
            myCamera.Render();

            // Activate the temporary render texture
            RenderTexture previouslyActiveRenderTexture = RenderTexture.active;
            RenderTexture.active = myCamera.targetTexture;

            // Extract the image into a new texture without mipmaps
            Texture2D texture = new Texture2D(myCamera.targetTexture.width, myCamera.targetTexture.height, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, myCamera.targetTexture.width, myCamera.targetTexture.height), 0, 0);
            texture.Apply(false);

            // Reactivate the previously active render texture
            RenderTexture.active = previouslyActiveRenderTexture;

            // Clean up after ourselves
            myCamera.targetTexture = null;
            RenderTexture.ReleaseTemporary(myCamera.targetTexture);

            // Return the texture
            return texture;
        }

        public void DestroyAllChilds(GameObject gameObject)
        {
            int childs = gameObject.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Object.Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }
    }
}


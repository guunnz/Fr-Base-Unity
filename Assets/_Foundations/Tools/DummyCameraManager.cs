using UnityEngine;

namespace Tools
{
    public class DummyCameraManager : MonoBehaviour
    {
        private Camera cam;

#if UNITY_EDITOR
        private void Update()
        {
            /*
         * if there is dummy camera, but also, there is another camera on world
         * then destroy the dummy camera
         */
            if (gameObject.TryGetComponent<Camera>(out var myCam) && FindObjectsOfType<Camera>().Length > 1)
            {
                if (myCam == cam)
                {
                    cam = null;
                }

                myCam.enabled = false;
            }


            /*
         * if there is not pointed camera, then find one
         */
            if (!cam)
            {
                cam = FindObjectOfType<Camera>();
            }

            /*
         * if there is not pointed camera after try to find one, then create a dummy camera
         */

            if (!cam)
            {
                cam = gameObject.AddComponent<Camera>();
            }
        }
#endif
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TransitionCamera : MonoBehaviour
{
    public CinemachineVirtualCamera cameraOnEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            AvatarRoomController avatar = collision.GetComponent<AvatarRoomController>();

            if (!avatar.IsLocalPlayer())
                return;

            CinemachineCamera mainCam = CinemachineCamera.Singleton;
            CinemachineVirtualCamera currentCam = mainCam.GetBrain().ActiveVirtualCamera as CinemachineVirtualCamera;

            if (mainCam.isChatOpen())
            {
                mainCam.SetCameraChatMode();
            }
            else
            {
                mainCam.SetCameraNormalMode();
            }

            currentCam.enabled = false;
            cameraOnEnter.enabled = true;
        }
    }
}
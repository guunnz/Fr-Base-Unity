using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SetConfinerColliderForCamera : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForFixedUpdate();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();
        CinemachineVirtualCamera currentVirtualCamera = null;
        while (currentVirtualCamera == null)
        {
            currentVirtualCamera = CinemachineCamera.Singleton.GetBrain().ActiveVirtualCamera as CinemachineVirtualCamera;
            yield return null;
        }

        Debug.Log(currentVirtualCamera.gameObject.name);

        CinemachineConfiner virtualCamConfiner = currentVirtualCamera.GetComponent<CinemachineConfiner>();

        while (confiner.m_BoundingShape2D == null)
        {
            confiner.m_ConfineMode = virtualCamConfiner.m_ConfineMode;
            confiner.m_ConfineScreenEdges = virtualCamConfiner.m_ConfineScreenEdges;
            confiner.m_Damping = virtualCamConfiner.m_Damping;
            confiner.m_BoundingShape2D = virtualCamConfiner.m_BoundingShape2D;
            confiner.m_BoundingVolume = virtualCamConfiner.m_BoundingVolume;
            yield return null;
        }
    }
}
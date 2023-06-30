using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingGameCoin : MonoBehaviour
{
  
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        transform.forward = new Vector3(mainCamera.transform.forward.x, transform.forward.y, mainCamera.transform.forward.z);
    }
}

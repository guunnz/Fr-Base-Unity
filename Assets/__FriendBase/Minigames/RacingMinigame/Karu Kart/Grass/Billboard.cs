using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(4f);
        if (GetComponent<BoxCollider>() != null)
            GetComponent<BoxCollider>().enabled = false;
    }

    void Update()
    {
        transform.forward = new Vector3(mainCamera.transform.forward.x, transform.forward.y, mainCamera.transform.forward.z);
    }
}

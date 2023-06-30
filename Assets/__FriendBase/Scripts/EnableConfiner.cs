using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EnableConfiner : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();
        confiner.enabled = true;
    }
}

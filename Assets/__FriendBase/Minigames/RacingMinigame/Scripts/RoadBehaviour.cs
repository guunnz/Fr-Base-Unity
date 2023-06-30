using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadBehaviour : MonoBehaviour
{
    public RacingMinigameGameplayManager RGM;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (RGM.raceStarted)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        this.gameObject.AddComponent<MeshCollider>();
        yield return new WaitForSeconds(1f);
        this.GetComponent<MeshCollider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grass"))
        {
            Destroy(other.gameObject);
        }
    }
}

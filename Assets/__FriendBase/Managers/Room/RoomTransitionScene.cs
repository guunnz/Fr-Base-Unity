using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomTransitionScene : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(GoToOtherRoomCoroutine());
    }

    IEnumerator GoToOtherRoomCoroutine()
    {
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene("RoomScene", LoadSceneMode.Additive);
        yield return new WaitForEndOfFrame();

        SceneManager.UnloadSceneAsync("GoToRoomScene");
    }
}

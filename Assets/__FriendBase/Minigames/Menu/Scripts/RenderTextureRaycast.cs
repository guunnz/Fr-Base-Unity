using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RenderTextureRaycast : MonoBehaviour
{
    public Camera portalCamera;

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            Ray ray = portalCamera.ScreenPointToRay(Input.mousePosition);

            // do we hit our portal plane?
            if (Physics.Raycast(ray, out hit))
            {
                MinigameMenuItem miniGame = hit.collider.gameObject.GetComponent<MinigameMenuItem>();
                if (miniGame == null)
                    return;
                miniGame.Select();
            }
        }
    }
}
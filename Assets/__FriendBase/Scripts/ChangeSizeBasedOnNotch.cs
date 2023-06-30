using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSizeBasedOnNotch : MonoBehaviour
{
    private PolygonCollider2D col;
    private void Start()
    {
        col = GetComponent<PolygonCollider2D>();

        float rightMultiplier = CinemachineCamera.Singleton.GetNotchXRightMultiplier();
        float leftMultiplier = CinemachineCamera.Singleton.GetNotchXLeftMultiplier();
        col.points = new Vector2[4] { new Vector2(col.points[0].x * leftMultiplier, col.points[0].y)
            , new Vector2(col.points[1].x * leftMultiplier, col.points[1].y)
            , new Vector2(col.points[2].x * rightMultiplier, col.points[2].y)
            , new Vector2(col.points[3].x * rightMultiplier, col.points[3].y) };
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WaypointType
{
    Forward = 0,
    Right = 1,
    Left = 2,
}
public class WaypointRacingGame : MonoBehaviour
{
    public WaypointType waypointType;

    public Transform leftPathNode;
    public Transform midPathNode;
    public Transform rightPathNode;
    public Transform finalPivot;
    [SerializeField] List<Sprite> grassSprites;
    [SerializeField] List<SpriteRenderer> grassSpriteRenderers;

    private void Awake()
    {
        foreach (SpriteRenderer grassSR in grassSpriteRenderers)
        {
            Sprite sprite = grassSprites[Random.Range(0, grassSprites.Count)];

            if (sprite == null)
            {
                continue;
            }

            grassSR.sprite = sprite;
        }
    }

    public Transform GetWaypoint(RacingPath path)
    {
        switch (path)
        {
            case RacingPath.Left:
                return leftPathNode;
            case RacingPath.Mid:
                return midPathNode;
            case RacingPath.Right:
                return rightPathNode;
            default:
                return null;
        }
    }
}

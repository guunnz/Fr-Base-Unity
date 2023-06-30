using System;
using UnityEngine;

public class Chair : MonoBehaviour
{
    [SerializeField] float sitY = 0.5f;
    [SerializeField] float sitX = 0f;
    [SerializeField] float chairSittingLeaway = 0.1f;
    private AvatarSittingController avatarSitting;

    [SerializeField] private SpriteRenderer armRest;
    private bool occupied = false;

    private void Awake()
    {
        if (armRest != null)
            armRest.enabled = false;
    }

    public Vector2 GetSitpoint()
    {
        return transform.position + (Vector3)(transform.localScale * new Vector2(sitX, sitY));
    }

    public float GetChairSittingLeaway()
    {
        return chairSittingLeaway;
    }

    public void SetChairOccupied(bool occupied, AvatarSittingController avatarSitting)
    {
        if (!occupied && armRest != null)
        {
            armRest.enabled = false;
        }
        else if (armRest != null)
        {
            armRest.enabled = true;
        }
        this.avatarSitting = avatarSitting;
        this.occupied = occupied;
    }

    public bool IsOccupied()
    {
        if (avatarSitting == null)
        {
            this.occupied = false;
        }
        return this.occupied;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.92f, 0.02f, 0.4f);
        Gizmos.DrawCube(GetSitpoint(), new Vector2(0.25f, 0.05f));
    }
}
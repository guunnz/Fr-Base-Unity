using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlatformSpaceJump : MonoBehaviour
{
    [SerializeField] BoxCollider2D colliderToEnable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            colliderToEnable.enabled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            colliderToEnable.enabled = false;
        }
    }
}

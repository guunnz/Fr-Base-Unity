using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SortingGroup))]
public class SpriteSortingGroupController : MonoBehaviour
{
    public Vector2 pivotOffset;

    SortingGroup sortingGroup;

#if UNITY_EDITOR

    public int debugValue;

#endif
    void Start()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }

    void Update()
    {
        if (sortingGroup)
        {
            var layer = -(int) ((transform.position.y + pivotOffset.y - 5) * 1000); //We substract 5 in order to have always negative numbers

#if UNITY_EDITOR
            debugValue = layer;
#endif

            sortingGroup.sortingOrder = layer;
        }
    }

    void OnDrawGizmos()
    {
        var pos = transform.position + new Vector3(pivotOffset.x, pivotOffset.y, 0);
        Gizmos.color = new Color(0.1f, 0.3f, 0.9f, 0.7f);
        Gizmos.DrawSphere(pos, Mathf.Max(pivotOffset.magnitude * 0.15f, 0.1f));
    }

    public void Deactive()
    {
        sortingGroup.enabled = false;
        this.enabled = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FurnitureChairController : MonoBehaviour
{
    [SerializeField] List<Chair> SitPoints;

    [SerializeField] float chairSittingLeaway = 0.1f;

    private bool occupied = false;
    


    private void Start()
    {
        SetChairOccupied(IsOccupied());
    }

    public Chair GetSitPoint(Vector3 hitPoint)
    {
        if (SitPoints.Count == 1)
        {
            return SitPoints[0];
        }
        float distance = 10000;

        Chair chair = null;

        foreach (Chair Chair in SitPoints)
        {
            if (Vector3.Distance(Chair.transform.position, hitPoint) < distance)
            {
                chair = Chair;
                distance = Vector3.Distance(Chair.transform.position, hitPoint);
            }
        }

        return chair;
    }

    public float GetChairSittingLeaway()
    {
        return chairSittingLeaway;
    }

    public void SetChairOccupied(bool occupied)
    {
        this.occupied = occupied;
    }

    public bool IsOccupied()
    {
        return this.occupied;
    }
}

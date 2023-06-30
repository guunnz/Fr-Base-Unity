using System.Collections.Generic;
using UnityEngine;

namespace Managers.InRoomGems
{
    public class InRoomGemsPoints : MonoBehaviour
    {
        [SerializeField] private List<Transform> points;

        public Vector3 GetRandomGemPoint()
        {
            if (points.Count == 0)
            {
                Debug.LogError("No gem points found");
                return Vector3.zero;
            }
            
            var randomIndex = Random.Range(0, points.Count);
            return points[randomIndex].position;
        }
    }
}
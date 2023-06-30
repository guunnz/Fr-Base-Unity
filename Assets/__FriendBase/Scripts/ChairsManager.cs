using PlayerRoom.View;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChairsManager : MonoBehaviour
{
    private List<Chair> chairList = new List<Chair>();
    private static ChairsManager _singleton;
    public static ChairsManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(ChairsManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    private void Awake()
    {
        Singleton = this;
    }
    // Update is called once per frame


    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        chairList = FindObjectsOfType<Chair>().ToList();
    }
    public Chair GetClosestChair(Vector2 destination)
    {
        float min = 100000;
        Chair closestChair = chairList.First();
        foreach (Chair chair in chairList)
        {
            float distance = Vector2.Distance(chair.GetSitpoint(), destination);
            if (distance < min)
            {
                closestChair = chair;
                min = distance;
            }
        }
        return closestChair;
    }
}

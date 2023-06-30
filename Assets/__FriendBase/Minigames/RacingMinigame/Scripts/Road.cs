using PathCreation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Road : MonoBehaviour
{
    public PathCreator pathCreatorMid;
    public PathCreator pathCreatorRight;
    public PathCreator pathCreatorLeft;

    private void Awake()
    {
        List<CarController> players = FindObjectsOfType<CarController>().ToList();

        players.ForEach(x => x.SetPaths(this));
    }
}

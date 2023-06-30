using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PathCreation.Examples;
using System;
using PathCreation;

public enum PathTurnType
{
    Forward = 0,
    Right = 1,
    HorizontalRight = 2,
    BackRight = 3,
    GoBack = 4,
    BackLeft = 5,
    HorizontalLeft = 6,
    Left = 7,
}

[System.Serializable]
public class PathWaypoint
{
    public GameObject WaypointPrefab;
    public PathTurnType pathType;
    public float Angle;
}

[System.Serializable]
public class PowerUpRace
{
    public GameObject PowerupPrefab;
    public RacingPowerUp.PowerUpType Type;
}

public class RacingMinigameTrackManager : MonoBehaviour
{
    [SerializeField] private List<PathTurnType> PathsThatCreateRoad;
    [SerializeField] private List<PowerUpRace> RacePowerupList;
    [SerializeField] private List<PathWaypoint> WaypointsPrefabs;
    [SerializeField] private Transform FirstWaypoint;

    [SerializeField] private GeneratePathExample LeftPath;
    [SerializeField] private GeneratePathExample MidPath;
    [SerializeField] private GeneratePathExample RightPath;

    [SerializeField] private Vector2 PowerUpChance1In;
    [SerializeField] private Transform Goal;
    [SerializeField] private PathSceneTool RoadCreator;


    [SerializeField] private VertexPath midVertexPath;

    [SerializeField] private GameObject roadMesh;

    private float trackLength = 15;

    public Transform GetGoal()
    {
        return Goal;
    }

    public VertexPath GetMidPath()
    {
        return midVertexPath;
    }

    public void CreatePathFromSeed(string seed)
    {
        Debug.Log(seed);
        string[] seedList = seed.Split('|');

        Transform lastWaypointMid = FirstWaypoint;

        foreach (string block in seedList)
        {
            if (block != "")
            {
                PathTurnType RoadTurn = (PathTurnType)int.Parse(block.First().ToString());
                RacingPath PathForPowerup = (RacingPath)(int.Parse(block[1].ToString()) - 1);
                RacingPowerUp.PowerUpType PowerUpSpawned = (RacingPowerUp.PowerUpType)int.Parse(block[2].ToString());

                PathWaypoint waypointPath = WaypointsPrefabs.FirstOrDefault(x => x.pathType == RoadTurn);

                WaypointRacingGame wayPoint = Instantiate(waypointPath.WaypointPrefab, lastWaypointMid.transform.position, Quaternion.Euler(0, waypointPath.Angle, 0), this.transform).GetComponent<WaypointRacingGame>();

                lastWaypointMid = wayPoint.finalPivot;
                LeftPath.waypoints.Add(wayPoint.GetWaypoint(RacingPath.Left));
                MidPath.waypoints.Add(wayPoint.GetWaypoint(RacingPath.Mid));
                RightPath.waypoints.Add(wayPoint.GetWaypoint(RacingPath.Right));

                if (PowerUpSpawned != RacingPowerUp.PowerUpType.None)
                {
                    PowerUpRace powerUp = RacePowerupList.FirstOrDefault(x => x.Type == PowerUpSpawned);
                    GameObject powerUpObj = Instantiate(powerUp.PowerupPrefab, (PathForPowerup == RacingPath.Left ? wayPoint.leftPathNode.position : PathForPowerup == RacingPath.Mid ? wayPoint.midPathNode.position : wayPoint.rightPathNode.position), (PathForPowerup == RacingPath.Left ? wayPoint.leftPathNode.rotation : PathForPowerup == RacingPath.Mid ? wayPoint.midPathNode.rotation : wayPoint.rightPathNode.rotation), this.transform);

                    if (PowerUpSpawned != RacingPowerUp.PowerUpType.Coin)
                    {
                        switch (PathForPowerup)
                        {
                            case RacingPath.Mid:
                                break;
                            case RacingPath.Right:
                                Instantiate(RacePowerupList.FirstOrDefault(x => x.Type == RacingPowerUp.PowerUpType.Coin).PowerupPrefab, wayPoint.leftPathNode.position, wayPoint.leftPathNode.rotation, this.transform);
                                break;
                            case RacingPath.Left:
                                Instantiate(RacePowerupList.FirstOrDefault(x => x.Type == RacingPowerUp.PowerUpType.Coin).PowerupPrefab, wayPoint.rightPathNode.position, wayPoint.rightPathNode.rotation, this.transform);
                                break;
                        }
                    }

                    BasicPathFollower follower = powerUpObj.GetComponent<PathCreation.Examples.BasicPathFollower>();
                    if (follower != null)
                    {
                        follower.SetPaths(PathForPowerup);
                    }
                    powerUpObj.transform.LookAt(wayPoint.GetWaypoint(PathForPowerup));
                }
            }
        }


        Goal.transform.position = MidPath.waypoints.Last().position;
        Goal.transform.rotation = MidPath.waypoints.Last().rotation;
        LeftPath.GeneratePath();
        MidPath.GeneratePath();
        RightPath.GeneratePath();
        RoadCreator.TriggerUpdate();

        this.midVertexPath = MidPath.pathCreator.path;
    }

    public string CreateRandomizedPath()
    {
        Transform lastWaypointMid = FirstWaypoint;

        PathTurnType pathTypeAux = PathTurnType.Forward;

        try
        {
            string Seed = "";

            for (int i = 0; i < trackLength; i++)
            {
                PathWaypoint waypointPath = WaypointsPrefabs.FirstOrDefault(x => x.pathType == pathTypeAux);

                WaypointRacingGame wayPoint = Instantiate(waypointPath.WaypointPrefab, lastWaypointMid.transform.position, Quaternion.Euler(0, waypointPath.Angle, 0), this.transform).GetComponent<WaypointRacingGame>();

                lastWaypointMid = wayPoint.finalPivot;
                LeftPath.waypoints.Add(wayPoint.GetWaypoint(RacingPath.Left));
                MidPath.waypoints.Add(wayPoint.GetWaypoint(RacingPath.Mid));
                RightPath.waypoints.Add(wayPoint.GetWaypoint(RacingPath.Right));

                if (UnityEngine.Random.Range(PowerUpChance1In.x, PowerUpChance1In.y) == 0 && i != 0) //i != 0 is : We don't want powerups too early
                {
                    RacingPowerUp.PowerUpType powerUpSelected = (RacingPowerUp.PowerUpType)UnityEngine.Random.Range((int)RacingPowerUp.PowerUpType.SpeedUp, (int)(RacingPowerUp.PowerUpType.Coin) + 1);
                    RacingPath pathSelected = GetRandomRacingPath();

                    if (powerUpSelected == RacingPowerUp.PowerUpType.Coin)
                    {
                        powerUpSelected = (RacingPowerUp.PowerUpType)UnityEngine.Random.Range((int)RacingPowerUp.PowerUpType.SpeedUp, (int)(RacingPowerUp.PowerUpType.Coin) + 1);
                        if (powerUpSelected == RacingPowerUp.PowerUpType.Coin)
                        {
                            pathSelected = RacingPath.Mid;
                        }
                        else
                        {
                            switch (pathSelected)
                            {
                                case RacingPath.Mid:
                                    break;
                                case RacingPath.Right:
                                    Instantiate(RacePowerupList.FirstOrDefault(x => x.Type == RacingPowerUp.PowerUpType.Coin).PowerupPrefab, wayPoint.leftPathNode.position, wayPoint.leftPathNode.rotation, this.transform);
                                    break;
                                case RacingPath.Left:
                                    Instantiate(RacePowerupList.FirstOrDefault(x => x.Type == RacingPowerUp.PowerUpType.Coin).PowerupPrefab, wayPoint.rightPathNode.position, wayPoint.rightPathNode.rotation, this.transform);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        switch (pathSelected)
                        {
                            case RacingPath.Mid:
                                break;
                            case RacingPath.Right:
                                Instantiate(RacePowerupList.FirstOrDefault(x => x.Type == RacingPowerUp.PowerUpType.Coin).PowerupPrefab, wayPoint.leftPathNode.position, wayPoint.leftPathNode.rotation, this.transform);
                                break;
                            case RacingPath.Left:
                                Instantiate(RacePowerupList.FirstOrDefault(x => x.Type == RacingPowerUp.PowerUpType.Coin).PowerupPrefab, wayPoint.rightPathNode.position, wayPoint.rightPathNode.rotation, this.transform);
                                break;
                        }
                    }

                    if (i == trackLength-1)
                    {
                        powerUpSelected = RacingPowerUp.PowerUpType.Coin;
                    }

                    PowerUpRace powerUp = RacePowerupList.FirstOrDefault(x => x.Type == powerUpSelected);
                    GameObject powerUpObj = Instantiate(powerUp.PowerupPrefab, (pathSelected == RacingPath.Left ? wayPoint.leftPathNode.position : pathSelected == RacingPath.Mid ? wayPoint.midPathNode.position : wayPoint.rightPathNode.position), (pathSelected == RacingPath.Left ? wayPoint.leftPathNode.rotation : pathSelected == RacingPath.Mid ? wayPoint.midPathNode.rotation : wayPoint.rightPathNode.rotation), this.transform);

                    BasicPathFollower follower = powerUpObj.GetComponent<PathCreation.Examples.BasicPathFollower>();
                    if (follower != null)
                    {
                        follower.SetPaths(pathSelected);
                    }
                    powerUpObj.transform.LookAt(wayPoint.GetWaypoint(pathSelected));


                    Seed += ((int)pathTypeAux).ToString() + ((int)pathSelected + 1).ToString() + ((int)powerUpSelected).ToString() + "|";
                }
                else
                {
                    RacingPath pathSelected = GetRandomRacingPath();

                    RacingPowerUp.PowerUpType powerUpSelected = RacingPowerUp.PowerUpType.Coin;

                    PowerUpRace powerUp = RacePowerupList.FirstOrDefault(x => x.Type == powerUpSelected);

                    GameObject powerUpObj = Instantiate(powerUp.PowerupPrefab, (pathSelected == RacingPath.Left ? wayPoint.leftPathNode.position : pathSelected == RacingPath.Mid ? wayPoint.midPathNode.position : wayPoint.rightPathNode.position), (pathSelected == RacingPath.Left ? wayPoint.leftPathNode.rotation : pathSelected == RacingPath.Mid ? wayPoint.midPathNode.rotation : wayPoint.rightPathNode.rotation), this.transform);

                    Seed += ((int)pathTypeAux).ToString() + ((int)pathSelected + 1).ToString() + ((int)powerUpSelected).ToString() + "|";
                }
                pathTypeAux = GetRandomPath(pathTypeAux);
            }
            Debug.Log(Seed);


            Goal.transform.position = MidPath.waypoints.Last().position;
            Goal.transform.rotation = MidPath.waypoints.Last().rotation;
            LeftPath.GeneratePath();
            MidPath.GeneratePath();
            RightPath.GeneratePath();
            RoadCreator.TriggerUpdate();
            this.midVertexPath = MidPath.pathCreator.path;
            return Seed;
        }
        catch (Exception ex)
        {

        }


        return "";
    }


    public RacingPath GetRandomRacingPath()
    {
        int randomSide = UnityEngine.Random.Range(-1, 3);

        return (RacingPath)randomSide;
    }

    public PathTurnType GetRandomPath(PathTurnType path)
    {
        int randomSide = UnityEngine.Random.Range(0, 3);

        Debug.Log(randomSide);
        switch (path)
        {
            case PathTurnType.Forward:
                return randomSide == 0 ? PathTurnType.Forward : randomSide == 1 ? PathTurnType.Left : PathTurnType.Right;
            case PathTurnType.BackRight:
                return randomSide == 0 ? PathTurnType.BackRight : randomSide == 1 ? PathTurnType.GoBack : PathTurnType.HorizontalRight;
            case PathTurnType.BackLeft:
                return randomSide == 0 ? PathTurnType.BackLeft : randomSide == 1 ? PathTurnType.HorizontalLeft : PathTurnType.GoBack;
            case PathTurnType.GoBack:
                return randomSide == 0 ? PathTurnType.GoBack : randomSide == 1 ? PathTurnType.BackLeft : PathTurnType.BackRight;
            case PathTurnType.HorizontalLeft:
                return randomSide == 0 ? PathTurnType.HorizontalLeft : randomSide == 1 ? PathTurnType.Left : PathTurnType.BackLeft;
            case PathTurnType.HorizontalRight:
                return randomSide == 0 ? PathTurnType.HorizontalRight : randomSide == 1 ? PathTurnType.BackRight : PathTurnType.Right;
            case PathTurnType.Left:
                return randomSide == 0 ? PathTurnType.Left : randomSide == 1 ? PathTurnType.HorizontalLeft : PathTurnType.Forward;
            case PathTurnType.Right:
                return randomSide == 0 ? PathTurnType.Right : randomSide == 1 ? PathTurnType.Forward : PathTurnType.HorizontalRight;
            default:
                return PathTurnType.Forward;
        }
    }

    public void CreatePath()
    {
        Transform lastWaypointMid = FirstWaypoint;

        foreach (PathTurnType path in PathsThatCreateRoad)
        {
            PathWaypoint waypointPath = WaypointsPrefabs.FirstOrDefault(x => x.pathType == path);

            WaypointRacingGame wayPoint = Instantiate(waypointPath.WaypointPrefab, lastWaypointMid.transform.position, Quaternion.Euler(0, waypointPath.Angle, 0), this.transform).GetComponent<WaypointRacingGame>();

            lastWaypointMid = wayPoint.finalPivot;
            LeftPath.waypoints.Add(wayPoint.GetWaypoint(RacingPath.Left));
            MidPath.waypoints.Add(wayPoint.GetWaypoint(RacingPath.Mid));
            RightPath.waypoints.Add(wayPoint.GetWaypoint(RacingPath.Right));
        }

        LeftPath.GeneratePath();
        MidPath.GeneratePath();
        RightPath.GeneratePath();
        this.midVertexPath = MidPath.pathCreator.path;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundData 
{
    public int IdBackground { get; private set; }
    public string NamePrefab { get; private set; }
    public float CameraInitialPositionx { get; private set; }
    public float CameraInitialPositiony { get; private set; }
    public float PlayerInitialPositionx { get; private set; }
    public float PlayerInitialPositiony { get; private set; }
    public int PlayerInitialOrientation { get; private set; }
    public int MaxUsers { get; private set; }

    public BackgroundData(int idBackground, string namePrefab, float cameraInitialPositionx, float cameraInitialPositiony, float playerInitialPositionx, float playerInitialPositiony, int playerInitialOrientation, int maxUsers)
    {
        IdBackground = idBackground;
        NamePrefab = namePrefab;
        CameraInitialPositionx = cameraInitialPositionx;
        CameraInitialPositiony = cameraInitialPositiony;
        PlayerInitialPositionx = playerInitialPositionx;
        PlayerInitialPositiony = playerInitialPositiony;
        PlayerInitialOrientation = playerInitialOrientation;
        MaxUsers = maxUsers;
    }
}

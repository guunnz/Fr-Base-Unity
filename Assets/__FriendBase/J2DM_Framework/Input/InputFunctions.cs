using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputFunctions
{
    public static Vector3 mousePosition => Input.mousePosition;

    public static bool GetMouseButtonDown(int value)
    {
        return (Input.GetMouseButtonDown(value));
    }

    public static bool GetMouseButtonUp(int value)
    {
        return (Input.GetMouseButtonUp(value));
    }

    public static bool GetMouseButton(int value)
    {
        return (Input.GetMouseButton(value));
    }
}



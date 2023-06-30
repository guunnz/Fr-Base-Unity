using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameDpadButton : MonoBehaviour
{
    [SerializeField] float HorizontalValue;
    [SerializeField] float VerticalValue;
    MinigameInputManager inputManager;

    private void Start()
    {
        inputManager = MinigameInputManager.Singleton;
    }

    public void MouseDown()
    {
        Debug.Log(VerticalValue);
        inputManager.SetHorizontal(HorizontalValue);
        inputManager.SetVertical(VerticalValue);
    }

    public void MouseUp()
    {
        inputManager.SetHorizontal(0);
        inputManager.SetVertical(0);
    }
}

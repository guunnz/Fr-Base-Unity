using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MinigameInputManager : MonoBehaviour
{
    private static MinigameInputManager _singleton;
    public static MinigameInputManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(MinigameInputManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    private float HorizontalAxis;
    private float VerticalAxis;

    private bool PrimaryButton;
    private bool SecondaryButton;

    [SerializeField] Image DpadImage;
    [SerializeField] Image PrimaryImage;
    [SerializeField] Image SecondaryImage;
    [SerializeField] Sprite DpadUp;
    [SerializeField] Sprite DpadDown;
    [SerializeField] Sprite DpadRight;
    [SerializeField] Sprite DpadLeft;
    [SerializeField] Sprite DpadNeutral;

    [SerializeField] Sprite PrimaryButtonPressed;
    [SerializeField] Sprite PrimaryButtonReleased;
    [SerializeField] Sprite SecondaryButtonPressed;
    [SerializeField] Sprite SecondaryButtonReleased;

    private void Awake()
    {
        Singleton = this;
    }

    private void Update()
    {
        SetSprites();
    }

    private void SetSprites()
    {
        PrimaryImage.sprite = GetPrimaryButton() ? PrimaryButtonPressed : PrimaryButtonReleased;

        SecondaryImage.sprite = GetSecondaryButton() ? SecondaryButtonPressed : SecondaryButtonReleased;

        if (GetHorizontal() != 0)
        {
            DpadImage.sprite = GetHorizontal() > 0 ? DpadRight : DpadLeft;
        }
        else if (GetVertical() != 0)
        {
            DpadImage.sprite = GetVertical() > 0 ? DpadUp : DpadDown;
        }
        else
        {
            DpadImage.sprite = DpadNeutral;
        }
    }


    public bool AnyButton()
    {
        return GetPrimaryButton() == true || GetSecondaryButton() == true && GetHorizontal() != 0 && GetVertical() != 0;
    }

    public bool GetPrimaryButton()
    {
        return PrimaryButton;
    }

    public bool GetSecondaryButton()
    {
        return SecondaryButton;
    }

    public float GetHorizontal()
    {
        if (!Input.GetMouseButton(0))
        {
            HorizontalAxis = 0;
            return 0;
        }
        return HorizontalAxis;
    }

    public float GetVertical()
    {
        if (!Input.GetMouseButton(0))
        {
            VerticalAxis = 0;
            return 0;
        }
        return VerticalAxis;
    }

    public void SetVertical(float Vertical)
    {
        VerticalAxis = Vertical;
    }

    public void SetHorizontal(float Horizontal)
    {
        HorizontalAxis = Horizontal;
    }

    public void SetPrimaryButton(bool pressed)
    {
        PrimaryButton = pressed;
    }

    public void SetSecondaryButton(bool pressed)
    {
        SecondaryButton = pressed;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetControlsOnStartRacing : MonoBehaviour
{
    public Button SelectButton;
    private void OnEnable()
    {
        SelectButton.onClick.Invoke();
    }
}
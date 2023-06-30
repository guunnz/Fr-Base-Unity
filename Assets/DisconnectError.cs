using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DisconnectError : MonoBehaviour
{

    public TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        //Use to debug disconnections

        //text.text = PlayerPrefs.GetString("QUIT") + Environment.NewLine + PlayerPrefs.GetString("CLOSE");
    }
}
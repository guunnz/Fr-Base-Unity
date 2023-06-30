using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.ScrollView;
using System;
using TMPro;
using Architecture.Injector.Core;

public class UIDataPickerCardController : UIAbstractCardController
{
    public enum SIZE_DATE { SMALL, MEDIUM, BIG }

    [SerializeField] protected TextMeshProUGUI txtButton;
    [SerializeField] protected TMP_FontAsset fontLight;
    [SerializeField] protected TMP_FontAsset fontBold;

    protected Action<EventType, DataPickerData, UIAbstractCardController> callback;
    public DataPickerData dataPickerData;

    public override void SetUpCard(System.Object itemData, Action<EventType, System.Object, UIAbstractCardController> callback)
    {
        dataPickerData = (DataPickerData)itemData;
        txtButton.text = dataPickerData.TxtButton;
        this.callback = callback;
    }

    public void MouseDown()
    {
        if (callback != null)
        {
            callback(EventType.MouseDown, dataPickerData, this);
        }
    }

    public void MouseUp()
    {
        if (callback != null)
        {
            callback(EventType.MouseUp, dataPickerData, this);
        }
    }

    public void SetSize(SIZE_DATE sizeDate)
    {
        switch (sizeDate)
        {
            case SIZE_DATE.BIG:
                txtButton.fontSize = 32;
                txtButton.font = fontBold;
                break;
            case SIZE_DATE.MEDIUM:
                txtButton.fontSize = 26;
                txtButton.font = fontLight;
                break;
            case SIZE_DATE.SMALL:
                txtButton.fontSize = 20;
                txtButton.font = fontLight;
                break;
        }
    }
}

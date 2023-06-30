using System;
using System.Collections;
using System.Collections.Generic;
using Data.Rooms;
using TMPro;
using UI.ScrollView;
using UnityEngine;
using UnityEngine.UI;

public class UISelectEventTypeCardController : UIAbstractCardController
{
    [SerializeField] protected TextMeshProUGUI txtEventTypeName;

    private SelectEventTypeData eventTypeData;
    protected Action<EventType, SelectEventTypeData, UIAbstractCardController> callback;

    public override void SetUpCard(System.Object itemData, Action<EventType, System.Object, UIAbstractCardController> callback)
    {
        eventTypeData = (SelectEventTypeData)itemData;
        this.callback = callback;
        txtEventTypeName.text = eventTypeData.TextEvent;
    }

    public void MouseDown()
    {
        if (callback != null)
        {
            callback(EventType.MouseDown, eventTypeData, this);
        }
    }

    public void MouseUp()
    {
        if (callback != null)
        {
            callback(EventType.MouseUp, eventTypeData, this);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectEventTypeData 
{
    public int Index { get; private set; }
    public string TextEvent { get; private set; }

    public SelectEventTypeData(int index, string textEvent)
    {
        Index = index;
        TextEvent = textEvent;
    }
}

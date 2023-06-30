using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnalytics
{
    public void SendAnalytics(AnalyticsEvent analyticsEvent, string extraText = "");
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnalyticsSender
{

    public void AddAnalytics(IAnalytics analytics);

    public void SendAnalytics(AnalyticsEvent analyticsEvent, string extraText = "");


}
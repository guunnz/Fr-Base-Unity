using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Architecture.Injector.Core;

public class AmplitudeAnalytics : MonoBehaviour, IAnalytics
{
    private IAnalyticsSender analyticsSender;

    void Start()
    {
        Amplitude amplitude = Amplitude.getInstance();
        amplitude.setServerUrl("https://api2.amplitude.com");
        amplitude.logging = true;
        amplitude.trackSessionEvents(true);
        amplitude.init("a7893a7c1ee9d40a3a59186bc2390fc7");
        analyticsSender = Injection.Get<IAnalyticsSender>();

        analyticsSender.AddAnalytics(this);
    }

    public void SendAnalytics(AnalyticsEvent analyticsEvent, string extraText = "")
    {
        string EventName = Enum.GetName(typeof(AnalyticsEvent), analyticsEvent);

        Amplitude.Instance.logEvent(EventName + extraText);
    }
}
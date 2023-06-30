using Architecture.Injector.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsOnStart : MonoBehaviour
{
    IAnalyticsSender analyticsSender;

    [SerializeField] AnalyticsEvent AnalyticsEvent;
    // Start is called before the first frame update
    void OnEnable()
    {
        analyticsSender = Injection.Get<IAnalyticsSender>();
        if (analyticsSender != null)
            analyticsSender.SendAnalytics(AnalyticsEvent);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture.Context;
using Architecture.Injector.Core;

public class AnalyticsModule : IModule
{
    public void Init()
    {
        Injection.Register<IAnalyticsService, FirebaseAnalytic>();
    }
}

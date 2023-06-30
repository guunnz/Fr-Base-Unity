using Architecture.Context;
using Architecture.Injector.Core;
using UnityEngine;

public class NauthFlowEndpointsModule : IModule
{
    public void Init()
    {
        Injection.Register<IAuthFlowEndpoint, NauthFlowEndpoints>();
    }
}

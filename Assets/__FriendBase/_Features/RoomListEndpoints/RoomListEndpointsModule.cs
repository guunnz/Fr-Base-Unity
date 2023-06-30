using System.Collections;
using System.Collections.Generic;
using Architecture.Context;
using Architecture.Injector.Core;
using UnityEngine;

public class RoomListEndpointsModule : IModule
{
    public void Init()
    {
        Injection.Register<IRoomListEndpoints, RoomListEndpoints>();
    }
}

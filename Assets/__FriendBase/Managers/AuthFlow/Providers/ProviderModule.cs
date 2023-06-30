using System.Collections;
using System.Collections.Generic;
using Architecture.Context;
using Architecture.Injector.Core;
using UnityEngine;

public class ProviderModule : IModule
{
    public void Init()
    {
        Injection.Register<IProvider, ProviderManager>();
    }
}

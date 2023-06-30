using System;

namespace Architecture.Injector.Core
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
    public class Inject : Attribute
    {
    }
}
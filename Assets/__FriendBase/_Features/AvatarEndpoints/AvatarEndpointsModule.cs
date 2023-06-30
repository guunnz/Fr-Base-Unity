using Architecture.Context;
using Architecture.Injector.Core;

public class AvatarEndpointsModule : IModule
{
    public void Init()
    {
        Injection.Register<IAvatarEndpoints, AvatarEndpoints>();
    }
}

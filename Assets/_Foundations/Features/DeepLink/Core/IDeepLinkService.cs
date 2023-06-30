using System;

namespace DeepLink.Core
{
    public interface IDeepLinkService
    {
        IObservable<DeepLinkInfo> OnDeepLink();
        DeepLinkInfo GetDeepLinkInfo();
    }
}
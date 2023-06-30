using System;
using UniRx;

namespace WebAssets.Core
{
    public interface IWebAssetUpdater
    {
        IObservable<Unit> UpdateAsset<TModel>(IWebAsset asset);
    }
}
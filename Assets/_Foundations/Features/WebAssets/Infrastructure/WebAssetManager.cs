using System;
using CloudStorage.Core;
using UniRx;
using WebAssets.Core;

namespace WebAssets.Infrastructure
{
    public class RuntimeWebAssetUpdater : IWebAssetUpdater
    {
        private readonly ICloudStorage cloudStorage;

        public RuntimeWebAssetUpdater(ICloudStorage cloudStorage)
        {
            this.cloudStorage = cloudStorage;
        }


        public IObservable<Unit> UpdateAsset<TModel>(IWebAsset asset)
        {
            cloudStorage.Fetch<TModel>(asset.Key);
            return null;
        }
    }
}
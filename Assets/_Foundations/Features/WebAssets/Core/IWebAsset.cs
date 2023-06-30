using System;

namespace WebAssets.Core
{
    public interface IWebAsset
    {
        object Info { get; set; }
        string Json { get; set; }
        string Key { get; }
        Type InfoType { get; }
        bool RuntimeUpdated { get; set; }
    }
}
using System;
using System.Collections.Generic;
using UniRx;

namespace CloudStorage.Core
{
    public interface ICloudStorage
    {
        IObservable<Unit> Save<T>(T data, string key, string type = null);
        IObservable<T> Fetch<T>(string key, string type = null);
        IObservable<List<(string version, T data)>> FetchAll<T>(string key, string type = null);
    }
}
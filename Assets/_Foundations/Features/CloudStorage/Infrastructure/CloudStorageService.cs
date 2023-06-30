using System;
using System.Collections.Generic;
using System.Linq;
using CloudStorage.Core;
using CloudStorage.Infrastructure.DTOs;
using CloudStorage.Shared;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Web;

namespace CloudStorage.Infrastructure
{
    [UsedImplicitly]
    public class CloudStorageService : ICloudStorage
    {
        public IObservable<Unit> Save<T>(T data, string key, string type = null)
        {
            var json = JsonUtility.ToJson(data);
            return WebClient
                .Post(Endpoints.SaveData(type ?? typeof(T).Name, key), json)
                .AsUnitObservable()
                .ObserveOnMainThread();
        }

        public IObservable<T> Fetch<T>(string key, string type = null)
        {
            return WebClient
                .Get(Endpoints.FetchData(type ?? typeof(T).Name, key))
                .Select(r => r.text)
                .Select(ParseJsonFirst<T>)
                .ObserveOnMainThread();
        }

        public IObservable<List<(string version, T data)>> FetchAll<T>(string key, string type = null)
        {
            return WebClient
                .Get(Endpoints.FetchData(type ?? typeof(T).Name, key))
                .Select(r => r.text)
                .Select(ParseJsonMany<T>)
                .Select(infos => infos.Select(data => (data.version, data.data)))
                .Select(infos => infos.ToList())
                .Do(SortByVersion)
                .ObserveOnMainThread();
        }

        public static void SortByVersion<T>(List<(string version, T data)> infos)
        {
            infos.Sort((v1, v2) => IsGreatVersion(v1.version, v2.version) ? -1 : 1);
        }

        private static bool IsGreatVersion(string ver1, string ver2) // v1 greater than v2
        {
            var v1 = ver1.Split('.');
            var v2 = ver2.Split('.');
            var count = Math.Max(v1.Length, v2.Length);
            for (var i = 0; i < count; i++)
            {
                if (i >= v1.Length) return false;
                if (i >= v2.Length) return true;
                var v1IsInt = int.TryParse(v1[i], out var v1Value);
                var v2IsInt = int.TryParse(v2[i], out var v2Value);
                if (!v1IsInt || !v2IsInt) return true;
                if (v1Value == v2Value) continue;
                return v1Value > v2Value;
            }

            return true;
        }

        public static T ParseJsonFirst<T>(string json)
        {
            var options = JsonUtility.FromJson<WebDataOptionsDTO<T>>(json).options;
            if (options.Count <= 0) throw new Exception("Empty list on fetch data");

            return options[0].data;
        }

        public static List<WebDataDTO<T>> ParseJsonMany<T>(string json)
        {
            var options = JsonUtility.FromJson<WebDataOptionsDTO<T>>(json).options;

            return options;
        }
    }
}
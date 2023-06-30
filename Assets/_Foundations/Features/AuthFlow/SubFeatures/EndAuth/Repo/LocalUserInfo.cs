using System;
using System.Collections.Generic;
using System.Linq;
using LocalStorage.Core;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace AuthFlow.EndAuth.Repo
{
    public class LocalUserInfo : ILocalUserInfo, IDisposable
    {
        const string StorageKey = "local-user-info";
        readonly ILocalStorage localStorage;
        JObject obj;

        public LocalUserInfo(ILocalStorage localStorage)
        {
            this.localStorage = localStorage;
        }

        void LoadData()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                obj = null;
            }
#endif
            if (obj == null)
            {
                var json = localStorage.GetString(StorageKey);
                obj = string.IsNullOrEmpty(json) ? JObject.FromObject(new object()) : JObject.Parse(json);
            }
        }

        string GetValue(string k)
        {
            LoadData();
            if (obj.ContainsKey(k) && obj[k] != null)
            {
                return obj[k].Value<string>();
            }

            return string.Empty;
        }

        void SetValue(string k, string v)
        {
            LoadData();
            obj[k] = v;
            SaveData();
        }

        public IEnumerable<string> GetKeys()
        {
            LoadData();
            return obj.Properties().Select(p => p.Name);
        }

        void SaveData()
        {
            var json = obj.ToString();
            localStorage.SetString(StorageKey, json);
        }

        public string this[string key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }

        public void Clear()
        {
            /* var firebaseLoginToken = this["firebase-login-token"]; */

            obj = null;
            localStorage.SetString(StorageKey, string.Empty);

            /* this["firebase-login-token"] = firebaseLoginToken; */
        }

        public void Refresh()
        {
            obj = null;
            LoadData();
        }

        public void Dispose()
        {
            obj = null;
        }
    }

    public interface ILocalUserInfo
    {
        string this[string key] { get; set; }
        void Clear();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using CloudStorage.Infrastructure;
using CloudStorage.Shared;
using FoundationEditorTools;
using UnityEngine;
using WebAssets.Core;

namespace WebAssets
{
    public static class WebAssetHelper //: IWebAssetUpdater
    {
        public static List<(string version, string json)> FetchAll(IWebAsset webAsset)
        {
            var dataType = webAsset.InfoType.Name;
            var key = webAsset.Key;

            var fetchDataEndpoint = Endpoints.FetchData(dataType, key, "0");
            string versionsJson;
            try
            {
                versionsJson = EditorWebClient.Get(fetchDataEndpoint);
            }
            catch (Exception)
            {
                return new List<(string version, string json)>();
            }

            var infos = CloudStorageService.ParseJsonMany<JsonStringDTO>(versionsJson);
            var list = infos.Select(info => (info.version, info.data.info)).ToList();
            CloudStorageService.SortByVersion(list);
            return list;
        }

        public static void Save(IWebAsset webAsset)
        {
            var dataType = webAsset.InfoType.Name;
            var key = webAsset.Key;
            var json = JsonUtility.ToJson(new JsonStringDTO {info = webAsset.Json});

            var saveEndpoint = Endpoints.SaveData(dataType, key);
            EditorWebClient.Post(saveEndpoint, json);
        }

        [Serializable]
        public struct JsonStringDTO
        {
            public string info;
        }
    }
}
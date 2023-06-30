using UnityEngine;

namespace CloudStorage.Shared
{
    public static class Endpoints
    {
        private const string ServerProd = "https://t89a1m70pa.execute-api.us-east-1.amazonaws.com/prod/";
        private const string CloudStorageEndpointProd = "cloud-shared-storage/";
        private static string CloudStorageEndpoint => ServerProd + CloudStorageEndpointProd;

        private static string Version => Application.version;

        //GET
        public static string FetchData(string dataType, string key, string version = null)
        {
            return $"{CloudStorageEndpoint}fetch-data/{dataType}/{key}/{version ?? Version}";
        }

        private const string SecretEditorCode =
#if UNITY_EDITOR
            "code=HelloThereGeneralKenobi";
#else
            "secret-code=ThisIsAnInvalidCodeGG";
#endif
        //POST
        public static string SaveData(string dataType, string key)
        {
            return $"{CloudStorageEndpoint}save-data/{dataType}/{key}/{Version}?{SecretEditorCode}";
        }

        //DELETE
        public static string DeleteData(string dataType, string key)
        {
            return $"{CloudStorageEndpoint}delete-data/{dataType}/{key}/{Version}";
        }

        //GET
        public static string GetKeys(string dataType)
        {
            return $"{CloudStorageEndpoint}get-keys/{dataType}/{Version}";
        }
    }
}
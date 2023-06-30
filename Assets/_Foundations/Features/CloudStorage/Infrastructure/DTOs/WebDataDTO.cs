using System;

namespace CloudStorage.Infrastructure.DTOs
{
    [Serializable]
    public struct WebDataDTO<T>
    {
        public string version;
        public string id;
        public T data;
        public string key;
        public string dataType;
    }
}
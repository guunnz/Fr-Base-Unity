using System;

namespace Localization.DTOs
{
    [Serializable]
    public struct StringKVDTO
    {
        public string k, v;

        public StringKVDTO(string k, string v)
        {
            this.k = k;
            this.v = v;
        }
    }
}
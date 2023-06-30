using System;

namespace Localization.DTOs
{
    [Serializable]
    public struct LocalizationInfoDTO
    {
        public string language;
        public StringKVDTO[] pairs;
    }
}
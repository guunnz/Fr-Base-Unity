using System.Collections.Generic;

namespace Localization.Model
{
    public class LocalizationInfo
    {
        public LocalizationInfo(Language language, Dictionary<string, string> pairs)
        {
            Language = language;
            Pairs = pairs;
        }

        public Language Language { get; }
        public Dictionary<string, string> Pairs { get; }
    }
}
using System.Collections.Generic;

namespace DeepLink.Core
{
    public class DeepLinkInfo
    {
        public readonly IReadOnlyDictionary<string, string> deepLinkEntries;
        public bool ValidDeepLink => deepLinkEntries != null && deepLinkEntries.Count > 0;

        public DeepLinkInfo(IReadOnlyDictionary<string, string> deepLinkEntries)
        {
            this.deepLinkEntries = deepLinkEntries;
        }
    }
}
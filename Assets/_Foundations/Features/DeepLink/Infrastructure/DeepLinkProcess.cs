using System;
using System.Linq;

namespace DeepLink
{
    public class DeepLinkProcess : IDeepLinkProcess
    {
        public (string key, string value)[] GetInfo(string absoluteURL)
        {
            
            /*
            deep link will contain the form:
            com.opticpower.friendbase://friendbaseDl?key1=value1&key2=value2&key3=value3
            */
            
            var array = absoluteURL.Split('?');
            
            if (array.Length < 2) return Empty;

            // "key1=value1;key2=value2;key3=value3"
            var rawKeyValues = array[1];

            //["key1=value1", "key2=value2", "key3=value3"]
            var rawPairs = rawKeyValues.Split('&');

            var pairs = rawPairs
                .Select(rawPair => rawPair.Split('='))
                .Where(pairParts => pairParts.Length == 2)
                .Select(pairParts => (pairParts[0], pairParts[1]))
                .ToArray();

            return pairs;
        }

        private static (string, string)[] Empty => Array.Empty<(string, string)>();
    }
}
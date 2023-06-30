using System;
using System.Collections.Generic;

namespace Shared.Utils
{
    public static class DictExtension
    {
        public static bool Pattern(this IReadOnlyDictionary<string, string> dict, string key, object value)
        {
            return dict.TryGetValue(key, out var v) &&
                   string.Equals(v, value.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool Pattern(this IReadOnlyDictionary<string, string> dict, string key, out string value)
        {
            value = default;
            if (!dict.TryGetValue(key, out var v)) return false;
            value = v;
            return true;
        }
    }
}
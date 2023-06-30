using System.Collections.Generic;

namespace Tools.Collections
{
    public static class ListsExtensions
    {
        public static int? Index(this IReadOnlyList<string> list, string elem)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] == elem)
                {
                    return i;
                }
            }

            return null;
        }
    }
}
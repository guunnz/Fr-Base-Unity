using System;

namespace Tools.Enums
{
    public static class EnumsExtensions
    {
        public static T? ParseEnum<T>(this string value) where T : struct
        {
            if (Enum.TryParse<T>(value, out var enumValue)) return enumValue;

            return null;
        }
    }
}
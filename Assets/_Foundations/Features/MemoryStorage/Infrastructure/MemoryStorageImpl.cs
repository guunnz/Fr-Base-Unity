using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MemoryStorage.Core;

namespace MemoryStorage.Infrastructure
{
    [UsedImplicitly]
    public class MemoryStorageImpl : IMemoryStorage, IDisposable
    {
        readonly Dictionary<string, string> dict = new Dictionary<string, string>();

        public string Get(string key)
        {
            return dict.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public void Set(string key, string value)
        {
            dict[key] = value;
        }
        public void Dispose()
        {
            dict.Clear();
        }
    }
}
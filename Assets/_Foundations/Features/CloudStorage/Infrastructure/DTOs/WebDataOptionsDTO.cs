using System;
using System.Collections.Generic;

namespace CloudStorage.Infrastructure.DTOs
{
    [Serializable]
    public struct WebDataOptionsDTO<T>
    {
        public List<WebDataDTO<T>> options;
    }
}
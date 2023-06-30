using System;
using System.Collections.Generic;

namespace Currency.Infrastructure
{
    [Serializable]
    internal class CurrenciesDTO
    {
        public List<CurrencyDTO> currencies;
    }
}
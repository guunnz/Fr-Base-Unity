using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data.Store
{
    public class StoreItemData
    {
        public string StoreItemId { get; private set; }
        public int Amount { get; private set; }
        public float Price { get; private set; }
        public bool MostPopular { get; private set; }
        public bool BestValue { get; private set; }

        public StoreItemData(string storeItemId, int amount, float price, bool mostPopular, bool bestValue)
        {
            StoreItemId = storeItemId;
            Amount = amount;
            Price = price;
            MostPopular = mostPopular;
            BestValue = bestValue;
        }
    }
}


using System;
using UnityEngine;

namespace WebAssets
{
    [CreateAssetMenu]
    public class WebAssetExample : WebAsset<WebAssetExample.ExampleDTO>
    {
        [Serializable]
        public struct ExampleDTO
        {
            public int number;
            public string text1;
            public string text2;
        }
    }
}
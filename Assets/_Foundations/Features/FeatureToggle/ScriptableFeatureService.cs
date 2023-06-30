using System;
using System.Collections.Generic;
using UnityEngine;
using WebAssets;

namespace FeatureToggle
{
    [Serializable]
    public class FeatureServiceData
    {
        public List<string> enableFeatures;
    }

    [CreateAssetMenu]
    public class ScriptableFeatureService : WebAsset<FeatureServiceData>
    {
    }
}
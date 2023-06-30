using System;
using UnityEngine;
using WebAssets.Core;

namespace WebAssets
{
    public abstract class WebAsset : ScriptableObject, IWebAsset
    {
        public string key;

        [HideInInspector] public string favouriteVersion;


        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(key)) key = GetInstanceID().ToString();
        }

        public string Key => key;
        public abstract Type InfoType { get; }

        public bool RuntimeUpdated { get; set; }


        public abstract object Info { get; set; }
        public abstract string Json { get; set; }
    }

    public abstract class WebAsset<TDTO> : WebAsset
    {
        public TDTO info;

        public override object Info
        {
            get => info;
            set => info = (TDTO) value;
        }

        public override Type InfoType => typeof(TDTO);

        public override string Json
        {
            get => JsonUtility.ToJson(info);
            set => info = JsonUtility.FromJson<TDTO>(value);
        }
    }
}
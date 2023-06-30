using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AuthFlow.Terms.View
{
    [CreateAssetMenu(menuName = Const.GameNameMenu + "Languages Info")]
    public class LanguageInfo : ScriptableObject
    {
        [Serializable]
        class LangInfo
        {
            public string key;
            //public Sprite sprite;
            public string labelName;
        }

        [SerializeField] LangInfo defaultValue;
        [SerializeField] List<LangInfo> info;

        //public Sprite GetSprite(string key)
        //{
        //    return (info.FirstOrDefault(langInfo => langInfo.key == key) ?? defaultValue).sprite;
        //}

        public string GetLabelName(string key)
        {
            return (info.FirstOrDefault(langInfo => langInfo.key == key) ?? defaultValue).labelName;
        }
    }
}
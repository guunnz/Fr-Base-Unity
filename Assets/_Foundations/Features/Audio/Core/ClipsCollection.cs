using System;
using System.Collections.Generic;
using System.Linq;
using Audio.Music;
using UnityEngine;

namespace Audio.Core
{
    [CreateAssetMenu(fileName = "Clips Collection", menuName = Const.GameNameMenu + "Audio/Clips Collection",
        order = 1)]
    public class ClipsCollection : ScriptableObject
    {
        public List<PairTrackID> pairs = new List<PairTrackID>();


        public AudioClip GetClip(string key)
        {
            return pairs
                .Where(pair => string.Equals(pair.id, key, StringComparison.CurrentCultureIgnoreCase))
                .Select(pair => pair.clip)
                .FirstOrDefault();
        }

        public bool TryGetClip(string key, out AudioClip clip)
        {
            return clip = GetClip(key);
        }
    }
}
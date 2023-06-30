using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu]
    public class IconsCollection : ScriptableObject
    {
        [Serializable]
        public class IconKey
        {
            public string key;
            public Sprite sprite;
        }

        public List<IconKey> iconKeys;
        public Sprite defaultSprite;

        public Sprite FindSprite(string key)
        {
            return (iconKeys.FirstOrDefault(iconKey => key == iconKey.key)?.sprite) ?? defaultSprite;
        }

        public int GetKeyIndex(string value)
        {
            for (var i = 0; i < iconKeys.Count; i++)
            {
                if (iconKeys[i].key == value) return i;
            }

            return -1;
        }

        public string[] Keys()
        {
            return iconKeys.Select(iconKey => iconKey.key).ToArray();
        }

        public string GetKeyByIndex(int index)
        {
            if (iconKeys.Count > index && index >= 0)
            {
                return iconKeys[index].key;
            }

            return "";
        }

        public IEnumerable<Sprite> Sprites()
        {
            return iconKeys.Select(iconKey => iconKey.sprite);
        }
    }
}
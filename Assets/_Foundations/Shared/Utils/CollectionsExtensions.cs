using System.Linq;
using UnityEngine;

namespace Shared.Utils
{
    public static class CollectionsExtensions
    {
        public static void DestroyChildren(this GameObject go, params GameObject[] exceptions)
        {
            if (!go || !go.transform) return;
            var objects = go
                .GetComponentsInChildren<Transform>()
                .Where(a => a)
                .Where(a => a.transform)
                .Select(g => g.gameObject)
                .Where(g => !exceptions.Contains(g))
                .Where(g => g != go);

            foreach (var gameObject in objects) SmartDestroy(gameObject);
        }

        public static void SmartDestroy(this Object obj)
        {
            if (!obj) return;
            if (Application.isPlaying)
            {
                Object.Destroy(obj);
            }
            else
            {
                Object.DestroyImmediate(obj);
            }
        }
    }
}
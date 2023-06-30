using UnityEngine;

namespace TargetPlatformTools
{
    

    public class TargetSelector : MonoBehaviour
    {
        public int targets;

        private void Awake()
        {
            if (ShouldDestroy())
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        private bool ShouldDestroy() => ((int) CalculateCurrentPlatform() & targets) == 0;

        private TargetPlatform CalculateCurrentPlatform()
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            // because it will change on different builds
            if (TargetPlatformUtility.IsAndroid) return TargetPlatform.Android;
            if (TargetPlatformUtility.IsWebGl) return TargetPlatform.WebGL;
            if (TargetPlatformUtility.IsIOS) return TargetPlatform.IOS;
            return TargetPlatform.UnityEditor;
        }
    }
    public enum TargetPlatform
    {
        UnityEditor = 0b0001,
        Android = 0b0010,
        IOS = 0b0100,
        WebGL = 0b1000,
    }
}
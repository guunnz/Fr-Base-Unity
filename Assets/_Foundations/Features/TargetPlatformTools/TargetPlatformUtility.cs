namespace TargetPlatformTools
{
    public static class TargetPlatformUtility
    {
        public const bool IsIOS =
#if UNITY_IOS
            true;
#else
            false;
#endif

        public const bool IsAndroid =
#if UNITY_ANDROID
            true;
#else
        false;
#endif

        public const bool IsWebGl =
#if UNITY_WEB_GL
        true;
#else
            false;
#endif
    }
}
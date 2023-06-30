using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

[assembly: Preserve]
namespace Adinmo
{
    public class AdinmoNativeWrapper
    {

#if !UNITY_EDITOR && UNITY_ANDROID
        [DllImport("AdinmoAndroidWebPlugin")]
        [RuntimeInitializeOnLoadMethod]
        public static extern System.IntPtr updateTexture();
#endif
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        [Preserve]
        public static extern System.IntPtr AllocWebWrapper(System.IntPtr nativeTexturePointer, System.IntPtr webBitmap,int textureWidth, int textureHeight, string backgroundColour, int borderMode);

        [DllImport("__Internal")]
        [Preserve]
        public static extern void ReleaseWebWrapper(System.IntPtr webWrapper);

        [DllImport("__Internal")]
        [Preserve]
        public static extern void LoadUrl(System.IntPtr webWrapper, string url);

        [DllImport("__Internal")]
        [Preserve]
        public static extern void LoadHtml(System.IntPtr webWrapper, string html);

        [DllImport("__Internal")]
        [Preserve]
        public static extern bool GetFrame(System.IntPtr webWrapper);

        [DllImport("__Internal")]
        [Preserve]
        public static extern void RequestUserAgent(string gameObjectName, string callbackMethod);

        [DllImport("__Internal")]
        [Preserve]
        public static extern bool CalculateHash(System.IntPtr webWrapper);

        [DllImport("__Internal")]
        [Preserve]
        public static extern bool GetNavigationComplete(System.IntPtr webWrapper);

        [DllImport("__Internal")]
        [Preserve]
        public static extern bool UpToDate(System.IntPtr webWrapper);

        [DllImport("__Internal")]
        [Preserve]
        public static extern bool GetContentHasLoaded(System.IntPtr webWrapper);

        [DllImport("__Internal")]
        [Preserve]
        public static extern System.IntPtr AllocWebBitmap(string guid, int width, int height, string adinmoManagerName);

        [DllImport("__Internal")]
        [Preserve]
        public static extern void ReleaseWebBitmap(System.IntPtr webWrapper);

        [DllImport("__Internal")]
        [Preserve]
        public static extern bool CopyWebpage(System.IntPtr webBitmap);




#endif
    }
}
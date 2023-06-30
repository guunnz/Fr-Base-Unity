#if UNITY_IOS
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Unity.Usercentrics
{
    internal class UsercentricsIOS: IUsercentricsPlatform
    {
        [DllImport("__Internal")]
        private static extern void initCMP(string initArgsJson);
    
        [DllImport("__Internal")]
        private static extern void showFirstLayer(string rawLayout);

        [DllImport("__Internal")]
        private static extern void showSecondLayer(bool showCloseButton);
    
        [DllImport("__Internal")]
        private static extern string getControllerId();

        [DllImport("__Internal")]
        private static extern void getTCFData();

        [DllImport("__Internal")]
        private static extern void restoreUserSession(string controllerId);

        [DllImport("__Internal")]
        private static extern void reset();

        [DllImport("__Internal")]
        private static extern void subscribeOnConsentUpdated();

        [DllImport("__Internal")]
        private static extern void disposeOnConsentUpdatedSubscription();

        public void Initialize(string initArgsJson)
        {
            initCMP(initArgsJson);
        }

        public void ShowFirstLayer(string rawLayout)
        {
            showFirstLayer(rawLayout);
        }

        public void ShowSecondLayer(bool showCloseButton)
        {
            showSecondLayer(showCloseButton);
        }

        public string GetControllerID()
        {
            return getControllerId();
        }

        public void GetTCFData()
        {
            getTCFData();
        }

        public void RestoreUserSession(string controllerId)
        {
            restoreUserSession(controllerId);
        }

        public void Reset()
        {
            reset();
        }

        public void SubscribeOnConsentUpdated()
        {
            subscribeOnConsentUpdated();
        }

        public void DisposeOnConsentUpdatedSubscription()
        {
            disposeOnConsentUpdatedSubscription();
        }
    }
}
#endif

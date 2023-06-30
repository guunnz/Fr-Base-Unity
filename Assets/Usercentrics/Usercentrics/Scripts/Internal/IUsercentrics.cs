using System.Collections.Generic;

namespace Unity.Usercentrics
{
    internal interface IUsercentricsPlatform
    {
        void Initialize(string initArgsJson);
        void ShowFirstLayer(string rawLayout);
        void ShowSecondLayer(bool showCloseButton);
        string GetControllerID();
        void GetTCFData();
        void RestoreUserSession(string controllerId);
        void Reset();
        void SubscribeOnConsentUpdated();
        void DisposeOnConsentUpdatedSubscription();
    }
}

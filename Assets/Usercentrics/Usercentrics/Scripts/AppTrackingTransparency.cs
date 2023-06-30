using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AppTrackingTransparency : Singleton<AppTrackingTransparency>
{
    #region INSPECTOR FIELDS
    public string PopupMessage = "";
    public static string m_ATTMessage;

    public bool IsATTEnabled = false;
    public static bool m_ATTEnabled;
    #endregion

    private void OnValidate()
    {
        m_ATTMessage = PopupMessage;
        m_ATTEnabled = IsATTEnabled;
    }

    /// <summary>
    /// Show Apple Tracking Transparency Popup if needed,
    /// if the user has already interacted with, it will not display anything,
    /// just return the previous input.
    /// </summary>
    /// <param name="attCallback">
    /// Callback block that is invoked when the user interacts with the popup.
    /// </param>
    public void PromptForAppTrackingTransparency(UnityAction<AuthorizationStatus> attCallback)
    {

        #if UNITY_IOS
        if (IsATTEnabled)
        {
            AppTrackingTransparencyManager.AttCallback = attCallback;
            AppTrackingTransparencyManager.RequestForAppTrackingTransparency();
        }
        #else
        Debug.Log("AppTrackingTransparency Not supported for this platform.");
        #endif

    }
}
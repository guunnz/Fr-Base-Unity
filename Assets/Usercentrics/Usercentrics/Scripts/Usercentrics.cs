using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.ComponentModel;

namespace Unity.Usercentrics
{
    /// <summary>
    /// Usercentrics Consent Management Platform (c) 2022.
    /// It gives access to the singleton (Instance) and the whole API.
    /// </summary>
    public class Usercentrics : Singleton<Usercentrics>
    {
        #region INSPECTOR FIELDS
        [SerializeField] public string SettingsID = "";
        [SerializeField] public UsercentricsOptions Options = new UsercentricsOptions();
#if UNITY_EDITOR
        [InspectorLink] [SerializeField] internal string AdminInterface = "https://admin.usercentrics.com";
        [InspectorLink] [SerializeField] internal string Documentation = "https://docs.usercentrics.com/cmp_in_app_sdk/latest/";
#endif
        #endregion

        private readonly IUsercentricsPlatform UsercentricsPlatform =
#if UNITY_IOS
            new UsercentricsIOS();
#elif UNITY_ANDROID
            new UsercentricsAndroid();
#else
            null;
#endif

        private UnityAction<UsercentricsConsentUserResponse> onDismissCallback;
        private UnityAction<UsercentricsReadyStatus> initializeCallback;
        private UnityAction<string> initializeErrorCallback;
        private UnityAction<TCFData> tcfDataCallback;
        private List<UnityAction<UsercentricsUpdatedConsentEvent>> onConsentUpdatedCallbacks = new List<UnityAction<UsercentricsUpdatedConsentEvent>>();

        private UnityAction<UsercentricsReadyStatus> restoreSessionSuccessCallback;
        private UnityAction<string> restoreSessionErrorCallback;

        #region USERCENTRICS API
        /// <summary>
        /// Get if Usercentrics was initialized or not.
        /// </summary>
        /// <returns>true if it was initialized, false otherwise.</returns>
        public bool IsInitialized
        { get; private set; }

        /// <summary>
        /// Initialize Usercentrics obtaining latest online configuration and
        /// local storage to enable consents management.
        /// </summary>
        /// <param name="initializeCallback">
        /// Callback block that is invoked when the initialize process finishes.
        /// It returns UsercentricsReadyStatus.
        /// </param>
        /// <param name="initializeErrorCallback">
        /// Callback block that is invoked when the initialize process finishes
        /// with an error.
        /// It returns a non-localized string with information about the error.
        /// </param>
        public void Initialize(
            UnityAction<UsercentricsReadyStatus> initializeCallback,
            UnityAction<string> initializeErrorCallback
        ){
            ensureSupportedPlatform();
            logDebug("Initialize invoked");

            this.initializeCallback = initializeCallback;
            this.initializeErrorCallback = initializeErrorCallback;

            var optionsInternal = UsercentricsOptionsInternal.CreateFrom(Options, SettingsID);
            string optionsJson = JsonUtility.ToJson(optionsInternal);
            UsercentricsPlatform?.Initialize(optionsJson);
        }

        /// <summary>
        /// Get if current selected platform is supported by Usercentrics.
        /// </summary>
        /// <returns>true if it is supported, false otherwise.</returns>
        public bool IsPlatformSupported()
        {
            return UsercentricsPlatform != null;
        }

        /// <summary>
        /// Show Usercentrics Banner's First Layer.
        /// </summary>
        /// <param name="layout">
        /// Banner predefined layout type, check UsercentricsLayout enum.
        /// </param>
        /// <param name="onDismissCallback">
        /// Callback block that is invoked when the user performs an action on the Banner.
        /// </param>
        public void ShowFirstLayer(UsercentricsLayout layout, UnityAction<UsercentricsConsentUserResponse> onDismissCallback)
        {
            ensureSupportedPlatform();
            logDebug("ShowFirstLayer invoked");
            ensureInitialized();
            this.onDismissCallback = onDismissCallback;
            UsercentricsPlatform?.ShowFirstLayer(layout.ToString());
        }

        /// <summary>
        /// Show Usercentrics Banner's First Layer.
        /// </summary>
        /// <param name="showCloseButton">
        /// Boolean flag to either show or hide close button located at the top of the screen.
        /// </param>
        /// <param name="onDismissCallback">
        /// Callback block that is invoked when the user performs an action on the Banner.
        /// </param>
        public void ShowSecondLayer(bool showCloseButton, UnityAction<UsercentricsConsentUserResponse> onDismissCallback)
        {
            ensureSupportedPlatform();
            logDebug("ShowSecondLayer invoked");
            ensureInitialized();
            this.onDismissCallback = onDismissCallback;
            UsercentricsPlatform?.ShowSecondLayer(showCloseButton);
        }

        /// <summary>
        /// Get the controller ID of the current session.
        /// </summary>
        /// <returns>the controller ID string</returns>
        public string GetControllerID()
        {
            ensureSupportedPlatform();
            logDebug("GetControllerID Invoked");
            ensureInitialized();
            return UsercentricsPlatform?.GetControllerID();
        }

        /// <summary>
        /// Retrieve TCF Data
        /// </summary>
        /// <returns>all data related to TCF, in case the SDK is using a TCF framework configuration</returns>
        public void GetTCFData(UnityAction<TCFData> callback)
        {
            ensureSupportedPlatform();
            logDebug("GetTCFData invoked");
            this.tcfDataCallback = callback;
            UsercentricsPlatform?.GetTCFData();
        }

        /// <summary>
        /// Restore Consents given by a user using its Controller ID.
        /// </summary>
        /// <param name="controllerId">
        /// User's Controller Id string.
        /// </param>
        /// <param name="successCallback">
        /// Callback block that is invoked when the restoration completes successfully.
        /// It returns UsercentricsReadyStatus.
        /// </param>
        /// <param name="initializeErrorCallback">
        /// Callback block that is invoked when the initialize process finishes
        /// with an error.
        /// It returns a non-localized string with information about the error.
        /// </param>
        public void RestoreUserSession(
            string controllerId,
            UnityAction<UsercentricsReadyStatus> successCallback,
            UnityAction<string> errorCallback
        ){
            ensureSupportedPlatform();
            logDebug("RestoreUserSession invoked");

            this.restoreSessionSuccessCallback = successCallback;
            this.restoreSessionErrorCallback = errorCallback;

            UsercentricsPlatform?.RestoreUserSession(controllerId);
        }

        /// <summary>
        /// Resets Usercentrics SDK, deleting all local data and forcing to be initialized again.
        /// </summary>
        public void Reset()
        {
            ensureSupportedPlatform();
            logDebug("Reset invoked");
            IsInitialized = false;
            UsercentricsPlatform?.Reset();
        }

        /// <summary>
        /// Subscribe to any consent updated event that happens within Usercentrics SDK
        /// </summary>
        public void SubscribeOnConsentUpdated(UnityAction<UsercentricsUpdatedConsentEvent> callback)
        {
            ensureSupportedPlatform();
            logDebug("SubscribeOnConsentUpdated invoked");
            if (onConsentUpdatedCallbacks.Count == 0)
            {
                UsercentricsPlatform?.SubscribeOnConsentUpdated();
            }
            this.onConsentUpdatedCallbacks.Add(callback);
        }

        /// <summary>
        /// Dispose all callbacks related to OnConsentUpdated event
        /// </summary>
        public void DisposeOnConsentUpdatedSubscription()
        {
            ensureSupportedPlatform();
            logDebug("DisposeOnConsentUpdatedSubscription invoked");
            this.onConsentUpdatedCallbacks.Clear();
            UsercentricsPlatform?.DisposeOnConsentUpdatedSubscription();
        }
        #endregion

        #region UTILS
        private void ensureInitialized()
        {
            if (!IsInitialized)
            {
                throw new NotInitializedException();
            }
        }

        private void ensureSupportedPlatform()
        {
            ShowEditorNotSupportedDialogIfNeeded();

            if (!IsPlatformSupported())
            {
                throw new PlatformSupportException();
            }
        }

        private void ShowEditorNotSupportedDialogIfNeeded()
        {
            #if UNITY_EDITOR
            bool openRequirements = EditorUtility.DisplayDialog("Unity Editor not supported", UCConstants.UNITY_EDITOR_MESSAGE, "See Documentation", "Close");
            if (openRequirements)
            {
                Help.BrowseURL(UCConstants.REQUIREMENTS_URL);
            }
            #endif
        }

        private void logDebug(string message)
        {
            if (Options.DebugMode)
            {
                Debug.Log("[USERCENTRICS][DEBUG] " + message);
            }
        }
        #endregion

        #region MESSAGES HANDLERS
#pragma warning disable IDE0051 // Remove unused private members
        internal void HandleInitSuccess(string rawUsercentricsReadyStatus)
        {
            logDebug("HandleInitSuccess UsercentricsReadyStatus=" + rawUsercentricsReadyStatus);
            var usercentricsReadyStatus = JsonUtility.FromJson<UsercentricsReadyStatus>(rawUsercentricsReadyStatus);
            IsInitialized = true;
            initializeCallback?.Invoke(usercentricsReadyStatus);
            initializeCallback = null;
        }

        internal void HandleInitError(string errorMessage)
        {
            logDebug("HandleInitError errorMessage=" + errorMessage);
            initializeErrorCallback?.Invoke(errorMessage);
            initializeErrorCallback = null;
        }

        internal void HandleBannerResponse(string rawUsercentricsConsentUserResponse)
        {
            logDebug("HandleBannerResponse UsercentricsConsentUserResponse=" + rawUsercentricsConsentUserResponse);
            var usercentricsConsentUserResponse = JsonUtility.FromJson<UsercentricsConsentUserResponse>(rawUsercentricsConsentUserResponse);
            onDismissCallback?.Invoke(usercentricsConsentUserResponse);
            onDismissCallback = null;
        }

        internal void HandleRestoreSuccess(string rawUsercentricsReadyStatus)
        {
            logDebug("HandleRestoreSuccess UsercentricsReadyStatus=" + rawUsercentricsReadyStatus);
            var usercentricsReadyStatus = JsonUtility.FromJson<UsercentricsReadyStatus>(rawUsercentricsReadyStatus);
            restoreSessionSuccessCallback?.Invoke(usercentricsReadyStatus);
            restoreSessionSuccessCallback = null;
        }

        internal void HandleRestoreError(string errorMessage)
        {
            logDebug("HandleRestoreError errorMessage=" + errorMessage);
            restoreSessionErrorCallback?.Invoke(errorMessage);
            restoreSessionErrorCallback = null;
        }

        internal void HandleTCFData(string rawTCFData)
        {
            logDebug("HandleTCFData tcfData=" + rawTCFData);
            var tcfData = JsonUtility.FromJson<TCFData>(rawTCFData);
            tcfDataCallback?.Invoke(tcfData);
            tcfDataCallback = null;
        }

        internal void HandleOnConsentUpdated(string rawUsercentricsUpdatedConsentEvent)
        {
            logDebug("HandleOnConsentUpdated usercentricsUpdatedConsentEvent=" + rawUsercentricsUpdatedConsentEvent);
            var usercentricsUpdatedConsentEvent = JsonUtility.FromJson<UsercentricsUpdatedConsentEvent>(rawUsercentricsUpdatedConsentEvent);
            onConsentUpdatedCallbacks?.ForEach(callback => callback.Invoke(usercentricsUpdatedConsentEvent));
        }
#pragma warning restore IDE0051 // Remove unused private members
        #endregion
    }
}

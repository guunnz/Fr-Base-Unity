using System;
using System.Collections.Generic;

namespace Unity.Usercentrics
{
    #region UNITY LOCAL

    [Serializable]
    public class UsercentricsOptions
    {
        public string DefaultLanguage = "";
        public string Version = "latest";
        public bool DebugMode = true;
        public long TimeoutMillis = 10000L;
    }
    #endregion

    [Serializable]
    public class UsercentricsOptionsInternal
    {
        public string settingsId;
        public string defaultLanguage;
        public string version;
        public long timeoutMillis;
        public UsercentricsLoggerLevel loggerLevel;

        public static UsercentricsOptionsInternal CreateFrom(UsercentricsOptions unityOptions, string settingsId)
        {
            var userOptions = new UsercentricsOptionsInternal();
            userOptions.settingsId = settingsId;
            userOptions.defaultLanguage = unityOptions.DefaultLanguage;
            userOptions.version = unityOptions.Version;
            userOptions.timeoutMillis = unityOptions.TimeoutMillis;
            userOptions.loggerLevel = unityOptions.DebugMode ? UsercentricsLoggerLevel.Debug : UsercentricsLoggerLevel.None;
            return userOptions;
        }
    }

    [Serializable]
    public enum UsercentricsLoggerLevel
    {
        None,
        Error,
        Warning,
        Debug
    }

    [Serializable]
    public class UsercentricsReadyStatus
    {
        public bool shouldCollectConsent;
        public List<UsercentricsServiceConsent> consents;
    }

    [Serializable]
    public class UsercentricsServiceConsent
    {
        public string templateId;
        public bool status;
        public List<UsercentricsConsentHistoryEntry> history;
        public UsercentricsConsentType? type;
        public string dataProcessor;
        public string version;
        public bool isEssential;
    }

    [Serializable]
    public class UsercentricsConsentHistoryEntry
    {
        public bool status;
        public UsercentricsConsentType type;
        public long timestampInMillis;
    }

    [Serializable]
    public enum UsercentricsConsentType
    {
        Explicit,
        Implicit
    }

    [Serializable]
    public enum UsercentricsLayout
    {
        Full,
        Sheet,
        PopupBottom,
        PopupCenter
    }

    [Serializable]
    public class UsercentricsConsentUserResponse
    {
        public UsercentricsUserInteraction userInteraction;
        public List<UsercentricsServiceConsent> consents;
        public string controllerId;
    }

    [Serializable]
    public enum UsercentricsUserInteraction
    {
        AcceptAll,
        DenyAll,
        Granular,
        NoInteraction
    }

    [Serializable]
    public class TCFData
    {
        public List<TCFFeature> features;
        public List<TCFPurpose> purposes;
        public List<TCFSpecialFeature> specialFeatures;
        public List<TCFSpecialPurpose> specialPurposes;
        public List<TCFStack> stacks;
        public List<TCFVendor> vendors;
        public string tcString;
    }

    [Serializable]
    public class TCFFeature
    {
        public string purposeDescription;
        public string descriptionLegal;
        public int id;
        public string name;
    }

    [Serializable]
    public class TCFPurpose
    {
        public string purposeDescription;
        public string descriptionLegal;
        public int id;
        public string name;
        public bool? consent;
        public bool isPartOfASelectedStack;
        public bool? legitimateInterestConsent;
        public bool showConsentToggle;
        public bool showLegitimateInterestToggle;
        public int? stackId;
    }

    [Serializable]
    public class TCFSpecialFeature
    {
        public string purposeDescription;
        public string descriptionLegal;
        public int id;
        public string name;
        public bool? consent;
        public bool isPartOfASelectedStack;
        public int? stackId;
        public bool showConsentToggle;
    }

    [Serializable]
    public class TCFSpecialPurpose
    {
        public string purposeDescription;
        public string descriptionLegal;
        public int id;
        public string name;
    }

    [Serializable]
    public class TCFStack
    {
        public string description;
        public int id;
        public string name;
        public List<int> purposeIds;
        public List<int> specialFeatureIds;
    }

    [Serializable]
    public class TCFVendor
    {
        public bool? consent;
        public List<IdAndName> features;
        public List<IdAndName> flexiblePurposes;
        public int id;
        public bool? legitimateInterestConsent;
        public List<IdAndName> legitimateInterestPurposes;
        public string name;
        public string policyUrl;
        public List<IdAndName> purposes;
        public List<TCFVendorRestriction> restrictions;
        public List<IdAndName> specialFeatures;
        public List<IdAndName> specialPurposes;
        public bool showConsentToggle;
        public bool showLegitimateInterestToggle;
        public double? cookieMaxAgeSeconds = null;
        public bool usesNonCookieAccess;
        public string deviceStorageDisclosureUrl = null;
        public ConsentDisclosureObject deviceStorage = null;
        public bool usesCookies = false;
        public bool? cookieRefresh = false;
        public bool? dataSharedOutsideEU = false;
    }

    [Serializable]
    public class IdAndName
    {
        public int id;
        public string name;
    }

    [Serializable]
    public class TCFVendorRestriction
    {
        public int purposeId;
        public RestrictionType restrictionType;
    }

    [Serializable]
    public enum RestrictionType
    {
        NotAllowed,
        RequireConsent,
        RequireLi
    }

    [Serializable]
    public class ConsentDisclosureObject
    {
        public List<ConsentDisclosure> disclosures = new List<ConsentDisclosure>();
    }

    [Serializable]
    public class ConsentDisclosure
    {
        public string identifier = null;
        public ConsentDisclosureType? type = null;
        public string name = null;
        public long? maxAgeSeconds = null;
        public bool cookieRefresh = false;
        public List<int> purposes = new List<int>();
        public string domain = null;
        public string description = null;
    }

    [Serializable]
    public enum ConsentDisclosureType
    {
        Cookie,
        Web,
        App
    }

    [Serializable]
    public class UsercentricsUpdatedConsentEvent
    {
        public List<UsercentricsServiceConsent> consents;
        public string controllerId;
        public string tcString = null;
        public string uspString = null;
    }
}

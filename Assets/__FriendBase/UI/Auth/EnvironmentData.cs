using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using System;
using Unity.RemoteConfig;
//https://app.remote-config.unity3d.com/configs/618075ea-40e2-43f8-9105-5094b3f851c1?environmentId=1e673281-dae7-463a-a552-c70f0d592e09&projectId=c032c517-1627-4f80-9973-9d54276a1b50
public class EnvironmentData
{
    public const bool FORCE_DEVELOP = true;
    public const bool IsPreProd = false;
    public const string PRODUCTION_ENVIRONMENT = "production";
    public const string DEVELOP_ENVIRONMENT = "develop";
    public const string REMOTE_KEY_MIN_VERSION_REQUIRED = "minVersionRequired";
    public const string REMOTE_KEY_DEVELOP_URL = "develop_url";
    public const string REMOTE_KEY_PRODUCTION_URL = "production_url";
    public const string REMOTE_KEY_ID_ROOM_LOGIN_DEV = "idRoomToJumpAfterLoginDev";
    public const string REMOTE_KEY_ID_ROOM_LOGIN_PRODUCTION = "idRoomToJumpAfterLoginProduction";

    private List<Version> productionBuildVersions;
    private string minVersionRequired;
    public string ProductionURL { get; private set; }
    public string DevelopURL { get; private set; }
    public struct userAttributes { }
    public struct appAttributes { }
    private bool fetchingReady;
    public static bool IsOnProduction { get; private set; }
    private static int idRoomToJumpAfterLoginDev;
    private static int idRoomToJumpAfterLoginProduction;

    public EnvironmentData()
    {
        ConfigManager.FetchCompleted += RemoteConfigUpdate;
    }
    void SetDefaultValues()
    {
        productionBuildVersions = new List<Version>();
        //productionBuildVersions.Add(new Version("0.0.100"));
        minVersionRequired = "0.0.100";
        ProductionURL = "friendbase-staging.fly.dev";
        DevelopURL = "friendbase-staging.fly.dev";
    }
    public async Task<bool> ReadData()
    {
        await Task.Yield();
        //await Task.WhenAll( ArrayList );
        //await Task.Delay(1000);
        fetchingReady = false;
        ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
        while (!fetchingReady)
        {
            await Task.Yield();
        }
        return true;
    }
    void RemoteConfigUpdate(ConfigResponse response)
    {
        Debug.Log(response.status + "<----- RESPONSE REMOTE CONFIG ");
        if (response.status == ConfigRequestStatus.Success)
        {
            minVersionRequired = ConfigManager.appConfig.GetString(REMOTE_KEY_MIN_VERSION_REQUIRED);
            ProductionURL = ConfigManager.appConfig.GetString(REMOTE_KEY_PRODUCTION_URL);
            DevelopURL = ConfigManager.appConfig.GetString(REMOTE_KEY_DEVELOP_URL);
            //Read prop in remote config (If current version is added as true => it is marked as production)
            IsOnProduction = ConfigManager.appConfig.GetBool(Application.version);
            idRoomToJumpAfterLoginDev = ConfigManager.appConfig.GetInt(REMOTE_KEY_ID_ROOM_LOGIN_DEV);
            idRoomToJumpAfterLoginProduction = ConfigManager.appConfig.GetInt(REMOTE_KEY_ID_ROOM_LOGIN_PRODUCTION);
        }
        else
        {
            minVersionRequired = "3.9.1";
            ProductionURL = "friendbase-prod.fly.dev";
            DevelopURL = "friendbase-staging.fly.dev";
            IsOnProduction = true;
            idRoomToJumpAfterLoginDev = 405;
            idRoomToJumpAfterLoginProduction = 27820;
        }

        //Debug.Log("********* minRequiredVersion:" + minVersionRequired);
        //Debug.Log("********* Application.version:" + Application.version);
        //Debug.Log("********* isProduction:" + IsProduction());
        fetchingReady = true;
    }
    public bool IsValidBuildVersion()
    {
        Version currentVersion = new Version(Application.version);
        Version minVersion = new Version(minVersionRequired);
        int result = currentVersion.CompareTo(minVersion);
        return result >= 0;
    }
    public static bool IsProduction()
    {
        IsOnProduction = !FORCE_DEVELOP;
        return IsOnProduction;

        //Double check to avoid humans error (Force production if the app is intalled from Google Play or App Store)
        if (Application.installMode == ApplicationInstallMode.Store)
        {
            IsOnProduction = true;
            return true;
        }
        return IsOnProduction;
    }
    public string GetCurrentEnvironment()
    {
        return IsProduction() ? PRODUCTION_ENVIRONMENT : DEVELOP_ENVIRONMENT;
    }
    public string GetBackEndURL()
    {
        if (IsProduction())
        {
            return ProductionURL;
        }
        else
        {
            return DevelopURL;
        }
    }

    public static int GetPublicIdRoomToJumpAfterLogin()
    {
        if (IsProduction())
        {
            return idRoomToJumpAfterLoginProduction;
        }
        else
        {
            return idRoomToJumpAfterLoginDev;
        }
    }

    public string GetGoogleSingInKey()
    {
        if (IsProduction())
        {
            return "534255011920-mj139mcv411k1cg24cafk78lb19mhrmg.apps.googleusercontent.com";
        }
        else
        {
            return "1045385706868-0l0p89r0o4c8ro6kqnt43e2ali9loo1n.apps.googleusercontent.com";
        }
    }
    public string GetFirebaseCertificates()
    {
        if (IsProduction())
        {
#if UNITY_ANDROID
            return Resources.Load("FirebaseCertificates/certificateProductionAndroid").ToString();
#endif
#if (UNITY_IPHONE || UNITY_IOS)
            return Resources.Load("FirebaseCertificates/certificateProductionIos").ToString();
#endif
        }
        else
        {
#if UNITY_ANDROID
            return Resources.Load("FirebaseCertificates/certificateDevelopAndroid").ToString();
#endif
#if (UNITY_IPHONE || UNITY_IOS)
            return Resources.Load("FirebaseCertificates/certificateDevelopIos").ToString();
#endif
        }
        return Resources.Load("FirebaseCertificates/certificateProductionAndroid").ToString();
    }
    public void Destroy()
    {
        ConfigManager.FetchCompleted -= RemoteConfigUpdate;
    }
}
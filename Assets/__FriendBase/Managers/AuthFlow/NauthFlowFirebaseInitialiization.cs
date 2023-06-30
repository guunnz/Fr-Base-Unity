using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DebugConsole;
using UnityEngine;
using Firebase;
using LocalStorage.Core;
using Architecture.Injector.Core;
using Firebase.Analytics;
using Firebase.Auth;

public class NauthFlowFirebaseInitialiization
{
    public enum FirebaseCompleteStatus { SUCCEED, FAILED, UPDATE_GAME}
    public const string LAST_ENVIRONMENT_KEY = "lastEnvironmentKey";

    private IDebugConsole debugConsole;
    private EnvironmentData environmentData;

    public delegate void FirebaseInitializationComplete(FirebaseCompleteStatus status);
    public event FirebaseInitializationComplete OnFirebaseInitializationComplete;
    private MonoBehaviour monoBehaviourForCoroutine;
    private ILocalStorage localStorage;

    public static bool firebaseInitReady = false;
    private IAnalyticsSender analyticsSender;

    public NauthFlowFirebaseInitialiization(MonoBehaviour monoBehaviour)
    {
        monoBehaviourForCoroutine = monoBehaviour;
        localStorage = Injection.Get<ILocalStorage>();
        debugConsole = Injection.Get<IDebugConsole>();
        analyticsSender = Injection.Get<IAnalyticsSender>();
    }

    public void Init()
    {
        ReadEnvironmentData();
    }

    public void Destroy()
    {

    }

    async Task ReadEnvironmentData()
    {
        environmentData = new EnvironmentData();
        //Read Remote Config Data
        await environmentData.ReadData();

        if (EnvironmentData.IsProduction())
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Environment Production");
            CheckMinRequiredVersion();
        }
        else
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "Environment Develop");
            CreateDevelopFirebaseApp();
        }
    }

    void CheckMinRequiredVersion()
    {
        if (environmentData.IsValidBuildVersion())
        {
            monoBehaviourForCoroutine.StartCoroutine(CreateProductionFirebaseApp());
        }
        else
        {
            if (OnFirebaseInitializationComplete != null)
            {
                OnFirebaseInitializationComplete(FirebaseCompleteStatus.UPDATE_GAME);
            }
        }
    }

    void CreateDevelopFirebaseApp()
    {
        //Get Firebase Certificates and Create App
        if (!firebaseInitReady)
        {
            AppOptions options = AppOptions.LoadFromJsonConfig(environmentData.GetFirebaseCertificates());
            FirebaseApp.Create(options);
            firebaseInitReady = true;
        }
        CheckEnvironment();
    }

    IEnumerator CreateProductionFirebaseApp()
    {
        yield return new WaitForEndOfFrame();

        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "TRY CREATING FIREBASE PROD");

        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowInitFirebase);

        if (!firebaseInitReady)
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                DependencyStatus _dependencyStatus = task.Result;
                if (_dependencyStatus == DependencyStatus.Available)
                {
                    FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                    if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "FIREBASE INITIALIZE OK " + app.Name + " - " + app.Options.AppId);
                    firebaseInitReady = true;
                    analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowInitFirebaseSuccedd);
                }
                else
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowInitFirebaseError);
                    if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "FIREBASE FAILED");
                }
            });
        }

        while (!firebaseInitReady)
        {
            yield return new WaitForEndOfFrame();
        }

        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "FIREBASE PROD SUCCEED");

        CheckEnvironment();
    }

    void CheckEnvironment()
    {
        Constants.SetHostname(environmentData.GetBackEndURL());

        FirebaseAnalytics.LogEvent("custom_firebase_init"); //Test Log on Firebase

        string lastEnvironment = localStorage.GetString(LAST_ENVIRONMENT_KEY);
        string currentEnvironment = environmentData.GetCurrentEnvironment();

        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "lastEnvironment: " + lastEnvironment);
        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "currentEnvironment: " + currentEnvironment);
        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "current mail: " + FirebaseAuth.DefaultInstance.CurrentUser?.Email);
        if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "current firebase id: " + FirebaseAuth.DefaultInstance.CurrentUser?.UserId);

        if (!lastEnvironment.Equals(currentEnvironment))
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "DIFF ENV -> CLEAR");
            //Clear Data and Cache
            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "USER MAIL 1: " + FirebaseAuth.DefaultInstance.CurrentUser?.Email);
            FirebaseAuth.DefaultInstance.SignOut();
            localStorage.SetString(LAST_ENVIRONMENT_KEY, currentEnvironment);

            //We need this to clear the cache of the Firebase User after the logout (If not it is like the logout does not work)
            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "USER MAIL 2: " + FirebaseAuth.DefaultInstance.CurrentUser?.Email);
        }
        else
        {
            if (debugConsole.isLogTypeEnable(LOG_TYPE.AUTH_FLOW)) debugConsole.TraceLog(LOG_TYPE.AUTH_FLOW, "SAME ENV");
        }

        if (OnFirebaseInitializationComplete != null)
        {
            OnFirebaseInitializationComplete(FirebaseCompleteStatus.SUCCEED);
        }
    }
}

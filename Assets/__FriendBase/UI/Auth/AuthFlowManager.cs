using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using AuthFlow;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.EndAuth.Repo;
using Data;
using Firebase.Auth;
using MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using System.Collections;
using System.Threading.Tasks;
using LocalStorage.Core;
using System.Collections.Generic;
using System.Linq;
using Firebase.DynamicLinks;
using Firebase.Analytics;

namespace UI.Auth
{
    public class AuthFlowManager : GenericSingleton<AuthFlowManager>
    {
        public const string LAST_ENVIRONMENT_KEY = "lastEnvironmentKey";
        public static bool firebaseInitReady = false;

        public UIAuthLandingView UILandingView;
        public UIAuthLegacyLandingView UILandingViewLegacy;

        public UIAuthEnterEmailView UIEnterEmailView;
        public UIAuthLegacyCheck UILegacyCheck;
        public UIAuthLoginView UILoginView;
        public UIAuthCheckEmailView UICheckEmailView;
        public UIAuthCheckEmailViewLegacy UICheckEmailLegacyView;
        public UIAuthSetNewPasswordView UISetNewPasswordView;
        public UIAuthTermsView UITermsView;
        public UIAuthAboutYouView UIAboutYouView;
        public UIAuthPostLegacyLogin UIPostLegacyLogin;

        public AbstractUIPanel viewCurrent;
        public AbstractUIPanel viewPrev;
        IAnalyticsSender analyticsSender;
        public CanvasGroup loadingPanel;
        public AuthModalError modalError;

        private ILocalUserInfo _userInfo;
        private IAboutYouStateManager _aboutYouState;

        private string _deeplinkURL;
        private EnvironmentData environmentData;
        private ILocalStorage localStorage;

        internal bool LegacyLogin = false;
        internal bool LegacyFacebookLogin = false;

        // Deep linking
        // More info: https://docs.unity3d.com/Manual/enabling-deep-linking.html

        protected override void Initialize()
        {
            //string installSource = Application.installerName;
            //string store = ApplicationInstallMode.Store; Application.installMode;
            //JesseUtils.Logout();
            localStorage = Injection.Get<ILocalStorage>();

            ReadEnvironmentData();
        }

        private void OnDisable()
        {
            DynamicLinks.DynamicLinkReceived -= OnDynamicLink;
        }

        // Display the dynamic link received by the application.
        void OnDynamicLink(object sender, EventArgs args)
        {
            var dynamicLinkEventArgs = args as ReceivedDynamicLinkEventArgs;
            Debug.LogFormat("Received dynamic link {0}",
                            dynamicLinkEventArgs.ReceivedDynamicLink.Url.OriginalString);
        }

        async Task ReadEnvironmentData()
        {
            SetLoading(true);
            environmentData = new EnvironmentData();
            //Read Remote Config Data
            await environmentData.ReadData();
            AuthProviderGoogle.SetWebClientId(environmentData.GetGoogleSingInKey());
            AuthLinkProviders.SetWebClientId(environmentData.GetGoogleSingInKey());

            if (EnvironmentData.IsProduction())
            {
                Debug.Log(" ----- Production Environment ");
                CheckMinRequiredVersion();
            }
            else
            {
                Debug.Log(" ----- Develop Environment ");
                CreateDevelopFirebaseApp();
            }
        }

        void CheckMinRequiredVersion()
        {
            if (environmentData.IsValidBuildVersion())
            {
                //CreateFirebaseApp();
                StartCoroutine(CreateProductionFirebaseApp());
            }
            else
            {
                SceneManager.UnloadSceneAsync(GameScenes.AuthFlow);
                SceneManager.LoadSceneAsync(GameScenes.UpdateGame, LoadSceneMode.Additive);
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

            Debug.Log("---- CreateFirebaseApp:" + firebaseInitReady);

            if (!firebaseInitReady)
            {
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    DependencyStatus _dependencyStatus = task.Result;
                    if (_dependencyStatus == DependencyStatus.Available)
                    {
                        FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                        Debug.Log("---- FIREBASE INITIALIZE OK 01 - " + app.Name + " - " + app.Options.AppId);


                        firebaseInitReady = true;
                        //FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    }
                    else
                    {
                        Debug.Log("***** FIREBASE FAILED");
                    }
                });
            }

            while (!firebaseInitReady)
            {
                yield return new WaitForEndOfFrame();
            }

            Debug.Log("***** FIREBASE SUCCEED");

            CheckEnvironment();
        }

        void CheckEnvironment()
        {
            Constants.SetHostname(environmentData.GetBackEndURL());

            Debug.Log("***** FIREBASE ANALYTICS LOG");
            FirebaseAnalytics.LogEvent("Custom Firebase succeed ANDROID MANIFEST");

            string lastEnvironment = localStorage.GetString(LAST_ENVIRONMENT_KEY);
            string currentEnvironment = environmentData.GetCurrentEnvironment();

            Debug.Log(" ----- lastEnvironment: " + lastEnvironment);
            Debug.Log(" ----- currentEnvironment: " + currentEnvironment);
            Debug.Log(" ----- current mail: " + FirebaseAuth.DefaultInstance.CurrentUser?.Email);
            Debug.Log(" ----- current firebase id: " + FirebaseAuth.DefaultInstance.CurrentUser?.UserId);

            if (!lastEnvironment.Equals(currentEnvironment))
            {
                Debug.Log(" ----- CLEAR");
                //Clear Data and Cache
                Debug.Log("--USER MAIL 1: " + FirebaseAuth.DefaultInstance.CurrentUser?.Email);
                FirebaseAuth.DefaultInstance.SignOut();
                localStorage.SetString(LAST_ENVIRONMENT_KEY, currentEnvironment);

                //We need this to clear the cache of the Firebase User after the logout (If not it is like the logout does not work)
                Debug.Log("--USER MAIL 2: " + FirebaseAuth.DefaultInstance.CurrentUser?.Email);

                InitializeDeepLink();
                InitializeAsync();
            }
            else
            {
                Debug.Log(" ----- LOGIN");
                InitializeDeepLink();
                InitializeAsync();
            }
        }

        void InitializeDeepLink()
        {
            DynamicLinks.DynamicLinkReceived += OnDynamicLink;
            Application.deepLinkActivated += OnDeepLinkActivated;
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                // Cold start and Application.absoluteURL not null so process Deep Link.
                OnDeepLinkActivated(Application.absoluteURL);
            }
            // Initialize DeepLink Manager global variable.
            else _deeplinkURL = "";
        }

        private void Start()
        {
            analyticsSender = Injection.Get<IAnalyticsSender>();
        }
        public void SetFacebookLegacyLoginUI()
        {
            LegacyFacebookLogin = true;
        }

        public void SetLegacyLogin()
        {
            CloseAll();
            LegacyLogin = true;
            UILandingViewLegacy.Open();
        }

        public void NormalLogin()
        {
            CloseAll();
            LegacyLogin = false;
            UILandingView.Open();
        }

        public void DoneLegacy()
        {
            CloseAll();
            LegacyLogin = false;
            UIPostLegacyLogin.Open();
        }

        private async void InitializeAsync()
        {
            _userInfo = Injection.Get<ILocalUserInfo>();
            _aboutYouState = Injection.Get<IAboutYouStateManager>();

            CloseAll();
            SetLoading(true);

            // Check if the user is already logged in
            string loginToken = await JesseUtils.IsUserLoggedIn();

            if (string.IsNullOrEmpty(loginToken))
            {
                _userInfo["firebase-login-token"] = "";

                // If the user is not logged in, show the landing view

                //if (PlayerPrefs.GetInt("LegacyCheck") == 0)
                //{
                //    UILegacyCheck.Open();
                //}
                //else
                //{
                //    UILandingView.Open();
                //}
                UILandingView.Open();
                SetLoading(false);
            }
            else
            {
                // If the user is logged in, try go to the main menu
                Finish();
            }


        }

        private void CloseAll()
        {
            UILandingView.Close();
            UIEnterEmailView.Close();
            UILoginView.Close();
            UILandingViewLegacy.Close();
            UICheckEmailView.Close();
            UISetNewPasswordView.Close();
            UITermsView.Close();
            UIAboutYouView.Close();
            UILegacyCheck.Close();
            UICheckEmailLegacyView.Close();
            UIPostLegacyLogin.Close();
        }

        public void SetView(AbstractUIPanel viewNext)
        {
            CloseAll();
            viewPrev = viewCurrent;
            viewCurrent = viewNext;
            viewNext.Open();
        }

        // Attempts to finish the authentication process
        public async void Finish(bool setTerms = false)
        {
            if (JesseUtils.HasProvider(AuthProvidersFirebase.APPLE))
            {
                Debug.Log("-----AuthFlowManager Finish APPLE");
                //Set empty as apple do not allow to ask name. And we can not get name because of legacy sdk
                if (!_aboutYouState.FirstName.HasValue)
                {
                    Debug.Log("-----AuthFlowManager Finish Name");
                    _aboutYouState.FirstName = "";
                }
                if (!_aboutYouState.LastName.HasValue)
                {
                    Debug.Log("-----AuthFlowManager Finish LastName");
                    _aboutYouState.LastName = "";
                }
            }

            // The user needs to verify his email address
            if (!FirebaseAuth.DefaultInstance.CurrentUser.IsEmailVerified)
            {
                Debug.Log("---------- IsEmailVerified FALSE");
                if (JesseUtils.HasProvider(AuthProvidersFirebase.PASSWORD))
                {
                    Debug.Log("---------- HasMailPasswordProvider TRUE");
                    CloseAll();
                    UICheckEmailView.Open();
                    SetLoading(false);
                    return;
                }
            }

            Debug.Log("-----AuthFlowManager Finish 02");
            if (setTerms)
            {
                if (JesseUtils.HasProvider(AuthProvidersFirebase.APPLE))
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.RegistrationWithApple);
                }
                else if (JesseUtils.HasProvider(AuthProvidersFirebase.FACEBOOK))
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.RegistrationWithFacebook);
                }

                Debug.Log("-----AuthFlowManager Finish 03");
                _userInfo["terms"] = "True";
            }

            Debug.Log("-----AuthFlowManager Finish 04");

            if (LegacyLogin && LegacyFacebookLogin)
            {
                Debug.Log("-----AuthFlowManager Finish Legacy");
                CloseAll();
                UIPostLegacyLogin.Open();
                LegacyLogin = false;
                SetLoading(false);
                return;
            }
            Debug.Log("-----AuthFlowManager Finish 05");

            // The user needs to accept the terms and conditions
            if (_userInfo["terms"] != "True")
            {
                LegacyLogin = false;
                Debug.Log("-----AuthFlowManager Finish Terms");
                CloseAll();
                UITermsView.Open();
                SetLoading(false);
                return;
            }

            Debug.Log("-----AuthFlowManager Finish 06");
            // The user must complete information on the About You screen
            if (!_aboutYouState.BirthDate.HasValue
                || !_aboutYouState.FirstName.HasValue
                || !_aboutYouState.LastName.HasValue
                || !_aboutYouState.UserName.HasValue
               )
            {
                Debug.Log("-----AuthFlowManager Finish 07");
                CloseAll();
                UIAboutYouView.Open();
                SetLoading(false);
                return;
            }

            //if (PlayerPrefs.GetInt("LegacyCheck") == 0)
            //{
            //    UILegacyCheck.Open();
            //    PlayerPrefs.SetInt("LegacyCheck", 1);
            //}
            Debug.Log("-----AuthFlowManager Finish 08");
            //SetLoading(false);
            LoadGame();
        }

        // Loads the game, assuming the user is already logged in
        private static void LoadGame()
        {
            IAnalyticsSender analyticsSender;

            analyticsSender = Injection.Get<IAnalyticsSender>();
            // The user is ready to play
            SceneManager.UnloadSceneAsync(GameScenes.AuthFlow);
            IGameData gameData = Injection.Get<IGameData>();
            if (gameData.GetUserInformation().Do_avatar_customization)
            {
                SceneManager.LoadSceneAsync(GameScenes.AvatarCustomization, LoadSceneMode.Additive);
            }
            else
            {
                if (JesseUtils.HasProvider(AuthProvidersFirebase.APPLE))
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.LoginWithApple);
                }
                else if (JesseUtils.HasProvider(AuthProvidersFirebase.FACEBOOK))
                {
                    analyticsSender.SendAnalytics(AnalyticsEvent.LoginWithFacebook);
                }
                //HideView();
                Injection.Get<IViewManager>().Show<MainMenuView>();
            }
        }

        // Show the loading panel
        public void SetLoading(bool isLoading)
        {
            if (isLoading)
            {
                Injection.Get<ILoading>().Load();
            }
            else
            {
                Debug.LogError("UNLOAD");
                Injection.Get<ILoading>().Unload();
            }
        }

        private void OnDeepLinkActivated(string url)
        {
            // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
            _deeplinkURL = url;
            string link = url.Split(new string[] { "://" }, StringSplitOptions.None)[1];

            switch (link)
            {
                case "login":
                    CloseAll();
                    UILoginView.Open();
                    break;
            }
        }

        private void OnDestroy()
        {
            if (environmentData != null)
            {
                environmentData.Destroy();
            }
        }
    }
}
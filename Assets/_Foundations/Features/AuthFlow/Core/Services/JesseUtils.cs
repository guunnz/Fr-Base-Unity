using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Architecture.Injector.Core;
using Firebase.Auth;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Web;
using UniRx;
using AuthFlow.EndAuth.Repo;
using AuthFlow.AboutYou.Core.Services;
using Data;
using AppleAuth.Interfaces;
using Data.Catalog;
using Data.Users;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using WebClientTools.Core.Services;
using Google;
using Data.Rooms;
using LocalizationSystem;
using UI.Auth;

class Constants
{
    public const string GOOGLE_PLAY_URL = "https://play.google.com/store/apps/details?id=air.com.friendbase.android";
    public const string APP_STORE_URL = "https://apps.apple.com/es/app/friendbase-chat-create-play/id658389442";

    public static string Protocol = "https://";
    public static string Hostname = ""; //friendbase-staging.fly.dev
    public static string ApiRoot;

    public static string CreateEmailURL;
    public static string EmailExistsURL;
    public static string UsernameValidationEndpoint;
    public static string ResetPasswordEndpoint;
    public static string VerifyEmailEndpoint;
    public static string VerifyLegacyEmailEndpoint;

    public static string UsersEndpoint;
    public static string UserEndpoint;
    public static string UsersRootEndPoint;
    public static string ReportUserEndPoint;
    public static string UserRelationshipEndpoint;
    public static string StoreRootEndPoint;
    public static string ItemsEndPoint;
    public static string ChatRoomsEndpoint;
    public static string RoomTypesEndpoint;
    public static string SessionsEndpoint;
    public static string VerifyApplePurchaseEndpoint;
    public static string VerifyGooglePurchaseEndpoint;
    public static string PurchaseEndpoint;
    public static string GameInvite;

    public static string SocketUrl;

    public static string GuestUser;

    public static void SetHostname(string value)
    {
        Hostname = value;
        ApiRoot = Protocol + Hostname + "/api";
        CreateEmailURL = ApiRoot + "/users";
        EmailExistsURL = ApiRoot + "/email-exists";
        UsernameValidationEndpoint = ApiRoot + "/username-validation/";

        if (EnvironmentData.IsOnProduction)
        {
            ResetPasswordEndpoint = "https://us-central1-friendbase--prod.cloudfunctions.net/api/password-reset-email";
            VerifyEmailEndpoint = "https://us-central1-friendbase--prod.cloudfunctions.net/api/address-verification-email";
            VerifyLegacyEmailEndpoint = "https://us-central1-friendbase--prod.cloudfunctions.net/api/recover-legacy-account-email";

        }
        else
        {
            VerifyLegacyEmailEndpoint = "https://us-central1-friendbase--prod.cloudfunctions.net/api/recover-legacy-account-email";
            ResetPasswordEndpoint = "https://us-central1-friendbase-dev.cloudfunctions.net/api/password-reset-email";
            VerifyEmailEndpoint = "https://us-central1-friendbase-dev.cloudfunctions.net/api/address-verification-email";

        }

        UsersEndpoint = ApiRoot + "/users";
        UserEndpoint = ApiRoot + "/user";
        UsersRootEndPoint = ApiRoot + "/users";
        ReportUserEndPoint = ApiRoot + "/user-reports";
        UserRelationshipEndpoint = ApiRoot + "/users-relationship";
        StoreRootEndPoint = ApiRoot + "/store/";
        ItemsEndPoint = ApiRoot + "/items/";
        ChatRoomsEndpoint = ApiRoot + "/chat-rooms";
        RoomTypesEndpoint = ApiRoot + "/room-types";
        SessionsEndpoint = ApiRoot + "/user-sessions";
        VerifyApplePurchaseEndpoint = ApiRoot + "/verify-apple-purchase";
        VerifyGooglePurchaseEndpoint = ApiRoot + "/verify-google-purchase";
        GameInvite = ApiRoot + "/game-invitation";
        PurchaseEndpoint = ApiRoot + "/purchases";
        SocketUrl = "wss://" + Hostname + "/socket/websocket/";

        GuestUser = ApiRoot + "/guest-user";
    }
};

public class AppleJSON
{
    [JsonProperty("email")]
    public string Email { get; set; }
}

namespace AuthFlow
{
    public class JesseUtils
    {
        public static async Task<IEnumerable<string>> EmailProviders(string email)
        {
            IEnumerable<string> mailProvider = await FirebaseAuth.DefaultInstance.FetchProvidersForEmailAsync(email);
            IGameData gameData = Injection.Get<IGameData>();
            gameData.GetUserInformation().AuthProviders = mailProvider.ToList<string>();
            foreach (string provider in mailProvider)
            {
                Debug.Log("---- EmailProviders:" + provider);
            }
            return mailProvider;
        }

        public static bool HasProvider(String providerToCheck)
        {
            Debug.Log("---------- HasProvider:" + providerToCheck);
            if (FirebaseAuth.DefaultInstance.CurrentUser == null)
            {
                Debug.Log("---------- CurrentUser NULL");
                return false;
            }
            foreach (IUserInfo provider in FirebaseAuth.DefaultInstance.CurrentUser.ProviderData)
            {
                string prov = provider.ProviderId;
                string uid = provider.UserId;
                string mail = provider.Email;

                Debug.Log("---------- provider:" + prov + " uid:" + uid + " mail:" + mail);
                if (prov.Equals(providerToCheck))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<String> GetProviders()
        {
            Debug.Log("---------- GetProviders:");
            if (FirebaseAuth.DefaultInstance.CurrentUser == null)
            {
                Debug.Log("---------- CurrentUser NULL");
                return null;
            }
            List<String> listProviders = new List<string>();
            foreach (IUserInfo provider in FirebaseAuth.DefaultInstance.CurrentUser.ProviderData)
            {
                string prov = provider.ProviderId;
                string uid = provider.UserId;
                string mail = provider.Email;

                Debug.Log("---------- provider:" + prov + " uid:" + uid + " mail:" + mail);
                listProviders.Add(prov);
            }
            return listProviders;
        }

        // NOTE(Jesse) This could probably be merged with SignUpUser and switched
        // on a flag.  I feel like the error handling is the same.
        public static async Task<(FirebaseUser, string, string)> SignInUser(string email, string password)
        {
            (FirebaseUser, string, string) Result = (null, null, "Unknown error signing in user.");

            try
            {
                FirebaseUser firebaseUser = await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
                string loginToken = await firebaseUser.TokenAsync(true);

                /* Injection.Get<IAnalyticsService>().SendLoginEvent(); */
                Debug.LogFormat("Firebase user signed in successfully: {0} ({1})", firebaseUser.DisplayName,
                    firebaseUser.UserId);

                Result = (firebaseUser, loginToken, null);

                if (await FetchAndCachePhoenixUserInfo(firebaseUser, loginToken))
                {
                    Result = (firebaseUser, loginToken, null);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                if (e.Message.ToLower().Contains("too_many"))
                {

                    AuthModalError modalError = AuthFlowManager.Instance.modalError;
                    modalError.Show(
                     modalError.TemplateAccountTooMany.Title,
                     modalError.TemplateAccountTooMany.Message,
                     null
                 );
                }
                Result = (null, null, e.InnerException.Message);
            }

            return Result;
        }

        public static async Task<string> IsUserLoggedIn()
        {
            string Result = null;

            try
            {
                var firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
                if (firebaseUser != null)
                {
                    ILocalUserInfo userInfo = Injection.Get<ILocalUserInfo>();
                    string loginToken = userInfo["firebase-login-token"];

                    if (!string.IsNullOrEmpty(loginToken))
                    {
                        await FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync();
                        firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
                        if (firebaseUser != null)
                        {
                            loginToken = await firebaseUser.TokenAsync(true);

                            if (await FetchAndCachePhoenixUserInfo(firebaseUser, loginToken))
                            {
                                Result = loginToken;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Exception IsUserLoggedIn: " + e + " Message:" + e.Message);
                return null;
            }

            return Result;
        }

        public static async Task<bool> CreatePhoenixUser(FirebaseUser firebaseUser, string loginToken)
        {
            // TODO(Jesse): Why not just construct a JSON string directly?
            var json = new JObject
            {
                ["user"] = new JObject
                {
                    ["email"] = firebaseUser.Email,
                    ["firebase_uid"] = firebaseUser.UserId,
                    ["registration_type"] = "email",
                }
            }.ToString();

            var req = WebClient.Request(
                WebMethod.Post,
                Constants.CreateEmailURL,
                json,
                false,
                ("Content-Type", "application/json"),
                ("Authorization", "Bearer " + loginToken)
            );

            // NOTE(Jesse): If this request returns the actual user object (which I think it does)
            // we should return that instead of a boolean.  The call to this function in FetchAndCachePhoenixUserInfo
            // wants to know that information.
            RequestInfo response = null;
            try
            {
                response = await req.ObserveOnMainThread().ToTask();
            }
            catch (Exception e)
            {
                // Let the user know what went wrong?
            }

            bool Result = false;
            if (response != null)
            {
                var emailToken = response.json["data"]?["email"];
                // TODO(Jesse): This is wack.  There's got to be a better way.
                Result = emailToken != null && !(string.IsNullOrEmpty(emailToken.ToString()));
            }

            return Result;
        }


        // TODO(Jesse): The error handling in here is pretty janky .. fix it.
        public static async Task<(FirebaseUser, string, string)> SignUpUser(string email, string password)
        {
            string loginToken = null;
            string errorMessage = null;
            FirebaseUser firebaseUser = null;

            try
            {
                firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password);
                loginToken = await firebaseUser.TokenAsync(true);
            }
            catch (Exception e)
            {
                /* Debug.LogError("User Creation Exception: " + e.Message); */
                return (null, null, e.Message);
            }

            // NOTE(Jesse): I made the assumption this would return null if login/creation fails but I actually didn't check
            if (string.IsNullOrEmpty(loginToken) || firebaseUser == null)
            {
                return (null, null, "Unknown Error Creating Firebase User.");
            }
            else
            {
                Injection.Get<IAnalyticsService>().SendSignUpEvent("email");
                Debug.LogFormat("Firebase user created successfully: {0} ({1})", firebaseUser.DisplayName,
                    firebaseUser.UserId);
            }

            // NOTE(Jesse): This call to CreatePhoenixUser _should_ actually be redundant
            // because FetchAndCachePhoenixUserInfo will create it if it's not found, but it would
            // be an extra request, so we do it here anyways.
            bool Success = false;
            if (await JesseUtils.CreatePhoenixUser(firebaseUser, loginToken))
            {
                if (await JesseUtils.FetchAndCachePhoenixUserInfo(firebaseUser, loginToken))
                {
                    Success = true;
                }
            }

            (FirebaseUser, string, string) Result = (null, null, null);
            if (Success)
            {
                Result = (firebaseUser, loginToken, null);
            }
            else
            {
                Result = (null, null, "Error Signing up user.");
            }

            return Result;
        }

        private static async Task<bool> checkIfEmailIsTaken(
            string email,
            string currentProvider,
            SignInCheckCallbackData callbackData,
            Action<SignInCheckCallbackData> firebaseAuthCallback
            )
        {
            Debug.Log("************  checkIfEmailIsTaken 00");
            IEnumerable<string> providers = await JesseUtils.EmailProviders(email);
            Debug.Log("************  checkIfEmailIsTaken 01");
            Debug.Log("************  checkIfEmailIsTaken COUNT:" + providers.Count());
            foreach (string prov in providers)
            {
                Debug.Log("************  checkIfEmailIsTaken provider:" + prov);
            }

            // If email taken but apple not in providers
            if (providers.Count() > 0 && !providers.Contains(currentProvider))
            {
                Debug.Log("************  checkIfEmailIsTaken 02");
                callbackData.isSuccess = false;
                callbackData.providerUsingEmail = providers.First().ToString();
                firebaseAuthCallback(callbackData);
                Debug.Log("************  checkIfEmailIsTaken TRUE");
                return true;
            }
            Debug.Log("************  checkIfEmailIsTaken FALSE");
            return false;
        }

        // Using the credentials returned by Apple Auth allows logging in with Firebase
        public static async void SignInUserApple(
            IAppleIDCredential appleIdCredential,
            string rawNonce,
            Action<SignInCheckCallbackData> firebaseAuthCallback
            )
        {
            string identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
            string authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
            SignInCheckCallbackData callbackData = new SignInCheckCallbackData();

            Credential firebaseCredential = OAuthProvider.GetCredential(
                AuthProvidersFirebase.APPLE,
                identityToken,
                rawNonce,
                authorizationCode
            );

            //// Decode JWT
            //var jsonString = JwtBuilder.Create().Decode(identityToken);
            //AppleJSON appleJson = JsonConvert.DeserializeObject<AppleJSON>(jsonString);

            //// Here we check if email exists on Firebase and which providers are linked to it
            //Task<bool> isEmailTaken = checkIfEmailIsTaken(
            //    appleJson.Email,
            //    "apple.com",
            //    callbackData,
            //    firebaseAuthCallback
            //);
            //if (await isEmailTaken) return;

            await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(firebaseCredential).ContinueWithOnMainThread(
                signInTask => SignInWithUser(signInTask, (FirebaseUser firebaseUser) =>
                {
                    {
                        callbackData.isSuccess = true;
                        firebaseAuthCallback(callbackData);
                    }
                })
            );

            return;
        }

        public static async void SignInUserFacebook(Action<FirebaseUser> facebookAuthCallback)
        {
           
            // AccessToken class will have session details
            Facebook.Unity.AccessToken fullAccessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            string accessToken = fullAccessToken.TokenString;

            Credential firebaseCredential = FacebookAuthProvider.GetCredential(accessToken);

            await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(firebaseCredential).ContinueWithOnMainThread(
                task => SignInWithUser(task, facebookAuthCallback)
            );
        }

        public static async void LinkFacebookProvider(Action<FirebaseUser> facebookAuthCallback)
        {
            // AccessToken class will have session details
            Facebook.Unity.AccessToken fullAccessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            string accessToken = fullAccessToken.TokenString;

            Credential firebaseCredential = FacebookAuthProvider.GetCredential(accessToken);

            await FirebaseAuth.DefaultInstance.CurrentUser.LinkWithCredentialAsync(firebaseCredential).ContinueWithOnMainThread(
                task => SignInWithUser(task, facebookAuthCallback)
            );
        }

        public static async void SignInUserGoogle(Action<FirebaseUser> googleAuthCallback, string idToken)
        {
            // AccessToken class will have session details
            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
            Debug.Log("** SignInUserGoogle credential:" + credential);

            await FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(
                task => SignInWithUser(task, googleAuthCallback)
            );
        }

        public static async void LinkUserGoogle(Action<FirebaseUser> googleAuthCallback, string idToken)
        {
            // AccessToken class will have session details
            Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
            Debug.Log("** LinkUserGoogle credential:" + credential);

            await FirebaseAuth.DefaultInstance.CurrentUser.LinkWithCredentialAsync(credential).ContinueWithOnMainThread(
                task => SignInWithUser(task, googleAuthCallback)
            );
        }

        //public static async void SignInUserGoogle(Action<FirebaseUser> googleAuthCallback)
        public static async void SignInUserGoogle_OLD(Action<SignInCheckCallbackData> firebaseAuthCallback, string webClientId)
        {
            Debug.Log("** SignInUserGoogle 00");

            //if (GoogleSignIn.Configuration == null)
            //{
            //    GoogleSignIn.Configuration = new GoogleSignInConfiguration
            //    {
            //        RequestIdToken = true,
            //        WebClientId = webClientId
            //    };
            //}


            GoogleSignInConfiguration configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;

            Debug.Log("** SignInUserGoogle webClientId:" + webClientId);

            Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();

            Debug.Log("** SignInUserGoogle 01");

            TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();

            Debug.Log("** SignInUserGoogle 02");

            SignInCheckCallbackData callbackData = new SignInCheckCallbackData();

            Debug.Log("** SignInUserGoogle 03");

            await signIn.ContinueWithOnMainThread(async task =>
            {
                Debug.Log("** SignInUserGoogle 04");

                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.Log("Error signing in with Google.");
                    Debug.LogError(task.Exception?.ToString());
                    callbackData.isSuccess = false;

                    firebaseAuthCallback(callbackData);
                    return;
                }

                Debug.Log("** SignInUserGoogle 05");

                Debug.Log("Welcome: " + task.Result.DisplayName + "!");
                Debug.Log("Email = " + task.Result.Email);

                Task<bool> isEmailTaken = checkIfEmailIsTaken(
                    task.Result.Email,
                    "google.com",
                    callbackData,
                    firebaseAuthCallback
                );

                Debug.Log("** SignInUserGoogle 06 isEmailTaken:" + isEmailTaken);

                if (await isEmailTaken)
                {
                    //Should take you to login.
                    Debug.Log("Email and password for this account already exists, please log in with email and password");
                    Logout();
                    return;
                }

                Debug.Log("** SignInUserGoogle 07");

                // Register and continue with auth flow
                Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

                Debug.Log("** SignInUserGoogle 08 credential:" + credential);

                FirebaseAuth.DefaultInstance.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(
                    signInTask => SignInWithUser(signInTask, (FirebaseUser firebaseUser) =>
                    {
                        {
                            callbackData.isSuccess = true;
                            firebaseAuthCallback(callbackData);
                        }
                    })
                );

                Debug.Log("** SignInUserGoogle 09");
            });
        }


        // Access Firebase with a FirebaseUser, this should only be used by Providers
        private static async Task<(FirebaseUser, string, string)> SignInWithUser(Task<FirebaseUser> task, Action<FirebaseUser> firebaseUserCallback)
        {
            Debug.Log("---------- SignInWithUser 00");
            (FirebaseUser, string, string) Result = (null, null, "Unknown error signing in user.");

            if (task.IsCanceled)
            {
                Debug.Log("---------- SignInWithUser 01");
                Result = (null, null, "Firebase auth was canceled");
                firebaseUserCallback(null);
            }
            else if (task.IsFaulted)
            {
                Debug.Log("---------- SignInWithUser 02 Exception:" + task.Exception);
                Result = (null, null, "Firebase auth failed");
                firebaseUserCallback(null);
            }
            else
            {
                Debug.Log("---------- SignInWithUser 03");
                FirebaseUser firebaseUser = task.Result;
                Debug.Log("------- Firebase auth completed | User ID:" + firebaseUser.UserId);

                string loginToken = await firebaseUser.TokenAsync(true);
                Result = (firebaseUser, loginToken, null);

                if (await FetchAndCachePhoenixUserInfo(firebaseUser, loginToken))
                {
                    Result = (firebaseUser, loginToken, null);
                    firebaseUserCallback(firebaseUser);
                }
            }

            return Result;
        }

        // TODO(Jesse): This should actually return a bool and get called
        // internally by "LoginUser" and "SignUpUser" instead of where we're
        // logging in and signing up users.  Need to fix the AboutYouWebClient
        // situation first..
        public static async Task<bool> FetchAndCachePhoenixUserInfo(FirebaseUser firebaseUser, string loginToken)
        {
            bool Result = false;

            IAboutYouWebClient ayWebClient = Injection.Get<IAboutYouWebClient>();
            ILocalUserInfo userInfo = Injection.Get<ILocalUserInfo>();
            userInfo.Clear();

            IAboutYouStateManager aboutYouState = Injection.Get<IAboutYouStateManager>();
            aboutYouState.Clear();

            Web.RequestInfo userData = null;

            try
            {
                userData = await ayWebClient.GetUserData();
            }
            catch (Exception e)
            {
                // TODO(Jesse): This is pretty janky and could be improved.
                // Specifically, if the second GetUserData function fails .. what
                // then?  Instead of throwing exceptions we should just return null

                if (e.Message.StartsWith("HTTP error \"Not Found\"  HTTP/1.1 404 Not Found Server"))
                {
                    bool userCreated = await JesseUtils.CreatePhoenixUser(firebaseUser, loginToken);
                    if (userCreated)
                    {
                        userData = await ayWebClient.GetUserData();
                    }
                    else
                    {
                        // Hard failure case .. we have a firebase user but couldn't
                        // create a Phoenix user.
                        //
                        // At least logout the firebase user such that they can try
                        // again?
                        FirebaseAuth.DefaultInstance.SignOut();
                    }
                }
            }

            if (userData == null) // Hard failure case
            {
                Result = false;
            }
            else
            {
                var userJson = new JObject(userData.json).Value<JObject>("data");


                // NOTE(Jesse): this is a bit wonky .. custom_avatar is set to false
                // when a new account is created, and set to true once we've passed
                // through avatar customization.  Not an issue, but the code reads
                // a little weird.
                bool doAvatarCustomization = !userJson.Value<bool>("custom_avatar");

                IGameData gameData = Injection.Get<IGameData>();
                gameData.GetUserInformation().Do_avatar_customization = doAvatarCustomization;
                var user = gameData.GetUserInformation();

                userInfo["firebase-login-token"] = loginToken;
                userInfo["terms"] = userJson.Value<string>("terms_accepted");
                aboutYouState.FirstName = userJson.Value<string>("first_name");
                aboutYouState.LastName = userJson.Value<string>("last_name");
                aboutYouState.Gender = userJson.Value<string>("gender");
                aboutYouState.UserName = userJson.Value<string>("username");
                var birthday = userJson.Value<string>("birthday");

                if (!string.IsNullOrEmpty(birthday))
                {
                    aboutYouState.BirthDate = DateTime.Parse(birthday);
                }

                string language = userJson.Value<string>("language");


                if (!string.IsNullOrEmpty(language))
                {
                    Injection.Get<ILanguage>().SetCurrentLanguage(language);
                }

                if (await GetInitialAvatarEndpoints())
                {
                    Result = true;
                }
            }

            return Result;
        }

        public static async Task<bool> LoginTracking(UserInformation firebaseUser)
        {
            var token = await Injection.Get<IWebHeadersBuilder>().BearerToken;

            var platform = "";
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    platform = "android";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    platform = "ios";
                    break;
                case RuntimePlatform.WebGLPlayer:
                    platform = "web";
                    break;
                default:
                    platform = "unknown";
                    break;
            }

            var json = new JObject
            {
                ["login"] = new JObject
                {
                    ["platform"] = platform
                }
            }.ToString();

            var req = WebClient.Request(
                WebMethod.Post,
                Constants.UsersEndpoint + "/" + firebaseUser.UserId + "/login-tracking",
                json,
                false,
                ("Content-Type", "application/json"),
                token
            );

            RequestInfo response = null;
            try
            {
                response = await req.ObserveOnMainThread().ToTask();
            }
            catch (Exception e)
            {
                // Let the user know what went wrong?
            }
            return true;
        }

        public static async Task<bool> SessionTracking(UserInformation firebaseUser)
        {
            var token = await Injection.Get<IWebHeadersBuilder>().BearerToken;

            var json = new JObject
            {
                ["user_session"] = new JObject
                {
                    ["user_id"] = firebaseUser.UserId
                }
            }.ToString();

            var req = WebClient.Request(
                WebMethod.Post,
                Constants.SessionsEndpoint,
                json,
                false,
                ("Content-Type", "application/json"),
                token
            );

            RequestInfo response = null;
            try
            {
                response = await req.ObserveOnMainThread().ToTask();
            }
            catch (Exception e)
            {
                // Let the user know what went wrong?
            }
            return true;
        }

        public static async Task<bool> GetInitialAvatarEndpoints()
        {
            bool Result = false;

            try
            {

                IGameData gameData = Injection.Get<IGameData>();

                //Get User Information
                var usInformation = await Injection.Get<IAvatarEndpoints>().GetUserInformation();
                gameData.GetUserInformation().Initialize(usInformation);
                LoginTracking(gameData.GetUserInformation());

                //Get ALL Catalog of items
                var listItems = await Injection.Get<IAvatarEndpoints>().GetAvatarCatalogItemsList();

                //Initialize Catalof of items
                gameData.InitializeCatalogs(listItems.ToList<GenericCatalogItem>());

                //After we get the items we can ask for my avatar skin
                var json = await Injection.Get<IAvatarEndpoints>().GetAvatarSkin();

                //string jsonString = json.ToString(Formatting.Indented);
                //Debug.Log("GetAvatarSkin <color=green>" + jsonString + "</color>");
                AvatarCustomizationData avatarData = new AvatarCustomizationData();
                avatarData.SetDataFromUserSkin(json);
                gameData.GetUserInformation().GetAvatarCustomizationData().SetData(avatarData);

                //Get User Inventory
                var listBagItems = await Injection.Get<IAvatarEndpoints>().GetPlayerInventory();
                gameData.AddItemsToBag(listBagItems);

                //Get MY Room information
                RoomInformation myRoomInformation = await Injection.Get<IRoomListEndpoints>().GetMyIdHouse();
                gameData.SetMyHouseInformation(myRoomInformation);
                gameData.SetRoomInformation(myRoomInformation);

                await Injection.Get<IAvatarEndpoints>().TrackUserSession();

                //Get blocked users list
                var blockedUsers = await Injection.Get<IAvatarEndpoints>().GetAvatarBlockedList();
                JArray blockedUsersObject = blockedUsers["data"].Value<JArray>();

                List<int> blockedUsersIdList = new List<int>();

                foreach (var token in blockedUsersObject.Children())
                {
                    blockedUsersIdList.Add(token["user_blocked"]["id"].Value<int>());
                }

                ////Get blocked users list
                //var blockerUsers = await Injection.Get<IAvatarEndpoints>().GetAvatarBlockerList();
                //JArray blockerUsersObject = blockerUsers["data"].Value<JArray>();

                //foreach (var token in blockerUsersObject.Children())
                //{
                //    blockedUsersIdList.Add(token["user_blocked"]["id"].Value<int>());
                //}

                gameData.GetUserInformation().SetBlockedPlayers(blockedUsersIdList);

                //User Account Status
                UserAccountStatus userStatus = await Injection.Get<IAvatarEndpoints>().GetUserStatus();
                //UserAccountStatus userStatus = new UserAccountStatus();
                //userStatus.ActivateSuspension();
                gameData.GetUserInformation().UserStatus = userStatus;

                Result = true;
            }
            catch (Exception e)
            {
                // TODO(Jesse): Write actual error handling code
                Debug.LogError("Something went wrong during GetInitialAvatarEndpoints()");
            }
            return Result;
        }

        public static void Logout()
        {
            // TODO(Jesse): Disconnect websocket if it's connected
            // TODO(Jesse): Stop audio track if it's playing

            // TODO(Jesse): Should we nuke all playerprefs at this point?
            // Probably not because it's nice if the users email is saved when
            // they go to login again .. but I'm not sure if that's even
            // happening right now.
            // PlayerPrefs.DeleteAll();

            FirebaseAuth.DefaultInstance.SignOut();

            // Try to sign out from Google
            try
            {
                GoogleSignIn.DefaultInstance.SignOut();
            }
            catch (Exception e)
            {
                // Is not necessary to do anything here
            }

            // This is a 'nuke from orbit' type of operation.  The Injection
            // system caches objects until you tell it to remove them which
            // causes a number of issues throughout the Auth flow.  Doing this
            // papers over them, however users may still hit them if they go back
            // and forth between views a bunch of times.
            Injection.ClearAll();

            SceneManager.LoadScene("MainScene");
        }





        public static async void SendEmailResetPassword(string email)
        {
            JObject json = new JObject
            {
                ["email"] = email,
            };

            IObservable<RequestInfo> request = WebClient.Post(
                Constants.ResetPasswordEndpoint,
                json.ToString(),
                false,
                ("Content-Type", "application/json")
            );

            try
            {
                Debug.Log("---- SendEmailResetPassword");
                await request.ObserveOnMainThread().ToTask();
            }
            catch (Exception e)
            {
                Debug.Log("---- SendEmailResetPassword Exception: " + e.Message);
                // ..
            }
        }

        public static async void SendEmailVerification(string email)
        {
            JObject json = new JObject
            {
                ["email"] = email,
            };
            var token = await Injection.Get<IWebHeadersBuilder>().BearerToken;

            IObservable<RequestInfo> request = WebClient.Post(
                Constants.VerifyEmailEndpoint,
                json.ToString(),
                false,
                ("Content-Type", "application/json"), token
            );

            try
            {
                Debug.Log("---- SendEmailVerification: " + email);
                await request.ObserveOnMainThread().ToTask();
            }
            catch (Exception e)
            {
                Debug.Log("---- SendEmailVerification Exception: " + e.Message);
                if (e.Message.ToLower().Contains("too_many"))
                {
                    AuthModalError modalError = AuthFlowManager.Instance.modalError;
                    modalError.Show(
                     modalError.TemplateAccountTooMany.Title,
                     modalError.TemplateAccountTooMany.Message,
                     null
                 );
                }
            }
        }

        public static async void LegacyEmailVerification(string email)
        {
            JObject json = new JObject
            {
                ["email"] = email,
            };


            IObservable<RequestInfo> request = WebClient.Post(
                Constants.VerifyLegacyEmailEndpoint,
                json.ToString(),
                false,
                ("Content-Type", "application/json")
            );

            try
            {
                Debug.Log("---- LegacyEmailVerification");
                await request.ObserveOnMainThread().ToTask();
            }
            catch (Exception e)
            {
                Debug.Log("---- LegacyEmailVerification Exception: " + e.Message);
                if (e.Message.ToLower().Contains("too_many"))
                {
                    AuthModalError modalError = AuthFlowManager.Instance.modalError;
                    modalError.Show(
                     modalError.TemplateAccountTooMany.Title,
                     modalError.TemplateAccountTooMany.Message,
                     null
                 );
                }
            }
        }

        public static void ConnectionError()
        {
            ClearInjection();
            SceneManager.LoadScene("ConnectionError");
        }

        public static void StartGame()
        {
            NauthFlowManager.SetStateAuthFlow(NauthFlowManager.STATE_AUTH_FLOW.FIRST_TIME);
            UINauthFlowLoginScreen.ResetFirstTime();
            SceneManager.LoadScene("MainScene");
        }

        public static void Nuke()
        {
            ClearInjection();
            StartGame();
        }

        static void ClearInjection()
        {
            Injection.ClearAll();
        }

        public static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
using System;
using Architecture.Injector.Core;
using AuthFlow;
using AuthFlow.EndAuth.Repo;
using Firebase.Auth;
using UnityEngine;
using Google;
using System.Threading.Tasks;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using AuthFlow.AboutYou.Core.Services;

namespace UI.Auth
{
    public class AuthProviderGoogle : MonoBehaviour
    {
        private AuthFirebaseManager _authFirebaseManager;
        private AuthFlowManager _authFlowManager;
        private ILocalUserInfo _userInfo;
        private IAboutYouStateManager _aboutYouState;

        private string _userName;
        private string _userMail;

        private static string webClientId = "";
        public static void SetWebClientId(string value)
        {
            webClientId = value;
        }

        public void Start()
        {
            _userInfo = Injection.Get<ILocalUserInfo>();
            _authFirebaseManager = FindObjectsOfType<AuthFirebaseManager>()[0];
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];
            _aboutYouState = Injection.Get<IAboutYouStateManager>();

            if (_authFirebaseManager == null)
            {
                throw new Exception("AuthFirebaseManager not found");
            }
            
            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }
        }

        public async void SignInWithGoogle()
        {
            // Show the loading overlay
            _authFlowManager.SetLoading(true);

            Debug.Log("** SignInWithGoogle 01");

            GoogleSignInConfiguration configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;

            Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();
            TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();

            Debug.Log("** SignInWithGoogle 03");

            await signIn.ContinueWithOnMainThread(async task =>
            {
                Debug.Log("** SignInWithGoogle 04");

                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.Log("Error signing in with Google.");
                    Debug.LogError(task.Exception?.ToString());
                    _authFlowManager.SetLoading(false);
                    return;
                }

                _userMail = task.Result.Email;
                _userName = task.Result.DisplayName;

                Debug.Log("** SignInWithGoogle 05");
                Debug.Log("Welcome: " + _userName + "!");
                Debug.Log("Email = " + _userMail);

                // Here we check if email exists on Firebase and which providers are linked to it
                IEnumerable<string> providers = await JesseUtils.EmailProviders(_userMail);

                foreach (string provider in providers)
                {
                    Debug.Log("---- GOOGLE provider:" + provider);
                }
                // If no account with that email or facebook provider present, continue with user creation/login
                if (providers.Count() == 0 || providers.Contains(AuthProvidersFirebase.GOOGLE))
                {
                    Debug.Log("** SignInWithGoogle 06");
                    JesseUtils.SignInUserGoogle(SignInWithFirebaseCallback, task.Result.IdToken);
                    return;
                }

                Debug.Log("---- GOOGLE ERROR");
                _authFlowManager.SetLoading(false);

                // If provider doesn't exist, show error message saying to log in with existing provider
                AuthModalError modalError = _authFlowManager.modalError;
                modalError.Show(
                    modalError.TemplateEmailInUse.Title,
                    modalError.TemplateEmailInUse.Message + $"<br><br> Use {providers.First().ToString()} to login",
                    null
                );
            });
        }

        // TODO: This callback is the same used in AuthProviderApple.cs. We should refactor this to avoid code duplication
        private void SignInWithFirebaseCallback(FirebaseUser firebaseUser)
        {
            AuthModalError modalError = _authFlowManager.modalError;

            _authFlowManager.SetLoading(false);

            if (firebaseUser != null)
            {
                // Set the email because some accounts need to be verified
                _authFirebaseManager.Email = FirebaseAuth.DefaultInstance.CurrentUser.ProviderData.First().Email;
                //_aboutYouState.FirstName = _userName;
                //_aboutYouState.LastName = _userName;

                Debug.Log("---- GOOGLE FirstName:" + _aboutYouState.FirstName);
                Debug.Log("---- GOOGLE LastName:" + _aboutYouState.LastName);
                // Continue with the AuthFlow
                _authFlowManager.Finish();
            }
        }
    }
}
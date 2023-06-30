using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.Injector.Core;
using AuthFlow;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.EndAuth.Repo;
using Facebook.Unity;
using Firebase.Auth;
using UnityEngine;

namespace UI.Auth
{
    public class AuthProviderFacebook : MonoBehaviour
    {
        public GameObject uiButtonFacebook;

        private AuthFirebaseManager _authFirebaseManager;
        private AuthFlowManager _authFlowManager;
        private ILocalUserInfo _userInfo;
        private IAboutYouStateManager _aboutYouState;

        public async void Start()
        {
            _userInfo = Injection.Get<ILocalUserInfo>();
            _aboutYouState = Injection.Get<IAboutYouStateManager>();
            _authFirebaseManager = FindObjectsOfType<AuthFirebaseManager>()[0];
            _authFlowManager = FindObjectsOfType<AuthFlowManager>()[0];

            if (_authFirebaseManager == null)
            {
                throw new Exception("AuthFirebaseManager not found");
            }

            if (_authFlowManager == null)
            {
                throw new Exception("AuthFlowManager not found");
            }

            // Disable Facebook button
            uiButtonFacebook.SetActive(false);

            // Initialize Facebook and toggle the Facebook Login button
            if (!FB.IsInitialized)
            {
                Debug.Log("---- FB TRY INIT");
                FB.Init(() =>
                {
                    if (FB.IsInitialized)
                    {
                        Debug.Log("---- FB INIT SUCCEED");
                        uiButtonFacebook.SetActive(true);
                    }
                });
            }
            else
            {
                Debug.Log("---- FB ALREADY INIT");
                // Enable Facebook button 
                uiButtonFacebook.SetActive(true);

                // Enable Facebook features
                // More info: https://developers.facebook.com/docs/unity/reference/current/FB.ActivateApp/
                FB.ActivateApp();
            }
        }

        public void SignInWithFacebook()
        {
            try
            {
                Injection.Get<IAnalyticsSender>().SendAnalytics(AnalyticsEvent.FacebookLoginButton);
                _authFlowManager.SetLoading(true);

            var permissions = new List<string>()
            {
                "email",
                "public_profile",
            };

            Debug.Log("---- FB SIGN IN");

            FB.LogInWithReadPermissions(permissions, result =>
            {
                Debug.Log("---- FB LogInWithReadPermissions 00");

                if (!FB.IsLoggedIn)
                {
                    _authFlowManager.SetLoading(false);
                    Debug.Log("User cancelled login");
                    return;
                }

                Debug.Log("---- FB LogInWithReadPermissions 01");

                // Get user email from Facebook
                FB.API("/me?fields=email", HttpMethod.GET, async (resultGraph) =>
                {
                    string emailValue = resultGraph.ResultDictionary["email"].ToString();

                    Debug.Log("---- FB LogInWithReadPermissions 02 emailValue:"+ emailValue);

                    // Here we check if email exists on Firebase and which providers are linked to it
                    IEnumerable<string> providers = await JesseUtils.EmailProviders(emailValue);

                    foreach (string provider in providers)
                    {
                        Debug.Log("---- FB provider:" + provider);
                    }

                    // If no account with that email or facebook provider present, continue with user creation/login
                    if (providers.Count() == 0 || providers.Contains(AuthProvidersFirebase.FACEBOOK))
                    {
                        Debug.Log("---- FB LogInWithReadPermissions SignInUserFacebook");
                        JesseUtils.SignInUserFacebook(SignInWithFacebookCallback);
                        return;
                    }

                    Debug.Log("---- FB ERROR");
                    _authFlowManager.SetLoading(false);
    
                    // If provider doesn't exist, show error message saying to log in with existing provider
                    AuthModalError modalError = _authFlowManager.modalError;
                    modalError.Show(
                        modalError.TemplateEmailInUse.Title,
                        modalError.TemplateEmailInUse.Message + $"<br><br> Use {providers.First().ToString()} to login",
                        null
                    );
                });
            });
            }
            catch (Exception ex)
            {
                _authFlowManager.SetLoading(false);
            }
        }

        private void SignInWithFacebookCallback(FirebaseUser firebaseUser)
        {
            _authFlowManager.SetLoading(false);
            if (firebaseUser != null)
            {
                UpdateUserInfo(result =>
                {
                    // Set the email because some accounts need to be verified
                    _authFirebaseManager.Email = FirebaseAuth.DefaultInstance.CurrentUser.ProviderData.First().Email;
                    // Continue with the AuthFlow
                    _authFlowManager.Finish(); 
                });
            }
        }

        private void UpdateUserInfo(Action<bool> callback)
        {
            // Get first name from Facebook
            FB.API("/me?fields=first_name,last_name", HttpMethod.GET, result =>
            {
                if (result.Error != null)
                {
                    Debug.Log("Error retrieving data");
                    callback(false);
                }
                else
                {
                    _aboutYouState.FirstName = result.ResultDictionary["first_name"] as string;
                    _aboutYouState.LastName = result.ResultDictionary["last_name"] as string;

                    Debug.Log("---- FB FirstName:"+ _aboutYouState.FirstName);
                    Debug.Log("---- FB LastName:"+ _aboutYouState.LastName);

                    callback(true);
                }
            });
        }
    }
}
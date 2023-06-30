using System;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using Architecture.Injector.Core;
using AuthFlow;
using AuthFlow.AboutYou.Core.Services;
using AuthFlow.AppleLogin.Infrastructure;
using AuthFlow.EndAuth.Repo;
using Firebase.Auth;
using UnityEngine;

namespace UI.Auth
{
    public class AuthProviderApple : MonoBehaviour
    {
        public GameObject uiButtonApple;

        private AuthFirebaseManager _authFirebaseManager;
        private AuthFlowManager _authFlowManager;
        private NonceGenerator _nonceGenerator;
        private IAppleAuthManager _appleAuthManager;
        private ILocalUserInfo _userInfo;
        private IAboutYouStateManager _aboutYouState;

        public void Start()
        {
            _nonceGenerator = new NonceGenerator();
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

            // Toggle the Apple button
            if (uiButtonApple != null)
                uiButtonApple.SetActive(AppleAuthManager.IsCurrentPlatformSupported);
        }

        private void Update()
        {
            // Updates the AppleAuthManager instance to execute pending callbacks inside Unity's execution loop
            _appleAuthManager?.Update();
        }

        public void SignInWithApple()
        {
            if (!AppleAuthManager.IsCurrentPlatformSupported)
            {
                Debug.LogWarning("The current platform is not supported by Apple Auth.");
                return;
            }

            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            PayloadDeserializer deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            _appleAuthManager = new AppleAuthManager(deserializer);

            // Show the loading overlay
            _authFlowManager.SetLoading(true);

            AppleAuthLoginArgs loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

            _appleAuthManager.LoginWithAppleId(
                loginArgs,
                credential =>
                {
                    // If a sign in with apple succeeds, we should have obtained the credential with the user id, name, and email, save it
                    _userInfo["AuthProviderAppleUser"] = credential.User;
                    // Sign In with Firebase
                    SignInWithFirebase(credential);
                },
                error =>
                {
                    _authFlowManager.SetLoading(false);
                    AuthorizationErrorCode authorizationErrorCode = error.GetAuthorizationErrorCode();
                    Debug.LogWarning("Sign in with Apple failed " + authorizationErrorCode + " " + error);
                }
            );
        }

        private void SignInWithFirebase(ICredential credential)
        {
            string nonce = _nonceGenerator.GenerateAppleNonce();
            IAppleIDCredential appleIdCredential = credential as IAppleIDCredential;

            if (appleIdCredential != null)
            {
                JesseUtils.SignInUserApple(appleIdCredential, nonce, SignInWithFirebaseCallback);
            }
        }

        // TODO: This callback is the same used in AuthProviderGoogle.cs. We should refactor this to avoid code duplication
        private void SignInWithFirebaseCallback(SignInCheckCallbackData callbackData)
        {
            AuthModalError modalError = _authFlowManager.modalError;

            _authFlowManager.SetLoading(false);

            if (callbackData.isSuccess)
            {
                //Set empty as apple do not allow to ask name. And we can not get name because of legacy sdk
                _aboutYouState.FirstName = "";
                _aboutYouState.LastName = "";
                // Continue with the AuthFlow
                _authFlowManager.Finish();
                return;
            }

            // If email taken show the error modal
            if (!callbackData.isSuccess && callbackData.providerUsingEmail != null)
            {
                modalError.Show(
                    modalError.TemplateEmailInUse.Title,
                    modalError.TemplateEmailInUse.Message + $"<br><br> Use {callbackData.providerUsingEmail} to login",
                    null
                );
                return;
            }

            // Show generic error message if something went wrong
            modalError.Show(
                modalError.TemplateDefault.Title,
                modalError.TemplateDefault.Message,
                null
            );
        }
    }
}
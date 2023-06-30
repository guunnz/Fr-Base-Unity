using System.Collections;
using System.Collections.Generic;
using AuthFlow;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

using System;
using System.Threading.Tasks;
using Firebase.Extensions;
using Google;


public class AuthLinkProviders : MonoBehaviour
{
    [SerializeField] private Button BtnLinkFacebookProvider;
    [SerializeField] private Button BtnLinkGoogleProvider;
    [SerializeField] private GameObject LoadingPanel;

    private string _userName;
    private string _userMail;

    private static string webClientId = "";
    public static void SetWebClientId(string value)
    {
        webClientId = value;
    }

    public void LinkFacebookProvider()
    {
        LoadingPanel.SetActive(true);

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
                LoadingPanel.SetActive(false);
                Debug.Log("User cancelled login");
                return;
            }

            Debug.Log("---- FB LogInWithReadPermissions 01");

            // Get user email from Facebook
            FB.API("/me?fields=email", HttpMethod.GET, async (resultGraph) =>
            {
                string emailValue = resultGraph.ResultDictionary["email"].ToString();
                Debug.Log("---- FB LogInWithReadPermissions 02 emailValue:" + emailValue);

                JesseUtils.LinkFacebookProvider(SignInWithFacebookCallback);
            });
        });
    }

    private void SignInWithFacebookCallback(FirebaseUser firebaseUser)
    {
        LoadingPanel.SetActive(false);
        BtnLinkFacebookProvider.gameObject.SetActive(false);
        BtnLinkGoogleProvider.gameObject.SetActive(false);
    }

    public async void LinkGoogleProvider()
    {
        LoadingPanel.SetActive(true);

        GoogleSignInConfiguration configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;

        Debug.Log("** SignInWithGoogle webClientId:" + webClientId);

        Task<GoogleSignInUser> signIn = GoogleSignIn.DefaultInstance.SignIn();
        TaskCompletionSource<FirebaseUser> signInCompleted = new TaskCompletionSource<FirebaseUser>();

        await signIn.ContinueWithOnMainThread(async task =>
        {
            Debug.Log("** LinkGoogleProvider 04");

            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("Error signing in with Google.");
                Debug.LogError(task.Exception?.ToString());
                LoadingPanel.SetActive(false);
                return;
            }

            _userMail = task.Result.Email;
            _userName = task.Result.DisplayName;

            Debug.Log("** LinkGoogleProvider 05");
            Debug.Log("Welcome: " + _userName + "!");
            Debug.Log("Email = " + _userMail);
            
            JesseUtils.LinkUserGoogle(SignInWithFirebaseCallback, task.Result.IdToken);
        });
    }

    private void SignInWithFirebaseCallback(FirebaseUser firebaseUser)
    {
        LoadingPanel.SetActive(false);
        BtnLinkGoogleProvider.gameObject.SetActive(false);
        BtnLinkFacebookProvider.gameObject.SetActive(false);
    }
}

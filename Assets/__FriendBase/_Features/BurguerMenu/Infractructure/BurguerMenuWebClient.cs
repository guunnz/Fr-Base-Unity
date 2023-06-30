using System;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using Data;
using Firebase.Auth;
using Firebase.Extensions;
using Newtonsoft.Json.Linq;
using UniRx;
using UnityEngine;
using Web;
using WebClientTools.Core.Services;

namespace BurguerMenu.Infractructure
{
    public class BurguerMenuWebClient
    {
        public static event Action<bool> OnPasswordUpdated;

        public static async Task<bool> CheckPassword(string currentPassword)
        {
            bool result;

            try
            {
                var passwordJson = new JObject
                {
                    ["password"] = currentPassword
                };

                var endpoint = Constants.UsersEndpoint + "/" + Injection.Get<IGameData>().GetUserInformation().UserId +
                               "/validate-password";

                var header = await Injection.Get<IWebHeadersBuilder>().BearerToken;


                var response = await WebClient.Post(endpoint, passwordJson, true, header);
                Debug.Log("Password validation Response " + response);

                var jResponse = response.json;

                result = jResponse["valid"].Value<bool>();
            }
            catch (Exception e)
            {
                result = false;
                Debug.LogError("Password may be wrong or request could have failed");
            }

            return result;
        }


        public static async Task<bool> RequestChangeEmailAsync(string newEmail)
        {
            bool result;

            try
            {
                var newEmailJson = new JObject
                {
                    ["email"] = newEmail
                };

                var endpoint = Constants.UsersEndpoint + "/" + Injection.Get<IGameData>().GetUserInformation().UserId +
                               "/email-change-request";

                var header = await Injection.Get<IWebHeadersBuilder>().BearerToken;

                await WebClient.Post(endpoint, newEmailJson, true, header);

                result = true;
            }
            catch (Exception e)
            {
                result = false;
                Debug.LogError(
                    "Unable to send change email: " + e.Message);
            }

            return result;
        }

        public static async Task<bool> ConfirmEmailChangeAsync()
        {
            bool result;

            try
            {
                var emptyJson = new JObject();

                var endpoint = Constants.UsersEndpoint + "/" + Injection.Get<IGameData>().GetUserInformation().UserId +
                               "/confirm-email-change";

                var header = await Injection.Get<IWebHeadersBuilder>().BearerToken;

                var response = await WebClient.Post(endpoint, emptyJson, true, header);

                Debug.Log("Confirm email change response " + response.text);

                result = true;
            }
            catch (Exception e)
            {
                result = false;
                Debug.LogError(
                    "Unable to confirm change email: " + e.Message);
            }

            return result;
        }

        public static void UpdatePassNoRelogin(string newPassword)
        {
            FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

            user.UpdatePasswordAsync(newPassword).ContinueWithOnMainThread(updatePasswordTask =>
            {
                if (updatePasswordTask.IsCanceled)
                {
                    if (OnPasswordUpdated != null) OnPasswordUpdated.Invoke(false);
                    Debug.LogError("UpdatePasswordAsync was canceled.");
                    return;
                }

                if (updatePasswordTask.IsFaulted)
                {
                    if (OnPasswordUpdated != null) OnPasswordUpdated.Invoke(false);
                    Debug.LogError("UpdatePasswordAsync encountered an error: " + updatePasswordTask.Exception);
                    return;
                }

                if (OnPasswordUpdated != null) OnPasswordUpdated.Invoke(true);
                Debug.Log("Password updated successfully.");
            });
        }


        public static void UpdatePassword(string currentPassword, string newPassword)
        {
            FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
            var userEmail = Injection.Get<IGameData>().GetUserInformation().Email;

            // Get auth credentials from the user for re-authentication. The example below shows
            // email and password credentials but there are multiple possible providers,
            // such as GoogleAuthProvider or FacebookAuthProvider.
            Credential credential = EmailAuthProvider.GetCredential(userEmail, currentPassword);

            if (user != null)
            {
                user.ReauthenticateAsync(credential).ContinueWith(authTask =>
                {
                    if (authTask.IsCanceled)
                    {
                        Debug.LogError("ReauthenticateAsync was canceled.");
                        return;
                    }

                    if (authTask.IsFaulted)
                    {
                        Debug.LogError("ReauthenticateAsync encountered an error: " + authTask.Exception);
                        return;
                    }

                    Debug.Log("User reauthenticated successfully.");

                    user.UpdatePasswordAsync(newPassword).ContinueWithOnMainThread(updatePasswordTask =>
                    {
                        if (updatePasswordTask.IsCanceled)
                        {
                            if (OnPasswordUpdated != null) OnPasswordUpdated.Invoke(false);
                            Debug.LogError("UpdatePasswordAsync was canceled.");
                            return;
                        }

                        if (updatePasswordTask.IsFaulted)
                        {
                            if (OnPasswordUpdated != null) OnPasswordUpdated.Invoke(false);
                            Debug.LogError("UpdatePasswordAsync encountered an error: " + updatePasswordTask.Exception);
                            return;
                        }

                        if (OnPasswordUpdated != null) OnPasswordUpdated.Invoke(true);
                        Debug.Log("Password updated successfully.");
                    });
                });
            }
        }

        public static async Task<bool> DeleteAccount()
        {
            bool result;

            try
            {
                var endpoint = Constants.UsersEndpoint + "/" +
                               Injection.Get<IGameData>().GetUserInformation().UserId;

                var header = await Injection.Get<IWebHeadersBuilder>().BearerToken;

                var response = await WebClient.Delete(endpoint, false, header);

                Debug.Log("Delete user account response: " + response.text);

                result = true;
            }
            catch (Exception e)
            {
                result = false;
                Debug.LogError(
                    "Unable to delete account: " + e.Message);
            }

            return result;
        }
    }
}
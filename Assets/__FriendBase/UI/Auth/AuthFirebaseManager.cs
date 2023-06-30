using System.Collections.Generic;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using AuthFlow;
using Firebase.Auth;
using LocalizationSystem;
using UnityEngine;

namespace UI.Auth
{
    public class AuthFirebaseManager : MonoBehaviour
    {
        public string Email;
        public List<string> EmailProviders;

        public bool UserHasAccount()
        {
            return EmailProviders != null && EmailProviders.Count > 0;
        }

        public async Task<(bool, string)> SignInWithEmail(string password)
        {
            (FirebaseUser firebaseUser, string loginToken, string error) = await JesseUtils.SignInUser(Email, password);

            // Wrong password
            if (error == "The password is invalid or the user does not have a password.")
            {
                return (false, Injection.Get<ILanguage>().GetTextByKey(LangKeys.AUTH_WRONG_PASSWORD));
            }

            // Another error
            if (error != null)
            {
                Debug.Log("error " + error);
                return (false, Injection.Get<ILanguage>().GetTextByKey(LangKeys.AUTH_UNEXPECTED_ERROR));
            }

            // Success
            return (true, "");
        }
    
        public async Task<bool> SignUpWithEmail(string email, string password)
        {
            (FirebaseUser firebaseUser, string loginToken, string error) = await JesseUtils.SignUpUser(email, password);

            if (error != null)
            {
                Debug.Log("error " + error);
                return false;
            }

            return true;
        }
    }
}

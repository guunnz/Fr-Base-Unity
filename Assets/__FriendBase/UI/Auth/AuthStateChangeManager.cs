using System;
using System.Collections;
using AuthFlow;
using Firebase.Auth;
using UnityEngine;

namespace UI.Auth
{
    public class AuthStateChangeManager : GenericSingleton<AuthStateChangeManager>
    {
        private Coroutine checkCoroutine;
        private bool isSubscribed;

        public void SubscribeOnAuthStateChange()
        {
            if (isSubscribed)
            {
                return;
            }

            isSubscribed = true;

            if (checkCoroutine != null)
            {
                StopCoroutine(CheckUserState());
                checkCoroutine = null;
            }

            checkCoroutine = StartCoroutine(CheckUserState());

            FirebaseAuth.DefaultInstance.StateChanged += AuthStateChanged;
        }

        public void UnsubscribeOnAuthStateChange()
        {
            isSubscribed = false;

            StopCoroutine(CheckUserState());
            checkCoroutine = null;

            FirebaseAuth.DefaultInstance.StateChanged -= AuthStateChanged;
        }

        void AuthStateChanged(object sender, EventArgs eventArgs)
        {
            //User credentials changed
            if (FirebaseAuth.DefaultInstance.CurrentUser == null)
            {
                Debug.LogError("The user's credentials expired, so the user was logged out. User must sign in again");
                UnsubscribeOnAuthStateChange();
                JesseUtils.Logout();
            }
        }

        private IEnumerator CheckUserState()
        {
            while (true)
            {
                yield return new WaitForSeconds(5);
                yield return FirebaseAuth.DefaultInstance.CurrentUser.ReloadAsync();
            }
        }

        private void OnDestroy()
        {
            UnsubscribeOnAuthStateChange();
        }
    }
}
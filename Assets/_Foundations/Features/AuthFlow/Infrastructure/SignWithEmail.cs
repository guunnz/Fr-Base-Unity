// TODO(Jesse): Delete this
//
using System;
using Architecture.Injector.Core;
using AuthFlow.Domain;
using Firebase.Auth;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace AuthFlow.Infrastructure
{
    [UsedImplicitly]
    public class SignWithEmail : ISignWithEmail, IUserPassword
    {

        public IObservable<UserAuthInfo> SignUpUser(string user, string password)
        {
            return CreateUser(user, password);
        }

        private IObservable<UserAuthInfo> CreateUser(string email, string password)
        {
            // START SIGN UP W EMAIL & PASSWORD

            return FirebaseAuth.DefaultInstance
                .CreateUserWithEmailAndPasswordAsync(email, password)
                .ToObservable()
                .Do(OnUserCreationSucceed)
                .DoOnError(OnUserCreationException)
                .SelectMany(ToDomain)
                .ObserveOnMainThread();
        }

        private static IObservable<UserAuthInfo> ToDomain(FirebaseUser firebaseUserInfo)
        {
            Debug.Log(firebaseUserInfo);
            return firebaseUserInfo.TokenAsync(true).ToObservable().Select(token =>
                new UserAuthInfo(
                    firebaseUserInfo.DisplayName,
                    firebaseUserInfo.Email,
                    firebaseUserInfo.PhotoUrl,
                    firebaseUserInfo.ProviderId,
                    firebaseUserInfo.UserId,
                    token));
        }

        private void OnUserCreationException(Exception e)
        {
            Debug.LogError("User Creation Exception: " + e.Message);
        }

        private void OnUserCreationSucceed(FirebaseUser firebaseUser)
        {
            // Firebase user has been created.
            Injection.Get<IAnalyticsService>().SendSignUpEvent("email");
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", firebaseUser.DisplayName,
                firebaseUser.UserId);
        }

        private void OnUserSignInSucceed(FirebaseUser firebaseUser)
        {
            // Firebase user has been created.
            Injection.Get<IAnalyticsService>().SendLoginEvent();
            Debug.LogFormat("Firebase user signed in successfully: {0} ({1})", firebaseUser.DisplayName,
                firebaseUser.UserId);
        }

        private void OnUserSignInException(Exception e)
        {
            //Debug.LogError("User Sign In Exception: " + e.Message);
        }

        public IObservable<UserAuthInfo> SignInUser(string email, string password)
        {
            return FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password)
                .ToObservable()
                .Do(OnUserSignInSucceed)
                .SelectMany(ToDomain)
                .ObserveOnMainThread();
        }

        private void OnResetPasswordException(Exception e)
        {
            Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + e.Message);
        }

        private void OnResetPasswordSucceed()
        {
            Debug.Log("Password reset email sent successfully.");
        }

        public IObservable<Unit> ResetPassword(string email)
        {
            Debug.Log($"Email {email}");
            return FirebaseAuth.DefaultInstance.SendPasswordResetEmailAsync(email)
                .ToObservable()
                .Do(_ => OnResetPasswordSucceed())
                .DoOnError(OnResetPasswordException)
                .AsUnitObservable();
        }

        private void OnUpdatePasswordException(Exception e)
        {
            Debug.LogError("UpdatePasswordAsync encountered an error: " + e.Message);
        }

        private void OnUpdatePasswordSucceed()
        {
            Debug.Log("Password updated successfully.");
        }

        public IObservable<Unit> UpdatePassword(string newPassword)
        {
            FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;

            return user.UpdatePasswordAsync(newPassword)
                .ToObservable()
                .Do(_ => OnUpdatePasswordSucceed())
                .DoOnError(OnUpdatePasswordException)
                .AsUnitObservable();
        }
    }
}

using System;
using System.Threading.Tasks;
using AuthFlow.Core.Services;
using Firebase;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace AuthFlow.Infrastructure
{
    [UsedImplicitly]
    public class FirebaseAccess : IFirebaseAccess
    {
        private FirebaseApp app;
        private IObservable<Unit> runningObservable;

        public IObservable<FirebaseApp> App => app != null ? Observable.Return(app) : GetFirebaseApp();

        private IObservable<FirebaseApp> GetFirebaseApp()
        {
            if (runningObservable != null)
            {
                return runningObservable.Select(_ => app);
            }

            runningObservable = InitFirebase().ToObservable();

            return runningObservable.Select(_ => app);
        }

        private async Task InitFirebase()
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = FirebaseApp.DefaultInstance;
                Debug.Log("Firebase is ready to use!");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                // Firebase Unity SDK is not safe to use here.
            }

            await Task.CompletedTask;
        }
    }
}
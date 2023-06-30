using System;
using UnityEngine;
using UniRx;
using Firebase.Analytics;

public class FirebaseAnalytic : IAnalyticsService
{
    public IObservable<Unit> SendLoginEvent()
    {
        return Observable.ReturnUnit().Do(() =>
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
        });
    }

    public IObservable<Unit> SendSignUpEvent(string signupMethod)
    {
        return Observable.ReturnUnit().Do(() =>
        {
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventSignUp, FirebaseAnalytics.ParameterSignUpMethod, signupMethod);
        });
    }
}

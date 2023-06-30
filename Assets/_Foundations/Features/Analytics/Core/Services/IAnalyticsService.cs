using System;
using UnityEngine;
using UniRx;

public interface IAnalyticsService
{
    IObservable<Unit> SendLoginEvent ();
    IObservable<Unit> SendSignUpEvent (string signUpMethod);
}

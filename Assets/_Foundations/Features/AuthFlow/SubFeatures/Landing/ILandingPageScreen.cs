using System;
using Architecture.MVP;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace AuthFlow.Landing
{
    public interface ILandingPageScreen : IPresentable
    {
        GameObject loadingSpinner { get; }

        Button emailLoginButton { get; }
        Button facebookLoginButton { get; }
        Button googleLoginButton { get; }
        Button appleLoginButton { get; }

        IObservable<Unit> OnGoToEmail { get; }
        IObservable<Unit> OnFacebookLogin { get; }
        IObservable<Unit> OnAppleLogin { get; }
    }
}

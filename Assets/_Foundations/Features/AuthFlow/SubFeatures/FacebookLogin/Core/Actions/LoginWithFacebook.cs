using System;
using AuthFlow.FacebookLogin.Core.Services;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace AuthFlow.FacebookLogin.Core.Actions
{
    [UsedImplicitly]
    public class LoginWithFacebook
    {
        readonly IFacebookService service;

        public LoginWithFacebook(IFacebookService service)
        {
            this.service = service;
        }

        public IObservable<Unit> Execute()
        {
            return Observable
                .ReturnUnit()
                .Do(() => Debug.Log("About to initialize FB"))
                .SelectMany(service.Init())
                .Do(() => Debug.Log("Initialized FB, logging in"))
                .SelectMany(service.Login());
        }
    }
}
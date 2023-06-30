using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AuthFlow.Landing
{
    public class LandingView : ViewNode, ILandingPageScreen
    {
        [SerializeField] Button goToEmail;
        [SerializeField] Button facebookLogin;
        [SerializeField] Button googleLogin;
        [SerializeField] Button appleLogin;

        [SerializeField] GameObject _loadingSpinner;

        public GameObject loadingSpinner => _loadingSpinner;
        public Button emailLoginButton => goToEmail;
        public Button googleLoginButton => googleLogin;
        public Button facebookLoginButton => facebookLogin;
        public Button appleLoginButton => appleLogin;

        public IObservable<Unit> OnGoToEmail => goToEmail.OnClickAsObservable();
        public IObservable<Unit> OnFacebookLogin => facebookLogin.OnClickAsObservable();
        public IObservable<Unit> OnAppleLogin => appleLogin.OnClickAsObservable();

        protected override void OnInit()
        {
            bool enableNewAuthFlow = true;

            if (enableNewAuthFlow)
            {
                SceneManager.LoadScene(GameScenes.NewAuthFlow, LoadSceneMode.Additive);
            }
            else
            {
                SceneManager.LoadScene(GameScenes.AuthFlow, LoadSceneMode.Additive);
            }
        }
    }
}

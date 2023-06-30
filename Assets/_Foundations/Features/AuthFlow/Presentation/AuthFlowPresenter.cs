using AuthFlow.Core.Services;
using CloudStorage.Core;
using LocalStorage.Core;
using UniRx;
using UnityEngine;

namespace AuthFlow.Presentation
{
    public class AuthFlowPresenter
    {
        private readonly IAuthFlowScreen screen;
        private readonly ILocalStorage localStorage;
        private readonly ICloudStorage cloudStorage;
        private readonly IFirebaseAccess firebaseAccess;
        private readonly ISignWithEmail signWithEmail;
        private readonly IUserPassword userPassword;

        private readonly CompositeDisposable disposable = new CompositeDisposable();
        private readonly CompositeDisposable buttonDisposable = new CompositeDisposable();

        public AuthFlowPresenter(IAuthFlowScreen screen, ILocalStorage localStorage, ICloudStorage cloudStorage,
            IFirebaseAccess firebaseAccess, ISignWithEmail signWithEmail, IUserPassword userPassword)
        {
            this.screen = screen;
            this.localStorage = localStorage;
            this.cloudStorage = cloudStorage;
            this.firebaseAccess = firebaseAccess;
            this.signWithEmail = signWithEmail;
            this.userPassword = userPassword;

            screen
                .OnEnabled
                .Subscribe(_ => Present())
                .AddTo(disposable);
            screen
                .OnDisabled
                .Subscribe(_ => Hide())
                .AddTo(disposable);
            screen
                .OnDisposed
                .Subscribe(_ => CleanUp())
                .AddTo(disposable);
        }

        private void Present()
        {
            firebaseAccess.App.Subscribe(app => { Debug.Log("app name : " + app.Name); }).AddTo(disposable);

            screen
                .OnSubmit
                .Subscribe(_ => OnSubmit())
                .AddTo(buttonDisposable);

            var storedUser = localStorage.GetString("userinfo-user");
            screen.UserFieldText = storedUser;
            Debug.Log(storedUser);
        }

        private void OnSubmit()
        {
            var user = screen.UserFieldText;
            var pass = screen.PassFieldText;
            localStorage.SetString("userinfo-user", user);
            localStorage.SetString("userinfo-pass", pass);
            Debug.Log($"Submit : user: {user}, pass: {pass}");

            // START SIGN UP W EMAIL & PASSWORD
            signWithEmail.SignUpUser(user, pass); // why both?
            signWithEmail.SignInUser(user, pass); // why both?

            // userPassword.UpdatePassword("12345678");
            // userPassword.ResetPassword(user);
        }

        private void Hide()
        {
            buttonDisposable.Clear();
        }

        private void CleanUp()
        {
            disposable.Clear();
        }

    }
}

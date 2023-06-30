using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using UI;
using UI.Utils;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AuthFlow.Login
{
    public class LoginView : ViewNode, ILoginScreen
    {
        [SerializeField] Button goToNext;
        [SerializeField] Button goToBack;
        [SerializeField] Button goToForgot;

        [SerializeField] PasswordWidget passwordWidget;
        [SerializeField] StringWidget emailLabel;
        [SerializeField] StringWidget errorLabelWidget;

        [SerializeField] Graphic errorOutline;

        [SerializeField] GameObject loadingPanel;

        [FormerlySerializedAs("errorLabel")] [SerializeField]
        CanvasGroup errorLabelAlpha;

        protected override void OnInit()
        {
            this.CreatePresenter<LoginPresenter, ILoginScreen>();
        }

        public IObservable<Unit> OnMoveNext =>
            goToNext.OnClickAsObservable()
                .Merge(passwordWidget.field.OnSubmitAsObservable()
                    .Do(passwordWidget.ClearField)
                    .AsUnitObservable());

        public IObservable<Unit> OnGoToBack => goToBack
            .OnClickAsObservable().Do(ClearPasswordField)
            .Do(_ => SetError(null));

        public IObservable<Unit> OnGoToForgot => goToForgot.OnClickAsObservable().Do(ClearPasswordField);

        public void SetEmail(string userEmail)
        {
            const int emailMaxCharactersOnScreen = 24;

            if (userEmail.Length > emailMaxCharactersOnScreen)
            {
                userEmail = $"{userEmail.Substring(0, emailMaxCharactersOnScreen)}...";
            }

            emailLabel.Value = userEmail;
        }

        public void SetLoadingPanelActive(bool state)
        {
            loadingPanel.SetActive(state);
        }

        public string GetPassword()
        {
            return passwordWidget.field.text;
        }

        void UpdateErrorAlpha(float alpha)
        {
            var color = errorOutline.color;
            color.a = alpha;
            errorOutline.color = color;
            errorLabelAlpha.alpha = alpha;
        }

        public void SetError(string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
              UpdateErrorAlpha(0);
              errorLabelWidget.Value = "";
            }
            else
            {
              UpdateErrorAlpha(1);
              errorLabelWidget.Value = errorMessage;
            }
        }

        public void ClearPasswordField()
        {
            passwordWidget.ClearField();
        }
    }
}

using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using TMPro;
using UI;
using UI.Utils;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace AuthFlow.SignUp
{
    public class SignUpView : ViewNode, ISignUpScreen
    {
        [SerializeField] Button goToNext;
        [SerializeField] Button goToBack;

        [SerializeField] PasswordWidget passwordWidget;
        [SerializeField] StringWidget emailLabel;

        [SerializeField] Graphic errorOutline;
        [SerializeField] CanvasGroup errorLabel;
        [SerializeField] StringWidget errorText;

        [SerializeField] GameObject loadingPanel;

        protected override void OnInit()
        {
            this.CreatePresenter<SignUpPresenter, ISignUpScreen>();
        }

        public IObservable<Unit> OnMoveNext =>
            goToNext.OnClickAsObservable()
                .Merge(passwordWidget.field.OnSubmitAsObservable().AsUnitObservable());

        public IObservable<Unit> OnGoToBack => goToBack.OnClickAsObservable().Do(_ => SetError(""));

        public void SetLoadingPanelActive(bool state)
        {
            loadingPanel.SetActive(state);
        }

        public void SetEmail(string userEmail)
        {
            if (userEmail.Length > 24)
            {
                emailLabel.Value = userEmail?.Substring(0, 24) + "...";
            }
            else
            {
                emailLabel.Value = userEmail;
            }
        }

        protected override void OnShow()
        {
            ClearPasswordField();
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
            errorLabel.alpha = alpha;
        }

        public void SetError(string err)
        {
            if (string.IsNullOrEmpty(err))
            {
              UpdateErrorAlpha(0);
              errorText.Value = "";
            }
            else
            {
              UpdateErrorAlpha(1);
              errorText.Value = err;
            }
        }

        public void ClearPasswordField()
        {
            passwordWidget.ClearField();
        }
    }
}

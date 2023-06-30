using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using AuthFlow.VerifyEmail.Presentation;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace AuthFlow.VerifyEmail.View
{
    public class VerifyEmailView : ViewNode, IVerifyEmailView
    {
        public Button sendAgainButton;
        public Button backButton;
        public StringWidget emailLabel;
        public GameObject beforeTime;

        protected override void OnInit()
        {
            this.CreatePresenter<VerifyEmailPresenter, IVerifyEmailView>();
        }

        protected override void OnShow()
        {
            beforeTime.SetActive(false);
            DoWait(3).Do(() => beforeTime.SetActive(true)).Subscribe().AddTo(showDisposables);
        }

        public void SetEmail(string infoEmail)
        {
            emailLabel.Value = infoEmail;
        }

        public IObservable<Unit> OnSendAgain => sendAgainButton.OnClickAsObservable();
        public IObservable<Unit> OnGoToBack => backButton.OnClickAsObservable();
    }
}

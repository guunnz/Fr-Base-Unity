using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using AuthFlow.ForgotPass.Presentation;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace AuthFlow.ForgotPass.View
{
    public class ForgotPassView : ViewNode, IForgotPassView
    {
        public Button sendAgainButton;
        public Button backButton;
        public StringWidget emailLabel;
        public GameObject beforeTime;

        protected override void OnInit()
        {
            this.CreatePresenter<ForgotPassPresenter, IForgotPassView>();
        }

        protected override void OnShow()
        {
            beforeTime.SetActive(false);
            DoWait(3).Do(() => beforeTime.SetActive(true)).Subscribe().AddTo(showDisposables);
        }

        public void OnResend()
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
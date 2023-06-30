using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace AuthFlow.EnterEmail
{
    public class EnterEmailView : ViewNode, IEnterEmailScreen
    {
        [SerializeField] Color regularColor = new Color(0.40625F, 0.1640625F, 0.09375F, 0.9960938F);
        [SerializeField] Color errorColor = new Color(0.8476563F, 0.2109375F, 0.07031025F, 0.9960938F);


        [SerializeField] Button goToBack;
        [SerializeField] Button goToNext;

        [SerializeField] TMP_InputField inputField;
        [SerializeField] GameObject errorContainer;
        [SerializeField] TMP_Text errorText;
        [SerializeField] Image outline;

        public IObservable<Unit> OnGoToBack => goToBack.OnClickAsObservable()
            .Do(_ => ShowError(false))
            .Do(_ => SetOutlineRegular());

        public IObservable<Unit> OnGoToNext => goToNext.OnClickAsObservable()
            .Do(_ => ShowError(false))
            .Do(_ => SetOutlineRegular())
            .Merge(inputField.onSubmit.AsObservable().AsUnitObservable());

        public string GetInputText() => inputField.text;
        public void ShowError(bool show) => errorContainer.SetActive(show);
        public void SetErrorText(string text) => errorText.text = text;
        public void SetOutlineRegular() => outline.color = regularColor;
        public void SetOutlineError() => outline.color = errorColor;
        public void SetInputText(string email) => inputField.text = email;

        void Awake()
        {
            errorContainer.SetActive(false);
        }

        protected override void OnInit()
        {
            this.CreatePresenter<EnterEmailPresenter, IEnterEmailScreen>();
        }
    }
}
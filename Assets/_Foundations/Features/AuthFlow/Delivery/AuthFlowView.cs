using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using AuthFlow.Presentation;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace AuthFlow.Delivery
{
    public class AuthFlowView : ViewNode, IAuthFlowScreen
    {
        [SerializeField] private Button submitButton;
        [SerializeField] private TMP_InputField userInput;
        [SerializeField] private TMP_InputField passInput;

        private readonly ISubject<Unit> onEnabled = new Subject<Unit>();
        private readonly ISubject<Unit> onDisabled = new Subject<Unit>();
        private readonly ISubject<Unit> onDisposed = new Subject<Unit>();
        private readonly ISubject<Unit> submitButtonSubject = new Subject<Unit>();

        protected override void OnInit()
        {
            submitButton
                .OnClickAsObservable()
                .Subscribe(submitButtonSubject);

            this.CreatePresenter<AuthFlowPresenter, IAuthFlowScreen>();
        }

        public IObservable<Unit> OnSubmit => submitButtonSubject;

        public string UserFieldText
        {
            get => userInput.text;
            set => userInput.text = value;
        }

        public string PassFieldText => passInput.text;


        public IObservable<Unit> OnEnabled => onEnabled;
        public IObservable<Unit> OnDisabled => onDisabled;
        public IObservable<Unit> OnDisposed => onDisposed;


        protected override void OnShow()
        {
            onEnabled.OnNext(Unit.Default);
        }

        protected override void OnHide()
        {
            onDisabled.OnNext(Unit.Default);
        }

        protected override void OnDispose()
        {
            onDisposed.OnNext(Unit.Default);
        }
    }
}
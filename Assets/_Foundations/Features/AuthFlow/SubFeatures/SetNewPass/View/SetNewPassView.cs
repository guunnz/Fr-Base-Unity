using System;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using AuthFlow.SetNewPass.Presentation;
using UI.DataField;
using UniRx;
using UnityEngine.UI;

namespace AuthFlow.SetNewPass.View
{
    public class SetNewPassView : ViewNode, ISetNewPassView
    {
        public DataField<string> newPassData;
        public DataField<string> confirmPassData;
        public Button submitButton;

        protected override void OnInit() => this.CreatePresenter<SetNewPassPresenter, ISetNewPassView>();

        public IDataField<string> NewPass => newPassData;
        public IDataField<string> ConfirmPass => confirmPassData;
        public IObservable<Unit> OnSubmit => submitButton.OnClickAsObservable();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using AuthFlow.AboutYou.Presentation;
using UI.DataField;
using UI.DateField;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace AuthFlow.AboutYou.View
{
    public class AboutYouView : ViewNode, IAboutYouView
    {
        public List<GameObject> birthGenderComponents;

        public DataField<string> firstNameField;
        public DataField<string> lastNameField;
        public DataField<string> userNameInputField;

        public GameObject loadingPanel;

        public DateField birthDayField;
        public ModalDropdown genderDropdown;

        public Button submitButton;
        private bool CannotSubmit;

        protected override void OnInit()
        {
            this.CreatePresenter<AboutYouPresenter, IAboutYouView>();
        }

        public void SetLoadingPanelActive(bool state)
        {
            loadingPanel.SetActive(state);


        }



        public void SelectContinue()
        {
            StartCoroutine(IContinue());
        }

        public void StopContinue()
        {
            CannotSubmit = true;
        }


        private IEnumerator IContinue()
        {
            yield return new WaitForFixedUpdate();
            if (!CannotSubmit)
            {
                submitButton.onClick.Invoke();
            }
            else
            {
                CannotSubmit = false;
            }
        }

        public void ShowNames()
        {
            lastNameField.gameObject.SetActive(true);
            firstNameField.gameObject.SetActive(true);
            birthGenderComponents.ForEach(SetActive(false));
            userNameInputField.gameObject.SetActive(false);
        }

        public void ShowBirthGender()
        {
            lastNameField.gameObject.SetActive(false);
            firstNameField.gameObject.SetActive(false);
            birthGenderComponents.ForEach(SetActive(true));
            userNameInputField.gameObject.SetActive(false);
        }

        public void ShowUserName()
        {
            lastNameField.gameObject.SetActive(false);
            firstNameField.gameObject.SetActive(false);
            birthGenderComponents.ForEach(SetActive(false));
            userNameInputField.gameObject.SetActive(true);
        }


        Action<GameObject> SetActive(bool active) => go => { go.SetActive(active); };

        public IObservable<Unit> OnSubmitSection => submitButton.OnClickAsObservable();
        public int GenderIndex
        {
            get => genderDropdown.Selected;
            set => genderDropdown.Selected = value;
        }


        public DateTime BirthDate => birthDayField.Date;
        public IDataField<string> UserName => userNameInputField;
        public IDataField<string> FirstName => firstNameField;
        public IDataField<string> LastName => lastNameField;


        public void SetGenders(List<string> genders)
        {
            genderDropdown.Options = genders;
        }
    }
}

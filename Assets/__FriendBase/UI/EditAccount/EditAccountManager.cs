using System;
using UnityEngine;
using BurguerMenu.Core.Domain;
using UnityEngine.UI;


namespace UI.EditAccount
{
    public class EditAccountManager : MonoBehaviour
    {
        public static event Action OnExitSection;

        [Header("Sections References")] [SerializeField]
        private AbstractUIPanel panelLanguage;

        [SerializeField] private AbstractUIPanel panelEmailChange;
        [SerializeField] private AbstractUIPanel forgotPasswordPanel;
        [SerializeField] private AbstractUIPanel panelSuccess;
        [SerializeField] private AbstractUIPanel panelDeleteAccount;
        [SerializeField] private GameObject passwordSection;

        [Header("Home")] [SerializeField] private GameObject home;


        [Header("Buttons")] [SerializeField] private Button languageButton;
        [SerializeField] private Button emailChangeButton;
        [SerializeField] private Button passwordButton;
        [SerializeField] private Button deleteAccountButton;
        [SerializeField] private Button closeButton;


        private void OnEnable()
        {
            home.SetActive(true);
            
            passwordButton.onClick.AddListener(() => ShowSection(EditAccountSections.ChangePassword));
            emailChangeButton.onClick.AddListener(() => ShowSection(EditAccountSections.ChangeEmail));
            deleteAccountButton.onClick.AddListener(() => ShowSection(EditAccountSections.DeleteAccount));
            closeButton.onClick.AddListener(Exit);
        }

        private void OnDisable()
        {
            passwordButton.onClick.RemoveAllListeners();
            emailChangeButton.onClick.RemoveAllListeners();
            deleteAccountButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();
        }


        public void ShowSection(EditAccountSections section)
        {
            CloseAllSections();
            switch (section)
            {
                case EditAccountSections.Home:
                    home.SetActive(true);
                    break;
                case EditAccountSections.ChangePassword:
                    passwordSection.SetActive(true);
                    break;
                case EditAccountSections.ChangeEmail:
                    panelEmailChange.Open();
                    break;
                case EditAccountSections.DeleteAccount:
                    panelDeleteAccount.Open();
                    break;
            }
        }

        public void CloseAllSections()
        {
            home.SetActive(false);
            panelLanguage.Close();
            passwordSection.SetActive(false);
            panelEmailChange.Close();
            forgotPasswordPanel.Close();
            panelSuccess.Close();
            panelDeleteAccount.Close();
        }

        public void Exit()
        {
            CloseAllSections();
            if (OnExitSection != null) OnExitSection.Invoke();
        }
    }
}
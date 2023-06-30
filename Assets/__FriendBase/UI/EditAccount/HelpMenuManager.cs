using System;
using BurguerMenu.Core.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace UI.EditAccount
{
    public class HelpMenuManager : MonoBehaviour
    {
        public static event Action OnExitSection;

        [Header("Home")] [SerializeField] private GameObject home;
        
        [Header("Sections")]
        [SerializeField] private GameObject about;
        [SerializeField] private Button closeButton;

        [Header("Buttons")] [SerializeField] private Button aboutButton;
        [SerializeField] private Button aboutBackButton;

        private void OnEnable()
        {
            ShowSection(HelpSections.Home);
            
            aboutButton.onClick.AddListener( () => ShowSection(HelpSections.About));
            aboutBackButton.onClick.AddListener(() => ShowSection(HelpSections.Home));
            closeButton.onClick.AddListener(Exit);
        }
        
        private void OnDisable()
        {
            closeButton.onClick.RemoveAllListeners();
            aboutButton.onClick.RemoveAllListeners();
            aboutBackButton.onClick.RemoveAllListeners();
        }

        private void ShowSection( HelpSections sections)
        {
            CloseAllSections();
            switch (sections)
            {
                case HelpSections.Home:
                    home.SetActive(true);
                    break;
                case HelpSections.About:
                    about.SetActive(true);
                    break;
            }
        }

        private void CloseAllSections()
        {
            home.SetActive(false);
            about.SetActive(false);
        }

        private void Exit()
        {
            CloseAllSections();
            if (OnExitSection != null) OnExitSection.Invoke();
        }
    }
}
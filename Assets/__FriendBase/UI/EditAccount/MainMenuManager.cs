using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.EditAccount
{
    public class MainMenuManager : MonoBehaviour
    {
        public static event Action OnExitSection;

        [SerializeField] private Button closeButton;

        private void OnEnable()
        {
            closeButton.onClick.AddListener(Exit);
        }

        private void Exit()
        {
            OnExitSection?.Invoke();
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveAllListeners();
        }
    }
}
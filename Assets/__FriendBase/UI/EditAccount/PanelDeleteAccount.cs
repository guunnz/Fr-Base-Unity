using Architecture.Injector.Core;
using AuthFlow;
using BurguerMenu.Infractructure;
using UnityEngine;
using UnityEngine.UI;

namespace UI.EditAccount
{
    public class PanelDeleteAccount : AbstractUIPanel
    {
        [SerializeField] EditAccountManager editAccountManager;

        [Header("Buttons")] 
        [SerializeField] private Button confirmDelete;
        [SerializeField] private Button cancelDelete;
        [SerializeField] private Button close;

        private void OnEnable()
        {
            confirmDelete.onClick.AddListener(TryDeleteAccount);
            cancelDelete.onClick.AddListener(Exit);
            close.onClick.AddListener(Exit);
        }

        private async void TryDeleteAccount()
        {
            var deleteTask = BurguerMenuWebClient.DeleteAccount();
            await deleteTask;

            
            if (!deleteTask.Result) return;
            
            //Todo: remove this line or replace logic when new auth be ready
            Injection.Get<IAuthStateManager>().Email = "";

            JesseUtils.Logout();
        }

        private void OnDisable()
        {
            confirmDelete.onClick.RemoveAllListeners();
            cancelDelete.onClick.RemoveAllListeners();
            close.onClick.RemoveAllListeners();
        }

        void Exit()
        {
            Close();
            editAccountManager.Exit();
        }
    }
}
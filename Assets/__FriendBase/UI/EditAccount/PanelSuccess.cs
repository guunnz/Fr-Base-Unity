using UnityEngine;
using UnityEngine.UI;

namespace UI.EditAccount
{
    public class PanelSuccess : AbstractUIPanel
    {
        public EditAccountManager editAccountManager;

        [SerializeField] private Button ok;
        [SerializeField] private Button close;

        private void OnEnable()
        {
            ok.onClick.AddListener(Exit);
            close.onClick.AddListener(Exit);
        }

        private void OnDisable()
        {
            ok.onClick.RemoveAllListeners();
            close.onClick.RemoveAllListeners();
        }

        public void Exit()
        {
            Close();
            editAccountManager.Exit();
        } 
    }
}
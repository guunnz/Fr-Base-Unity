using System.Collections.Generic;
using Architecture.Injector.Core;
using BurguerMenu.View;
using Data;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace WorldAreasCanvas
{
    public class MenusSwitcher : MonoBehaviour
    {
        [SerializeField] List<Button> buttons;

        [SerializeField] Button buyButton;
        [SerializeField] Button burguerButton;
        [SerializeField] BurguerView burguerView;
        [SerializeField] ChatView.View.ChatView chatView;
        [SerializeField] UIStoreManager storeManager;

        IGameData gameData;
        readonly CompositeDisposable disposables = new CompositeDisposable();

        [SerializeField] TextMeshProLabelWidget txtCurrentGems;

        void Awake()
        {
            gameData = Injection.Get<IGameData>();
        }

        private void OnEnable()
        {
            foreach (var button in buttons)
            {
                button.OnClickAsObservable().Subscribe(SwitchViews).AddTo(disposables);
            }

            buyButton.OnClickAsObservable().Subscribe(storeManager.Open)
                .AddTo(disposables);

            txtCurrentGems.Value = gameData.GetUserInformation().Gems.ToString();


            burguerButton.OnClickAsObservable()
                .Do(SwitchViews)
                .Do(_ => burguerView.gameObject.SetActive(true))
                .Subscribe().AddTo(disposables);
        }

        void UpdateGemsCounter(int amount)
        {
            txtCurrentGems.Value = amount.ToString();
        }

        void SwitchViews()
        {
            chatView.CloseChat();
        }

        private void OnDisable()
        {
            disposables.Clear();
        }
    }
}
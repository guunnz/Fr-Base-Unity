using Architecture.Injector.Core;
using Architecture.MVP;
using Architecture.ViewManager;
using MainMenu;
using Settings.Presentation;
using Shared.View;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Settings.View
{
    public class SettingsView : ViewNode, ISettingsScreen
    {
        [SerializeField] private ReactiveDropdown difficultyLevelDropdown;
        [SerializeField] private ReactiveDropdown videoQualityLevelDropdown;
        [SerializeField] private ReactiveSlider mainVolumeSlider;
        [SerializeField] private ReactiveSlider sfxVolumeSlider;
        [SerializeField] private ReactiveSlider musicVolumeSlider;
        [SerializeField] private ReactiveToggle fullScreenToggle;

        [SerializeField] private Button backButton;

        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private Presenter presenter;

        private IViewManager viewManager;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) GoBack();
        }

        public IReactiveProperty<float> MusicVol => musicVolumeSlider;
        public IReactiveProperty<float> SfxVol => sfxVolumeSlider;
        public IReactiveProperty<float> MasterVol => mainVolumeSlider;
        public IReactiveProperty<int> Difficulty => difficultyLevelDropdown;
        public IReactiveProperty<int> Quality => videoQualityLevelDropdown;
        public IReactiveProperty<bool> FullScreen => fullScreenToggle;

        protected override void OnInit()
        {
            viewManager = Injection.Get<IViewManager>();
            Injection.Register((ISettingsScreen) this);
            presenter = Injection.Create<SettingsPresenter>();
            presenter.OnInit();
            Debug.Log("asdsad");
        }

        protected override void OnShow()
        {
            presenter.OnShow();

            backButton
                .OnPointerClickAsObservable()
                .Do(_ => viewManager.Show<MainMenuView>())
                .Subscribe()
                .AddTo(disposables);
        }

        private void GoBack()
        {
            viewManager.Show<MainMenuView>();
        }

        protected override void OnHide()
        {
            presenter.OnHide();
            disposables.Clear();
        }
    }
}
using Architecture.MVP;
using JetBrains.Annotations;
using Settings.Domain;
using Settings.Services;
using UniRx;
using UnityEngine;

namespace Settings.Presentation
{
    [UsedImplicitly]
    public class SettingsPresenter : Presenter
    {
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        private readonly ISettingsScreen screen;
        private readonly ISettingsRepository settingsRepository;

        public SettingsPresenter(ISettingsRepository settingsRepository, ISettingsScreen screen)
        {
            this.settingsRepository = settingsRepository;
            this.screen = screen;
        }

        public override void OnShow()
        {
            Present();
            BindScreenInput();
        }

        private void Present()
        {
            var settings = settingsRepository.LoadSettings();
            screen.MusicVol.Value = settings.audioSettings.musicVol;
            screen.SfxVol.Value = settings.audioSettings.sfxVol;
            screen.MasterVol.Value = settings.audioSettings.masterVol;
            screen.Difficulty.Value = (int) settings.gameSettings.difficulty;
            screen.Quality.Value = (int) settings.videoSettings.quality;
            screen.FullScreen.Value = settings.videoSettings.fullScreen;
        }

        private void BindScreenInput()
        {
            screen.MusicVol.Subscribe(_ => UpdateRepo()).AddTo(disposables);
            screen.SfxVol.Subscribe(_ => UpdateRepo()).AddTo(disposables);
            screen.MasterVol.Subscribe(_ => UpdateRepo()).AddTo(disposables);
            screen.Difficulty.Subscribe(_ => UpdateRepo()).AddTo(disposables);
            screen.Quality.Subscribe(_ => UpdateRepo()).AddTo(disposables);
            screen.FullScreen.Do(FullScreenChange).Subscribe(_ => UpdateRepo()).AddTo(disposables);
        }

        private void FullScreenChange(bool isFull)
        {
            Screen.fullScreen = isFull;
        }

        private void UpdateRepo()
        {
            var settings = new SettingsModel
            {
                audioSettings =
                {
                    musicVol = screen.MusicVol.Value,
                    sfxVol = screen.SfxVol.Value,
                    masterVol = screen.MasterVol.Value
                },
                gameSettings =
                {
                    difficulty = (DifficultyLevel) screen.Difficulty.Value
                },
                videoSettings =
                {
                    quality = (VideoQuality) screen.Quality.Value, fullScreen = screen.FullScreen.Value
                }
            };
            settingsRepository.SaveSettings(settings);
        }

        public override void OnHide()
        {
            disposables.Clear();
        }
    }
}
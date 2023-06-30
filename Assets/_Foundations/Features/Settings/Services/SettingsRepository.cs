using System;
using Settings.Domain;
using UniRx;
using UnityEngine;

namespace Settings.Services
{
    public interface ISettingsRepository
    {
        SettingsModel LoadSettings();
        void SaveSettings(SettingsModel settings);
        IObservable<SettingsModel> OnChange();
    }


    public class LocalSettingsRepository : ISettingsRepository
    {
        private const string SettingsKey = "__settings__";


        private static readonly SettingsDTO DefaultSettings = new SettingsDTO
        {
            fullScreen = true,
            quality = 1,
            masterVol = 1,
            musicVol = 1,
            sfxVol = 1,
            difficulty = (int) DifficultyLevel.Normal
        };

        private readonly ISubject<SettingsModel> subject = new Subject<SettingsModel>();


        public SettingsModel LoadSettings()
        {
            var settings = PlayerPrefs.GetString(SettingsKey, string.Empty);

            var settingsDTO = string.IsNullOrEmpty(settings)
                ? DefaultSettings
                : JsonUtility.FromJson<SettingsDTO>(settings);

            var model = new SettingsModel
            {
                videoSettings = new VideoSettingsModel
                {
                    quality = (VideoQuality) settingsDTO.quality,
                    fullScreen = settingsDTO.fullScreen
                },
                audioSettings = new AudioSettingsModel
                {
                    sfxVol = settingsDTO.sfxVol,
                    musicVol = settingsDTO.musicVol,
                    masterVol = settingsDTO.masterVol
                },
                gameSettings = new GameSettingsModel
                {
                    difficulty = (DifficultyLevel) settingsDTO.difficulty
                }
            };
            return model;
        }

        public void SaveSettings(SettingsModel settings)
        {
            var dto = new SettingsDTO
            {
                difficulty = (int) settings.gameSettings.difficulty,
                quality = (int) settings.videoSettings.quality,
                fullScreen = settings.videoSettings.fullScreen,
                sfxVol = settings.audioSettings.sfxVol,
                musicVol = settings.audioSettings.musicVol,
                masterVol = settings.audioSettings.masterVol
            };
            var json = JsonUtility.ToJson(dto);
            PlayerPrefs.SetString(SettingsKey, json);
            PlayerPrefs.Save();
            subject.OnNext(settings);
        }

        public IObservable<SettingsModel> OnChange()
        {
            return subject;
        }
    }

    [Serializable]
    public struct SettingsDTO
    {
        public float masterVol;
        public float musicVol;
        public float sfxVol;
        public int quality;
        public bool fullScreen;
        public int difficulty;
    }
}
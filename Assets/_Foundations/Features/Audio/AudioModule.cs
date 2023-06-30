using Architecture.Context;
using Architecture.Injector.Core;
using Audio.Music;
using Audio.SFX;
using System.Collections;
using UnityEngine;

namespace Audio
{
    public class AudioModule : ScriptModule
    {
        [SerializeField] private UnityMusicPlayer musicPlayer;
        [SerializeField] private UnitySfxPlayer sfxPlayer;
        private bool soundOff;

        private void OnValidate()
        {
            musicPlayer.gameObject.SetActive(false);
            sfxPlayer.gameObject.SetActive(false);
        }

        public override void Init()
        {
            musicPlayer.gameObject.SetActive(true);
            sfxPlayer.gameObject.SetActive(true);
            Injection.Register<IMusicPlayer>(musicPlayer);
            Injection.Register<ISfxPlayer>(sfxPlayer);
            //SetVolume();
        }

        private void Start()
        {
            SetVolume();
        }

        private void SetVolume()
        {
            soundOff = PlayerPrefs.GetInt(Settings.PlayerPrefsValues.SoundOff) == 1;
            Debug.LogWarning("sound" + soundOff);
            if (soundOff)
            {
                MuteAudio();
            }
        }

        public bool IsAudioMuted()
        {
            return soundOff;
        }

        private void MuteAudio()
        {
            musicPlayer.Mute();
            sfxPlayer.Mute();
        }

        public void ToggleSound()
        {
            soundOff = !soundOff;
            PlayerPrefs.SetInt(Settings.PlayerPrefsValues.SoundOff, soundOff == true ? 1 : 0);
            musicPlayer.ToggleVolume();
            sfxPlayer.ToggleVolume();
        }
    }
}
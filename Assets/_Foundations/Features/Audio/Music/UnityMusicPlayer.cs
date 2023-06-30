using System.Collections;
using Audio.Core;
using UnityEngine;

namespace Audio.Music
{
    public class UnityMusicPlayer : MonoBehaviour, IMusicPlayer
    {
        public MusicVolumeControl deckA;
        public MusicVolumeControl deckB;

        [SerializeField] public AudioClip mainMenuAudioClip;
        private AudioClip currentClip;

        private MusicVolumeControl currentDeck;


        private Coroutine currentMix;
        private MusicVolumeControl nextDeck;

        private void Awake()
        {
            currentDeck = deckA;
            nextDeck = deckB;
        }

        private void Start()
        {
            Play(mainMenuAudioClip);
        }

        public void Play(AudioClip clip, float fadeDuration = 0)
        {
            if (fadeDuration > 0 && IsMixing())
            {
                Debug.LogWarning("you are trying to mix trackId while another mix is running, performing immediate mix");
                fadeDuration = 0;
            }

            if (currentClip == clip)
            {
                Debug.LogWarning(
                    $"You are trying to play {(clip ? clip.name : "")} when you are already playing that one");
            }

            currentClip = clip;
            if (fadeDuration <= 0)
                MixNow();
            else
                PerformMix(fadeDuration);
        }

        public void Stop()
        {
            currentDeck.Stop();
            nextDeck.Stop();
        }

        public void ToggleVolume()
        {
            deckA.gameObject.SetActive(!deckA.gameObject.activeSelf);
            deckB.gameObject.SetActive(!deckB.gameObject.activeSelf);
            MixNow();
        }

        public void Mute()
        {
            deckA.gameObject.SetActive(false);
            deckB.gameObject.SetActive(false);
        }

        public bool IsMixing()
        {
            return currentMix != null;
        }

        private void MixNow()
        {
            if (IsMixing())
            {
                StopCoroutine(currentMix);
                currentMix = null;
            }

            nextDeck.Stop();
            nextDeck.SetClip(currentClip);
            nextDeck.Vol = 1;
            nextDeck.Play();
            currentDeck.Stop();
            SwapDecks();
        }

        private void PerformMix(float fade)
        {
            IEnumerator Mix()
            {
                nextDeck.Stop();
                nextDeck.SetClip(currentClip);
                nextDeck.Play();
                nextDeck.Vol = 0;
                var totalTime = fade;
                while (fade >= 0)
                {
                    fade -= Time.deltaTime;
                    var t = fade / totalTime;
                    currentDeck.Vol = t;
                    nextDeck.Vol = 1 - t;
                    yield return null;
                }

                nextDeck.Vol = 1;
                currentDeck.Stop();
                SwapDecks();
                currentMix = null;
            }

            currentMix = StartCoroutine(Mix());
        }

        private void SwapDecks()
        {
            var aux = currentDeck;
            currentDeck = nextDeck;
            nextDeck = aux;
        }
    }
}
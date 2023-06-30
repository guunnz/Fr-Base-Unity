using UnityEngine;

namespace Managers.InRoomGems
{
    [RequireComponent(typeof(AudioSource))]
    public class InRoomGemsSFXController : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] private AudioClip[] audioClips;
        private Audio.AudioModule AM;
        private void Start()
        {
            AM = FindObjectOfType<Audio.AudioModule>();
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayPickGem()
        {
            if (AM.IsAudioMuted())
                return;
            audioSource.clip = audioClips[0];
            audioSource.Play();
        }

        public void PlayNewGem()
        { if (AM.IsAudioMuted())
                return;
            audioSource.clip = audioClips[1];
            audioSource.Play();
        }
    }
}

using UnityEngine;

namespace Audio.Music
{
    public interface IMusicPlayer
    {
        void Play(AudioClip audioClip, float fadeDuration = 0);
        void Stop();
    }
}
using Audio.Core;
using UnityEngine;

namespace Audio.SFX
{
    public class UnitySfxPlayer : MonoBehaviour, ISfxPlayer
    {
        [SerializeField] private UnitySfxPlayerPool pool;
        [SerializeField] private ClipsCollection tracks;

        public void Play(string sfxKey, float vol = 1)
        {
            if (!tracks.TryGetClip(sfxKey, out var clip))
            {
                Debug.Log("You are trying to play sfx : \"" + sfxKey + " \"but there is not matching clips");
                return;
            }

            pool.Play(clip, vol);
        }

        public void ToggleVolume()
        {
            foreach (VolumeControl volC in pool.GetVolumeControlList())
            {
                if (volC == null)
                    continue;
                volC.gameObject.SetActive(!volC.gameObject.activeSelf);

            }
        }

        public void Mute()
        {
            foreach (VolumeControl volC in pool.GetVolumeControlList())
            {
                if (volC == null)
                    continue;
                volC.gameObject.SetActive(false);
            }
        }
    }
}
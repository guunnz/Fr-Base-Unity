using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RacingMusicPlayer : MonoBehaviour
{
    [SerializeField] List<RacingSound> SoundList;

    [SerializeField] AudioSource _AudioSource;


    public void PlayIntro()
    {
        StartCoroutine(IPlayIntro());
    }
    public void playAudio(eRacingSound sound)
    {
        _AudioSource.clip = SoundList.Single(x => x.racingSound == sound).AudioClip;
        _AudioSource.Play();
    }

    private IEnumerator IPlayIntro()
    {
        playAudio(eRacingSound.FA_Elite_Stadium_Intro);
        yield return new WaitForSeconds(7.5f);
        playAudio(eRacingSound.FA_Elite_Stadium_FULL_Loop);
        _AudioSource.loop = true;
    }
}

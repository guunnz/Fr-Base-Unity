using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class FinishLineSpaceJump : MonoBehaviour
{
    [HideInInspector] public SpaceJumpMinigame miniGame;
    [SerializeField] private Transform Rocket;
    [SerializeField] private ParticleSystem RocketParticles;

    //Temporary
    [SerializeField] AudioClip RocketSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] CinemachineVirtualCamera cam;
    private bool soundOff; //PLEASE REMOVE AFTER PROPER SOUND IMPLEMENTATION PLEASE

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            soundOff = PlayerPrefs.GetInt(Settings.PlayerPrefsValues.SoundOff) == 1;

            if (!soundOff)
            {
                audioSource.clip = RocketSound;
                audioSource.Play();
                RocketParticles.Play();
            }

            cam.Follow = Rocket;
            Rocket.DOMove(new Vector3(this.transform.position.x, this.transform.position.y + 60, this.transform.position.z), 4f);
            collision.gameObject.SetActive(false);
            Invoke("EndGame", 3f);
        }
        else if (collision.CompareTag(Tags.AI))
        {
            miniGame.OpponentWin();
        }
    }


    private void EndGame()
    {
        miniGame.UserEnds();
    }
}

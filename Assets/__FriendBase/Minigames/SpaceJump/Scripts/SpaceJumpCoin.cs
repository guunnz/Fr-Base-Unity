using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceJumpCoin : MonoBehaviour
{
    [SerializeField] Animator animator;


    //temporary
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip coin1;
    [SerializeField] AudioClip coin2;
    private bool PickedUp;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player) && !PickedUp)
        {
            PickedUp = true;
            animator.SetTrigger(AnimationStates.CoinPickup);
            PlayerPrefs.SetInt("Bonus", PlayerPrefs.GetInt("Bonus") + 1);
            Invoke("Destroy", 0.6f);
            if (Random.Range(0, 1) == 0)
            {
                audioSource.clip = coin1;
            }
            else
            {
                audioSource.clip = coin2;
            }
            audioSource.Play();
        }
    }

    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}

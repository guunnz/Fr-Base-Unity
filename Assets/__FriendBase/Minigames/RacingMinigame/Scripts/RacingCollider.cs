using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RacingCollider : MonoBehaviour
{
    public CarController MyCar;
    public bool ImRight;
    public bool ImFront;
    [SerializeField] List<RacingSound> SoundList;

    [SerializeField] AudioSource _AudioSource;

    public void playAudio(eRacingSound sound)
    {
        _AudioSource.clip = SoundList.Single(x => x.racingSound == sound).AudioClip;
        _AudioSource.Play();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            CarController collisionCar = collision.GetComponent<CarController>();
            if (collisionCar == MyCar)
                return;
            if (ImFront)
            {
                
                StartCoroutine(CrashedOpponentFront(collisionCar));
            }
            else
            {
                if (ImRight)
                {
                    MyCar.CanMoveRight = false;
                }
                else
                {
                    MyCar.CanMoveLeft = false;
                }
            }

        }
    }

    private IEnumerator CrashedOpponentFront(CarController collisionCar)
    {
        playAudio(eRacingSound.FA_Funny_Impact_1_2);
        MyCar.DoCarAnimation(CarController.CarAnimations.Hit);
        MyCar.myPathFollower.speed = 0f;
        collisionCar.myPathFollower.speed += 10f;
        yield return null;
        //collisionCar.immune = true;
        //yield return new WaitForSeconds(1);
        //collisionCar.immune = false;
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            if (ImFront)
                return;

            if (ImRight)
            {
                MyCar.CanMoveRight = true;
            }
            else
            {
                MyCar.CanMoveLeft = true;
            }

        }
    }
}
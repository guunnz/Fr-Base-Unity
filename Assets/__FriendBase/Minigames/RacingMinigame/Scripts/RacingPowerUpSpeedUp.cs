using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingPowerUpSpeedUp : RacingPowerUp
{


    [SerializeField] private float SpeedUp;
    [SerializeField] private float Duration;
    [SerializeField] Animator SpeedUpAnimator;

    public override void EnablePowerup()
    {
        StartCoroutine(ISpeedUp());
    }

    private IEnumerator ISpeedUp()
    {
        float Timer = Duration;
        float startSpeed = carThatPickedUp.GetStartMaxSpeed();

        carThatPickedUp.DoKartSpeedUp();

        carThatPickedUp.canDeaccelerate = false;
        SpeedUpAnimator.SetTrigger("Pick");
        while (Timer >= 0)
        {
            carThatPickedUp.SetSpeed(SpeedUp);
            Timer -= Time.deltaTime;
            yield return null;
        }
        carThatPickedUp.SetSpeed(startSpeed);
        carThatPickedUp.canDeaccelerate = true;
    }
}

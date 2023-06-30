using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingPowerUp : MonoBehaviour
{

    internal CarController carThatPickedUp;

    public enum PowerUpType
    {
        None = 0,
        SpeedUp = 1,
        SpeedDown = 2,
        Coin = 3,
    }

    public virtual void EnablePowerup()
    {

    }

}
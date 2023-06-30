using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingPowerUpSpeedDown : RacingPowerUp
{
    [SerializeField] private float SpeedDown;
    [SerializeField] private float Duration;
    [SerializeField] MeshRenderer Renderer;
    [SerializeField] List<Material> materialList;

    private void Start()
    {
        Renderer.material = materialList[Random.Range(0, materialList.Count)];
    }

    public override void EnablePowerup()
    {
        StartCoroutine(ISpeedDown());
    }

    private IEnumerator ISpeedDown()
    {
        if (carThatPickedUp.immune)
            yield break;
        carThatPickedUp.canDeaccelerate = true;
        carThatPickedUp.DoCarAnimation(CarController.CarAnimations.Spin);
        float Speed = carThatPickedUp.GetStartMaxSpeed();
        carThatPickedUp.SetSpeed(SpeedDown);
        yield return new WaitForSeconds(1f);
        carThatPickedUp.SetSpeed(Speed, true);
    }
}
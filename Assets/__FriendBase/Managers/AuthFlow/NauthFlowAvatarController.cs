using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NauthFlowAvatarController : MonoBehaviour
{
    [SerializeField] private AvatarAnimationController avatarAnimationController;

    public enum possibleAnimations { SetHelloState = 0, SetTalkAnimation = 1, Dance = 2, Dance2 = 3 };

    void Start()
    {
        avatarAnimationController.SetIdleState();
        StartCoroutine(LoopAnimations());
    }

    IEnumerator LoopAnimations()
    {
        int minValue = 3;
        int maxValue = 7;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            int seconds = UnityEngine.Random.Range(minValue, maxValue);
            yield return new WaitForSeconds(seconds);
            possibleAnimations animation = GetRandomAnimation();
            switch (animation)
            {
                case possibleAnimations.SetHelloState:
                    avatarAnimationController.SetHelloState();
                    break;
                case possibleAnimations.SetTalkAnimation:
                    avatarAnimationController.SetTalkAnimation(UnityEngine.Random.Range(2, 5));
                    break;
                case possibleAnimations.Dance:
                    avatarAnimationController.SetDanceState();
                    break;
                case possibleAnimations.Dance2:
                    avatarAnimationController.SetDance2State();
                    break;
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(5, 9));
            avatarAnimationController.SetIdleState();
        }
    }

    public possibleAnimations GetRandomAnimation()
    {
        int enumItemCount = Enum.GetValues(typeof(possibleAnimations)).Length;
        int animation = UnityEngine.Random.Range(0, enumItemCount);
        return (possibleAnimations)animation;
    }
}

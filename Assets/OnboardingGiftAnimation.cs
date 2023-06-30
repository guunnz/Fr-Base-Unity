using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class OnboardingGiftAnimation : MonoBehaviour
{
    [SerializeField] Transform GiftTarget;
    [SerializeField] Image Hand;


    IEnumerator EnableHand()
    {
        yield return new WaitForSeconds(3f);
        Hand.DOColor(Color.white, 0.5f);
    }

    IEnumerator Start()
    {
        this.transform.DOMove(GiftTarget.position, 0.5f);
        this.GetComponent<Image>().DOBlendableColor(Color.white, 0.5f);
        StartCoroutine(EnableHand());
        while (gameObject.activeSelf)
        {
            this.transform.DOShakePosition(0.5f, 4);
            yield return new WaitForSeconds(1f);
        }
    }
}

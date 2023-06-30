using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class OnboardingCirclesAnimation : MonoBehaviour
{

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }


    public IEnumerator HandAnimation()
    {
        image.DOBlendableColor(new Color(1, 1, 1, 0.2f), 0.2f);
        yield return new WaitForSeconds(0.5f);
        image.DOBlendableColor(new Color(1, 1, 1, 1), 1.2f);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hand"))
        {
            StartCoroutine(HandAnimation());
        }
    }
}

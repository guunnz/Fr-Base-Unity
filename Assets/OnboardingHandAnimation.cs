using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class OnboardingHandAnimation : MonoBehaviour
{
    [SerializeField] Transform target;

    private IEnumerator Start()
    {
        Vector2 startPos = this.transform.localPosition;
        Vector2 targetPos = target.localPosition;

        while (gameObject.activeSelf)
        {
            this.transform.DOLocalMove(targetPos, 0.7f);
            yield return new WaitForSeconds(0.8f);
            this.transform.DOLocalMove(startPos, 0.7f);
            yield return new WaitForSeconds(1.2f);
        }
    }
}

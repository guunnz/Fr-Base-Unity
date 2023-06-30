using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DolphinController : MonoBehaviour
{
    [SerializeField] private GameObject dolphinContainer;
    [SerializeField] private Transform[] listPositions;

    private Animator animator;

    void Start()
    {
        animator = dolphinContainer.GetComponent<Animator>();
        StartCoroutine(SetNextAnimation());
    }

    IEnumerator SetNextAnimation()
    {
        dolphinContainer.SetActive(false);

        yield return new WaitForSeconds(Random.Range(3, 6));

        //Show
        dolphinContainer.SetActive(true);
        //Set random animation
        int animationId = Random.Range(1, 5);
        animator.SetTrigger("Jump_" + animationId);

        //Set Random position
        int indexPosition = Random.Range(0, listPositions.Length);
        Vector3 position = transform.position;
        position.x = listPositions[indexPosition].position.x;
        transform.position = position;

        //Set random orientation
        Vector3 localScale = transform.localScale;
        if (Random.Range(0, 1000)>500)
        {
            localScale.x = 1;
        }
        else
        {
            localScale.x = -1;
        }
        transform.localScale = localScale;

        yield return new WaitForSeconds(5);

        StartCoroutine(SetNextAnimation());
    }
}

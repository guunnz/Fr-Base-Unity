using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class CharacterShowcaser : MonoBehaviour
{
    [SerializeField] private List<GameObject> characters;
    [SerializeField] private Transform startingPoint1;
    [SerializeField] private Transform startingPoint2;
    [SerializeField] private Transform endPoint1;
    [SerializeField] private Transform endPoint2;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private string animationTrigger = "PlayAnimation";
    [SerializeField] private float animationDuration = 2f;

    private GameObject lastCharacter;
    private int lastCharacterIndex = -1;
    private int lastLASTCharacterIndex = -1;
    private Transform currentEndPoint;
    private Transform currentStartingPoint;
    private Transform lastCharacterStartingPoint;

    private void Start()
    {
        currentEndPoint = endPoint1;
        currentStartingPoint = startingPoint1;
        StartCoroutine(TeleportAndAnimate());
    }

    private IEnumerator TeleportAndAnimate()
    {
        while (true)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, characters.Count);
            } while (randomIndex == lastCharacterIndex || randomIndex == lastLASTCharacterIndex);

            GameObject currentCharacter = characters[randomIndex];
            currentCharacter.transform.position = currentStartingPoint.position;

            SetCharacterDirection(currentCharacter, currentStartingPoint, currentEndPoint);

            currentCharacter.transform.DOMove(currentEndPoint.position, moveDuration);
            currentCharacter.GetComponent<Animator>().SetTrigger("Walk");
            yield return new WaitForSeconds(moveDuration);
            currentCharacter.GetComponent<Animator>().SetTrigger("Idle");
            if (lastCharacter != null)
            {
                // Play animation for both characters
                currentCharacter.GetComponent<Animator>().SetTrigger(animationTrigger);
                yield return new WaitForSeconds(animationDuration);
                lastCharacter.GetComponent<Animator>().SetTrigger(animationTrigger);
                yield return new WaitForSeconds(animationDuration);

                // Move last character back to their starting point
                SetCharacterDirection(lastCharacter, lastCharacter.transform, lastCharacterStartingPoint);

                lastCharacter.transform.DOMove(lastCharacterStartingPoint.position, moveDuration);
                lastCharacter.GetComponent<Animator>().SetTrigger("Walk");
                yield return new WaitForSeconds(moveDuration);
            }

            lastLASTCharacterIndex = lastCharacterIndex;
            lastCharacter = currentCharacter;
            lastCharacterIndex = randomIndex;
            lastCharacterStartingPoint = currentStartingPoint;

            // Swap starting points and endpoints for the next character
            if (currentEndPoint == endPoint1)
            {
                currentEndPoint = endPoint2;
                currentStartingPoint = startingPoint2;
            }
            else
            {
                currentEndPoint = endPoint1;
                currentStartingPoint = startingPoint1;
            }
        }
    }

    private void SetCharacterDirection(GameObject character, Transform from, Transform to)
    {
        Vector3 localScale = character.transform.localScale;
        float originalScaleX = Mathf.Abs(localScale.x);
        if (to.position.x > from.position.x)
        {
            localScale.x = originalScaleX;
        }
        else
        {
            localScale.x = -originalScaleX;
        }
        character.transform.localScale = localScale;
    }

}

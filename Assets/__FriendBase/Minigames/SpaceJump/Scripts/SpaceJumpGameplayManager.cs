using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceJumpGameplayManager : MonoBehaviour
{
    [SerializeField] GameObject platformPrefab;
    [SerializeField] GameObject platformJumpPrefab;
    [SerializeField] GameObject coinPrefab;
    [SerializeField] Transform AI;
    [SerializeField] Transform FinishLine;
    [SerializeField] Transform Gradient;
    [SerializeField] FinishLineSpaceJump FinishLineScript;

    [SerializeField] Transform PlatformsParent;
    [SerializeField] float minSpaceBetweenPlatforms;
    [SerializeField] float maxSpaceBetweenPlatforms;

    [SerializeField] float chanceForSpecialPlatform1In = 5;

    [SerializeField] float aiMinSpeed;
    [SerializeField] float aiMaxSpeed;

    [SerializeField] float maxXArea;
    [SerializeField] float minXArea;
    [SerializeField] float firstPlatformHeight = 5;
    [SerializeField] float levelYUnitsHeight = 100;
    [SerializeField] int minCoinAmount = 3;
    [SerializeField] int maxCoinAmount = 10;
    [SerializeField] float chanceForCoinPerPlatformSpawned1In = 10;

    [HideInInspector] public SpaceJumpMinigame spaceJumpMinigame;

    private bool gameFinished;

    private void Start()
    {
        StartCoroutine(GenerateLevel());
    }

    private IEnumerator GenerateLevel()
    {
        Gradient.localScale = new Vector3(Gradient.localScale.x, (levelYUnitsHeight + 80) / 20, Gradient.localScale.z);
        InitializeAI();
        FinishLineScript.miniGame = this.spaceJumpMinigame;
        Instantiate(platformPrefab, new Vector2(Random.Range(minSpaceBetweenPlatforms, maxSpaceBetweenPlatforms), firstPlatformHeight), Quaternion.identity, PlatformsParent);

        float heightGenerated = firstPlatformHeight;
        int coinsSpawned = 0;
        while (levelYUnitsHeight > heightGenerated)
        {
            float heightSelected = Random.Range(minSpaceBetweenPlatforms, maxSpaceBetweenPlatforms);
            float widthSelected = Random.Range(minXArea, maxXArea);
            heightGenerated += heightSelected;
            if ((int)Random.Range(0, chanceForSpecialPlatform1In) == 1)
            {
                Instantiate(platformJumpPrefab, new Vector2(widthSelected, heightSelected + heightGenerated), Quaternion.identity, PlatformsParent);

            }
            else
            {
                Instantiate(platformPrefab, new Vector2(widthSelected, heightSelected + heightGenerated), Quaternion.identity, PlatformsParent);
            }

            if ((int)Random.Range(0, chanceForCoinPerPlatformSpawned1In) == 1 && coinsSpawned < maxCoinAmount)
            {
                coinsSpawned++;
                Instantiate(coinPrefab, new Vector2(widthSelected, heightSelected + heightGenerated + 1.5f), Quaternion.identity, PlatformsParent);
            }

            yield return null;
        }

        while (coinsSpawned < minCoinAmount)
        {
            float widthSelected = Random.Range(minXArea, maxXArea);
            coinsSpawned++;
            Instantiate(coinPrefab, new Vector2(widthSelected, Random.Range(20, levelYUnitsHeight) + 1.5f), Quaternion.identity, PlatformsParent);
            yield return null;
        }

        FinishLine.position = new Vector3(FinishLine.position.x, heightGenerated + (maxSpaceBetweenPlatforms * 2), FinishLine.position.z);
    }

    private IEnumerator InitializeAICoroutine()
    {
        float speed = Random.Range(aiMinSpeed, aiMaxSpeed);
        while (!gameFinished)
        {
            AI.position = Vector3.MoveTowards(AI.position, new Vector3(AI.position.x, levelYUnitsHeight + firstPlatformHeight + 15, AI.position.z), speed);
            yield return new WaitForFixedUpdate();
        }
    }

    public void InitializeAI()
    {
        StartCoroutine(InitializeAICoroutine());
    }
}

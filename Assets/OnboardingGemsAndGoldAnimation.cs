using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Data;
using Architecture.Injector.Core;

public class OnboardingGemsAndGoldAnimation : MonoBehaviour
{
    private int GemCount;
    private int GoldCount;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI gemText;

    public List<Image> imageList;
    public List<TextMeshProUGUI> textList;
    private IGameData gameData;
    public Transform target;
    public IEnumerator Start()
    {
        gameData = Injection.Get<IGameData>();
        if (gameData.IsGuest())
        {
            GoldCount = 50;
            GemCount = 1000;
        }
        else
        {
            GoldCount = 100;
            GemCount = 2000;
        }
        this.transform.DOMove(target.transform.position, 1);
        imageList.ForEach(x => x.DOFade(1, 1));
        textList.ForEach(x => x.DOFade(1, 1));
        // Set the initial text to 0
        goldText.text = "0";
        gemText.text = "0";

        // Animate the text of text1 and text2 to their respective values
        float duration = 1f; // Set the duration of the animation
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            // Calculate the progress of the tween
            float progress = (Time.time - startTime) / duration;

            // Calculate the current value of the tween
            int currentValue1 = (int)Mathf.Lerp(0, GemCount, progress);
            int currentValue2 = (int)Mathf.Lerp(0, GoldCount, progress);

            // Update the text of text1 and text2 to the current value
            goldText.text = currentValue2.ToString();
            gemText.text = currentValue1.ToString();

            yield return null; // Wait for the next frame
        }

        // Set the final text to the target value
        goldText.text = GoldCount.ToString();
        gemText.text = GemCount.ToString();

        // Coroutine is done
        yield break;
    }
}

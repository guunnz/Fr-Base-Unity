using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    public VideoPlayer vid;

    public RawImage rawImage;
    public float fadeTime = 0.5f;  // Adjust to your needs

    private void Start()
    {
        // Start with a transparent RawImage
        Color c = rawImage.color;
        c.a = 0f;
        rawImage.color = c;

        // Start the video
        vid.Prepare();
        vid.loopPointReached += EndReached;
        vid.prepareCompleted += VideoPrepared;
    }

    private void VideoPrepared(VideoPlayer vp)
    {
        StartCoroutine(FadeIn());
        vid.Play();
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color c = rawImage.color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsedTime / fadeTime);
            rawImage.color = c;
            yield return null;
        }
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        vp.Stop();
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace ChatView.View
{
    //Extending EventTrigger class prevents events propagation to parent objects: yeaah!
    public class ScrollFadeTool : EventTrigger
    {
        
        [SerializeField] Image imageToFade;
        private const float fadeInTime = 0.2f;
        private const float fadeOutTime = 0.8f;

        private void Start()
        {
            imageToFade.color = new Color(1, 1, 1, 0);
        }

        //OnPointerDown is also required to receive OnPointerUp callbacks
        public override void OnPointerDown(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(FadeImage(false, imageToFade.color.a));
        }

        //Do this when the mouse click on this selectable UI object is released.
        public override void OnPointerUp(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(FadeImage(true, imageToFade.color.a));
        }

        IEnumerator FadeImage(bool fadeAway, float startValue)
        {
            if (fadeAway)
            {
                for (float i = startValue; i >= 0; i -= Time.deltaTime/fadeOutTime)
                {
                    imageToFade.color = new Color(1, 1, 1, i);
                    yield return null;
                }
            }
            else
            {
                for (float i = startValue; i <= 1; i += Time.deltaTime/fadeInTime)
                {
                    imageToFade.color = new Color(1, 1, 1, i);
                    yield return null;
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using LocalizationSystem;
using Architecture.Injector.Core;

namespace Onboarding
{
    [System.Serializable]
    public class OnboardingAnimator
    {
        public string trigger;
        public Animator Animator;
    }

    [System.Serializable]
    public class OnboardingAnimation
    {
        public float AnimationDuration;
        public bool ReplaceWithNextAnim;
        public List<OnboardingAnimator> ObjectsAnimationsOnStart;
        [Header("On Start")]
        public List<GameObject> ObjectsToEnableOnAnimationStart;
        public List<GameObject> ObjectsToDisableOnAnimationStart;
        [Header("On End")]
        public List<GameObject> ObjectsToDisableOnAnimationEnd;
        public List<GameObject> ObjectsToEnableOnAnimationEnd;
        public List<OnboardingAnimator> ObjectsAnimationsOnEnd;
    }

    public class OnboardingGenericPopUp : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI txtMessage;
        [SerializeField] protected TextMeshProUGUI buttonText;
        [SerializeField] protected Image arrowLeftDown;
        [SerializeField] protected Image arrowRightDown;
        [SerializeField] protected Image arrowDown;
        [SerializeField] protected Image arrowRight;
        [SerializeField] protected Button button;
        [SerializeField] protected List<GameObject> extraObjects;
        [SerializeField] protected List<TextMeshProUGUI> extraTextsToTranslate;
        [SerializeField] protected List<OnboardingAnimation> Animations;

        internal bool animationRunning;
        protected ILanguage language;

        private float myScale;

        private void Awake()
        {
            myScale = this.transform.localScale.x;
            Debug.Log(myScale);
        }


        public void PlayAnimation(int step = 0)
        {
            StartCoroutine(IPlayAnimation(step));
        }

        private IEnumerator IPlayAnimation(int step = 0)
        {
            OnboardingAnimation animation = Animations[step];

            //Check if we need to replace last animation with this one
            if (step - 1 != -1)
            {
                OnboardingAnimation prevAnimation = Animations[step - 1];

                if (prevAnimation.ReplaceWithNextAnim)
                {
                    if (prevAnimation.ObjectsAnimationsOnEnd != null)
                        prevAnimation.ObjectsAnimationsOnEnd.ForEach(x => x.Animator.SetTrigger(x.trigger));
                    if (prevAnimation.ObjectsToDisableOnAnimationEnd != null)
                        prevAnimation.ObjectsToDisableOnAnimationEnd.ForEach(x => x.SetActive(false));
                    if (prevAnimation.ObjectsToEnableOnAnimationEnd != null)
                        prevAnimation.ObjectsToEnableOnAnimationEnd.ForEach(x => x.SetActive(true));
                }
            }

            if (animation.ObjectsToDisableOnAnimationStart != null)
                animation.ObjectsToDisableOnAnimationStart.ForEach(x => x.SetActive(false));
            if (animation.ObjectsToEnableOnAnimationStart != null)
                animation.ObjectsToEnableOnAnimationStart.ForEach(x => x.SetActive(true));
            if (animation.ObjectsAnimationsOnStart != null)
                animation.ObjectsAnimationsOnStart.ForEach(x => x.Animator.SetTrigger(x.trigger));


            if (animation.ReplaceWithNextAnim)
            {
                yield break;
            }
            yield return new WaitForSeconds(animation.AnimationDuration);
            if (animation.ObjectsAnimationsOnEnd != null)
                animation.ObjectsAnimationsOnEnd.ForEach(x => x.Animator.SetTrigger(x.trigger));
            if (animation.ObjectsToDisableOnAnimationEnd != null)
                animation.ObjectsToDisableOnAnimationEnd.ForEach(x => x.SetActive(false));
            if (animation.ObjectsToEnableOnAnimationEnd != null)
                animation.ObjectsToEnableOnAnimationEnd.ForEach(x => x.SetActive(true));
        }

        public void SetAnimationRunning(float time)
        {
            StartCoroutine(IAnimationRunning(time));
        }

        private IEnumerator IAnimationRunning(float time)
        {
            animationRunning = true;
            yield return new WaitForSeconds(time);
            animationRunning = false;
        }

        public GameObject getExtraObject(int num)
        {
            return extraObjects[num];
        }

        public void setActiveExtraObjectIn(int num, float time)
        {
            StartCoroutine(ISetActiveExtraObjectIn(num, time));
        }

        public void setInactiveExtraObjectIn(int num, float time)
        {
            StartCoroutine(ISetInactiveExtraObjectIn(num, time));
        }
        public void setActiveExtraObjectParticlesIn(int num, float time)
        {
            StartCoroutine(ISetActiveExtraObjectParticlesIn(num, time));
        }
        public void setInactiveExtraObjectParticlesIn(int num, float time)
        {
            StartCoroutine(ISetInactiveExtraObjectParticlesIn(num, time));
        }

        public IEnumerator ISetActiveExtraObjectIn(int num, float time)
        {

            yield return new WaitForSeconds(time);
            if (this.gameObject.activeSelf == false)
                yield break;
            extraObjects[num].SetActive(true);
        }

        public IEnumerator ISetInactiveExtraObjectIn(int num, float time)
        {

            yield return new WaitForSeconds(time);
            extraObjects[num].SetActive(false);
        }
        public IEnumerator ISetInactiveExtraObjectParticlesIn(int num, float time)
        {
            yield return new WaitForSeconds(time);
            var emission = extraObjects[num].GetComponent<ParticleSystem>().emission;
            emission.enabled = false;
        }
        public IEnumerator ISetActiveExtraObjectParticlesIn(int num, float time)
        {
            yield return new WaitForSeconds(time);
            var emission = extraObjects[num].GetComponent<ParticleSystem>().emission;
            emission.enabled = true;
        }

        public TextMeshProUGUI getExtraText(int num)
        {
            return extraTextsToTranslate[num];
        }

        public void SetText(string text)
        {
            txtMessage.text = text;
        }

        public void Show(string text)
        {
            this.gameObject.SetActive(true);
            Injection.Get<ILanguage>().SetText(txtMessage, text);
            HideArrows();

            this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            this.gameObject.transform.DOScale(myScale, 0.5f).SetEase(Ease.OutBounce);
        }

        public void ShowExtraObject(int objectNumber)
        {
            extraObjects[objectNumber].gameObject.SetActive(true);

            float myScaleObject = extraObjects[objectNumber].transform.localScale.x;

            extraObjects[objectNumber].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            extraObjects[objectNumber].transform.DOScale(myScaleObject, 0.5f).SetEase(Ease.OutBounce);
        }
        public void ShowObject(GameObject obj)
        {
            obj.SetActive(true);

            float myScaleObject = obj.transform.localScale.x;

            obj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            obj.transform.DOScale(myScaleObject, 0.5f).SetEase(Ease.OutBounce);
        }

        public void Hide()
        {
            this.gameObject.SetActive(false);
        }

        public void HideArrows()
        {
            if (arrowLeftDown != null) arrowLeftDown.gameObject.SetActive(false);
            if (arrowRightDown != null) arrowRightDown.gameObject.SetActive(false);
            if (arrowDown != null) arrowDown.gameObject.SetActive(false);
            if (arrowRight != null) arrowRight.gameObject.SetActive(false);
        }
        public void ShowButton()
        {
            if (arrowLeftDown != null) button.gameObject.SetActive(true);
        }

        public Button GetButton()
        {
            return button;
        }

        public void ShowArrowLeftDown()
        {
            if (arrowLeftDown != null) arrowLeftDown.gameObject.SetActive(true);
        }

        public void HideArrowLeftDown()
        {
            if (arrowLeftDown != null) arrowLeftDown.gameObject.SetActive(false);
        }

        public void ShowArrowRightDown()
        {
            if (arrowRightDown != null) arrowRightDown.gameObject.SetActive(true);
        }

        public void HideArrowRightDown()
        {
            if (arrowRightDown != null) arrowRightDown.gameObject.SetActive(false);
        }

        public void ShowArrowDown()
        {
            if (arrowDown != null) arrowDown.gameObject.SetActive(true);
        }

        public void HideArrowDown()
        {
            if (arrowDown != null) arrowDown.gameObject.SetActive(false);
        }

        public void ShowArrowRight()
        {
            if (arrowRight != null) arrowRight.gameObject.SetActive(true);
        }

        public void HideArrowRight()
        {
            if (arrowRight != null) arrowRight.gameObject.SetActive(false);
        }
    }
}


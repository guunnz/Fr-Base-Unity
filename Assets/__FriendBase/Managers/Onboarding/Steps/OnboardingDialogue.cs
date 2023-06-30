using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Onboarding
{
    public class OnboardingDialogue : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.WELCOME_GIFT;

        public OnboardingDialogue(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            Vector2 canvasPosition = RectTransformUtility.WorldToScreenPoint(references.Camera, references.AvatarController.transform.position) / references.MyCanvas.scaleFactor;

            canvasPosition.x -= references.PopUpWelcomeGift.GetComponent<RectTransform>().sizeDelta.x * 0.8f;
            canvasPosition.y += references.PopUpWelcomeGift.GetComponent<RectTransform>().sizeDelta.y * 2.2f;

            references.PopUpWelcomeGift.GetComponent<RectTransform>().anchoredPosition = canvasPosition;

            references.PopUpWelcomeGift.Show(language.GetTextByKey(LangKeys.ONBOARDING_AND_THIS_IS_YOU));
            references.PopUpWelcomeGift.ShowArrowRightDown();
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            onboardingManager.WaitAndNextStep();
        }

        protected override void OnLimitTimePassed()
        {
            onboardingManager.WaitAndNextStep();
        }

        public override void Destroy()
        {
            references.PopUpWelcomeGift.Hide();
            base.Destroy();
        }
    }
}
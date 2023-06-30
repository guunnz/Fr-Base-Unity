using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Onboarding
{
    public class OnboardingWelcomeGift : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.WELCOME_GIFT;

        private int maxMiniSteps = 1;
        private int miniSteps = 0;

        public OnboardingWelcomeGift(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PopUpWelcomeGift.Show(language.GetTextByKey(LangKeys.ONBOARDING_WELCOME_GIFT));
            references.PopUpWelcomeGift.getExtraObject(1).SetActive(true);
            references.PopUpWelcomeGift.setActiveExtraObjectIn(3, 0f);
            references.PopUpWelcomeGift.setInactiveExtraObjectParticlesIn(3, 2f);
            references.PopUpWelcomeGift.SetAnimationRunning(0.1f);
            references.PopUpWelcomeGift.setActiveExtraObjectIn(4, 3f);
            references.AvatarAnimator.SetTrigger("point 1");
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            if (references.PopUpWelcomeGift.animationRunning)
                return;

            if (miniSteps >= maxMiniSteps)
            {
                
                onboardingManager.WaitAndNextStep();
                return;
            }
            else
            {
                references.NextButton.SetActive(true);
                references.PopUpWelcomeGift.setInactiveExtraObjectParticlesIn(3, 0.01f);
                references.PopUpWelcomeGift.setActiveExtraObjectIn(0, 0.3f);
                references.PopUpWelcomeGift.getExtraObject(5).GetComponent<Animator>().SetTrigger("Hand Point Dissapear");
                references.PopUpWelcomeGift.getExtraObject(2).GetComponent<Animator>().SetTrigger("Tap Gift");
                //runanimation of box opening

                //references.PopUpWelcomeGift.getExtraObject(1).SetActive(false);
                references.PopUpWelcomeGift.SetText("");
                onboardingManager.WaitAndNextStep(1.1f);
            }
            miniSteps++;
        }

        //protected override void OnLimitTimePassed()
        //{
        //    onboardingManager.NextStep();
        //}

        public override void Destroy()
        {
            references.PopUpWelcomeGift.Hide();
            base.Destroy();
        }
    }
}
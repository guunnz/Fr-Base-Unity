using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingWelcomeStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.WELCOME_TO_YOUR_HOUSE;

        public OnboardingWelcomeStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PopUpWelcome.Show(language.GetTextByKey(LangKeys.ONBOARDING_HOME));
            references.PopUpWelcome.ShowButton();
            references.PopUpWelcome.getExtraText(0).text = language.GetTextByKey(LangKeys.ONBOARDING_NEXT);
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            onboardingManager.WaitAndNextStep(0.15f);
        }

        //protected override void OnLimitTimePassed()
        //{
        //    onboardingManager.WaitAndNextStep(0.15f);
        //}

        public override void Destroy()
        {
            references.PopUpWelcome.Hide();
            base.Destroy();
        }
    }
}
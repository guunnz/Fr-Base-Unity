using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingTapHereToGo : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.TAP_HERE_TO_GO;

        public OnboardingTapHereToGo(IOnboarding onboardingManager, OnboardingAssetsReferences references):base(onboardingManager, references)
        {
            references.PopUpTapHereToGo.Show(language.GetTextByKey(LangKeys.ONBOARDING_THIS_IS_YOUR_HOME));
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            onboardingManager.WaitAndNextStep();
        }


        public override void Destroy()
        {
            references.PopUpTapHereToGo.Hide();
            base.Destroy();
        }
    }
}
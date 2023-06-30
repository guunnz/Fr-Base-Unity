using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingShowRoomPopUp : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.GO_TO_PARK;

        public OnboardingShowRoomPopUp(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.OnboardingRoomsList.Open();
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            onboardingManager.WaitAndNextStep();
        }

        public override void Destroy()
        {
            references.OnboardingRoomsList.Close();
            base.Destroy();
        }
    }
}
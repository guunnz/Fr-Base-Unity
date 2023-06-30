using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Onboarding
{
    public class OnboardingShowMyProfile : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.DISCOVER_NEW_PLACES;


        public OnboardingShowMyProfile(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PanelBlackScreen.SetActive(true);
            references.ProfileCardManager.ShowMyProfile();
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            onboardingManager.WaitAndNextStep();
        }

        public override void Destroy()
        {
            references.PanelBlackScreen.SetActive(false);
            references.ProfileCardManager.Hide();
            base.Destroy();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Onboarding
{
    public class OnboardingClosePublicChat : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.CLOSE_PUBLIC_CHAT;

        public OnboardingClosePublicChat(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.OnboardingPublicChatPanel.gameObject.SetActive(true);
            references.OnboardingPublicChatPanel.ShowChat(onboardingManager);
        }



        public override void Destroy()
        {
            references.PopUpClosePublic.Hide();
            base.Destroy();
        }
    }
}
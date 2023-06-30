using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingPrivateChatStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.START_PRIVATE_CHAT;

        public OnboardingPrivateChatStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.OnboardingPrivateChatPanel.gameObject.SetActive(true);
            references.OnboardingPrivateChatPanel.ShowChatPrivate(onboardingManager);
            references.FriendNotificationController.ShowWhitePrivateChat();
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using Data.Users;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingFriendProfileStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.FRIEND_CARD;


        public OnboardingFriendProfileStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PanelBlackScreen.SetActive(true);
            AvatarCustomizationData avatarCustomizationData = references.AvatarFriendController.AvatarCustomizationData;
            references.ProfileCardManager.ShowFriendProfile(avatarCustomizationData);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Onboarding
{
    public class OnboardingFriendbaseMaster : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.FRIENDBASE_MASTER;

        public OnboardingFriendbaseMaster(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.FriendbaseMaster.getExtraText(0).text = language.GetTextByKey(LangKeys.ONBOARDING_GO_PLAY);
            references.FriendbaseMaster.Show(language.GetTextByKey(LangKeys.ONBOARDING_FRIENDBASE_MASTER));
            references.FriendbaseMaster.setActiveExtraObjectParticlesIn(0,0.01f);
            references.AvatarAnimator.SetTrigger("Dance 2");
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            onboardingManager.WaitAndNextStep();
        }


        public override void Destroy()
        {
            references.FriendbaseMaster.Hide();
            base.Destroy();
        }
    }
}
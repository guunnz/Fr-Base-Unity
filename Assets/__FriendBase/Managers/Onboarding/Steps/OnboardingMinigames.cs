using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Onboarding
{
    public class OnboardingMinigames : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.COINS_EARNED_MINIGAMES;

        private Button btnTapAvatar;
        private bool flagTapAvatar;

        public OnboardingMinigames(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            Vector3 scaleAvatar = references.AvatarController.transform.localScale;
            scaleAvatar.x *= -1;
            references.AvatarController.transform.localScale = scaleAvatar;
            references.PopUpMinigames.SetAnimationRunning(2f);//duracion de video de minigames
            references.PopUpMinigames.ShowExtraObject(0);
            references.AvatarAnimator.SetTrigger("point 3");
            references.PopUpMinigames.Show(language.GetTextByKey(LangKeys.ONBOARDING_MINIGAMES));
        }
        protected override void OnTapScreen(Vector3 mousePosition)
        {
            if (references.PopUpMinigames.animationRunning)
                return;
            references.PopUpMinigames.getExtraObject(1).SetActive(false);
            onboardingManager.WaitAndNextStep();
        }

        protected override void OnLimitTimePassed()
        {
            onboardingManager.WaitAndNextStep();
        }

        public override void Destroy()
        {
            references.PopUpMinigames.Hide();
            base.Destroy();
        }
    }
}
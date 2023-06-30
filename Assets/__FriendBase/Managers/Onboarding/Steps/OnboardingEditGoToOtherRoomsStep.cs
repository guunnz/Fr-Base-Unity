using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public class OnboardingEditGoToOtherRoomsStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.DISCOVER_NEW_PLACES;


        public OnboardingEditGoToOtherRoomsStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.NextButton.SetActive(false);
            Vector3 scaleAvatar = references.AvatarController.transform.localScale;
            scaleAvatar.x *= -1;
            references.AvatarController.transform.localScale = scaleAvatar;
            references.PopUpGoToOtherRooms.Show(language.GetTextByKey(LangKeys.ONBOARDING_NOW_TAP_HERE_TO_GO_TO_NEW_PLACES));
            //references.PopUpGoToOtherRooms.ShowArrowLeftDown();
            references.AvatarAnimator.SetTrigger("point 2");
            references.PopUpGoToOtherRooms.ShowObject(references.BtnRooms.gameObject);
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            references.AvatarAnimator.SetTrigger("Idle");
            references.PopUpGoToOtherRooms.getExtraObject(0).SetActive(false); //disable dialogue popup
            references.PopUpGoToOtherRooms.getExtraObject(1).SetActive(false);
            onboardingManager.WaitAndNextStep();
        }

        public override void Destroy()
        {
            references.PopUpGoToOtherRooms.Hide();
            base.Destroy();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Onboarding
{
    public class OnboardingTapHereToGoStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.TAP_HERE_TO_GO;

        private Button btnTapHere;
        private Transform avatarParentTransform;
        private bool tapped = false;

        public OnboardingTapHereToGoStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PopUpTapHereToGo.Show(language.GetTextByKey(LangKeys.ONBOARDING_TAP_TO_WALK));
            btnTapHere = references.PopUpTapHereToGo.transform.Find("BtnTapHere").gameObject.GetComponent<Button>();
            btnTapHere.onClick.AddListener(OnTapHere);
            avatarParentTransform = references.AvatarController.transform.parent.transform;
            references.PopUpTapHereToGo.setActiveExtraObjectIn(0, 0.1f);
            references.PopUpTapHereToGo.setActiveExtraObjectIn(1, 0.1f);
        }

        private void OnTapHere()
        {
            if (tapped)
                return;

            tapped = true;
            Vector3 worldPoint = references.Camera.ScreenToWorldPoint(InputFunctions.mousePosition);

            //Set z position
            Vector3 avatarPosition = avatarParentTransform.position;
            worldPoint.z = avatarPosition.z;

            references.PopUpTapHereToGo.Hide();
            //Move Avatar
            AvatarAnimationController avatarAnimationController = references.AvatarController.GetComponent<AvatarAnimationController>();
            avatarAnimationController.SetWalkState();
            references.PopUpTapHereToGo.getExtraObject(1).GetComponent<Animator>().SetTrigger("DIssapear");
            references.PopUpTapHereToGo.getExtraObject(2).GetComponent<Animator>().SetTrigger("Bye Hand");
            float timeTransition = GetTimeFromWalkDestination(avatarPosition, worldPoint);

            avatarParentTransform.DOMove(worldPoint, timeTransition).SetEase(Ease.Linear).OnComplete(() =>
            {

                avatarAnimationController.SetIdleState();
                onboardingManager.WaitAndNextStep();
            });
        }

        public override void Destroy()
        {
            references.PopUpTapHereToGo.Hide();
            btnTapHere.onClick.RemoveListener(OnTapHere);
            base.Destroy();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Onboarding
{
    public class OnboardingFriendsAppearStep : OnboardingAbstractStep
    {
        public override OnboardingStepType StepType => OnboardingStepType.LETS_INTERACT_WITH_OTHERS;
        private bool canGoToNextStep;
        private Transform friendTransform;

        public OnboardingFriendsAppearStep(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {

            SetParkBackground();
            friendTransform = references.AvatarFriendController.transform.parent.transform;
            canGoToNextStep = false;
            friendTransform.position = references.FriendStartPoint.transform.position;

            references.AvatarFriendController.gameObject.GetComponent<AvatarAnimationController>().SetWalkState();

            //Align finalPosition Friend to my Avatar
            Vector3 finalPosition = references.FriendEndPoint.transform.position;
            finalPosition.y = references.AvatarController.transform.parent.transform.position.y;
            finalPosition.x = references.AvatarController.transform.parent.transform.position.x + 3f;

            float timeTransition = GetTimeFromWalkDestination(references.FriendStartPoint.transform.position, finalPosition);

            //Flip Avatar
            Vector3 scaleAvatar = references.AvatarController.transform.localScale;
            scaleAvatar.x *= -1;
            references.AvatarController.transform.localScale = scaleAvatar;
            references.StartCoroutine(ShowObject());
            friendTransform.DOMove(finalPosition, timeTransition).SetEase(Ease.Linear).OnComplete(() =>
            {
                references.StartCoroutine(PlayHelloAnimations());
            });
        }

        IEnumerator ShowObject()
        {
            references.PopUpTapOnFriend.getExtraText(0).text = language.GetTextByKey(LangKeys.ONBOARDING_LONGTAP);
            yield return new WaitForSeconds(0.75f);
            references.PopUpTapOnFriend.Show(language.GetTextByKey(LangKeys.ONBOARDING_LOOK_A_FRIEND));
        }

        IEnumerator PlayHelloAnimations()
        {
            references.AvatarFriendController.gameObject.GetComponent<AvatarAnimationController>().SetIdleState();

            yield return new WaitForSeconds(0.5f);

            references.AvatarFriendController.GetComponent<AvatarAnimationController>().SetHelloState();

            yield return new WaitForSeconds(1.5f);

            references.AvatarController.GetComponent<AvatarAnimationController>().SetHelloState();

            yield return new WaitForSeconds(1);

            references.PopUpTapOnFriend.ShowExtraObject(0);
            references.PopUpTapOnFriend.ShowExtraObject(1);
            yield return new WaitForSeconds(2.5f);

            
            canGoToNextStep = true;
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            if (canGoToNextStep)
            {
                references.PopUpTapOnFriend.getExtraObject(1).gameObject.SetActive(false);
                onboardingManager.WaitAndNextStep();
            }
        }

        public override void Destroy()
        {
            references.PopUpTapOnFriend.Hide();
            base.Destroy();
        }
    }
}
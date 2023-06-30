using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Onboarding
{
    public class OnboardingReopenPrivateChat : OnboardingAbstractStep
    {
        private int maxMiniSteps = 1;
        private int miniSteps = 0;
        private Transform friendTransform1;
        private Transform friendTransform2;
        private Transform friendTransform3;
        public override OnboardingStepType StepType => OnboardingStepType.REOPEN_PRIVATE_CHAT;

        public OnboardingReopenPrivateChat(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
          
            references.PopUpReopenPrivate.Show(language.GetTextByKey(LangKeys.ONBOARDING_REOPEN_BUBBLE));
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            if (references.PopUpReopenPrivate.animationRunning)
                return;

            if (miniSteps >= maxMiniSteps)
            {
                onboardingManager.WaitAndNextStep();
                return;
            }
            else
            {
                references.PopUpReopenPrivate.getExtraText(0).text = language.GetTextByKey(LangKeys.ONBOARDING_PUBLIC_chat);
                references.PopUpReopenPrivate.ShowExtraObject(0);
                references.PopUpReopenPrivate.ShowExtraObject(1);
                MoveAvatars();
            }
            miniSteps++;
        }

        public void MoveAvatars()
        {
            Transform cameraTransform = references.Camera.transform;
            cameraTransform.DOMove(new Vector3(-2.5f, cameraTransform.position.y, cameraTransform.position.z), 1f);
            references.AvatarController.transform.localScale = new Vector3(-0.123f, 0.123f, -0.123f);
            SetParkBackground();
            friendTransform1 = references.AvatarFriendController1.transform.parent.transform;
            friendTransform2 = references.AvatarFriendController2.transform.parent.transform;
            friendTransform3 = references.AvatarFriendController3.transform.parent.transform;

            friendTransform1.position = references.FriendStartPoint1.transform.position;
            friendTransform2.position = references.FriendStartPoint2.transform.position;
            friendTransform3.position = references.FriendStartPoint3.transform.position;

            references.AvatarFriendController1.gameObject.GetComponent<AvatarAnimationController>().SetWalkState();
            references.AvatarFriendController2.gameObject.GetComponent<AvatarAnimationController>().SetWalkState();
            references.AvatarFriendController3.gameObject.GetComponent<AvatarAnimationController>().SetWalkState();

            //Align finalPosition Friend to my Avatar
            Vector3 finalPosition1 = references.FriendEndPoint1.transform.position;

            Vector3 finalPosition2 = references.FriendEndPoint2.transform.position;

            Vector3 finalPosition3 = references.FriendEndPoint3.transform.position;

            float timeTransition1 = GetTimeFromWalkDestination(references.FriendStartPoint1.transform.position, finalPosition1);
            float timeTransition2 = GetTimeFromWalkDestination(references.FriendStartPoint2.transform.position, finalPosition2);
            float timeTransition3 = GetTimeFromWalkDestination(references.FriendStartPoint3.transform.position, finalPosition3);

            references.PopUpReopenPrivate.SetAnimationRunning(timeTransition2);

            friendTransform1.DOMove(finalPosition1, timeTransition1).SetEase(Ease.Linear).OnComplete(() =>
            {
                references.AvatarFriendController1.gameObject.GetComponent<AvatarAnimationController>().SetIdleState();
            });

            friendTransform2.DOMove(finalPosition2, timeTransition2).SetEase(Ease.Linear).OnComplete(() =>
            {
                references.AvatarFriendController2.gameObject.GetComponent<AvatarAnimationController>().SetIdleState();
            });

            friendTransform3.DOMove(finalPosition3, timeTransition3).SetEase(Ease.Linear).OnComplete(() =>
            {

                references.AvatarFriendController3.gameObject.GetComponent<AvatarAnimationController>().SetIdleState();
            });
        }


        public override void Destroy()
        {
            references.PopUpReopenPrivate.Hide();
            base.Destroy();
        }
    }
}
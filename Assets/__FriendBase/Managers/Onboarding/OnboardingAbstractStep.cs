using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using LocalizationSystem;
using UnityEngine;

namespace Onboarding
{
    public abstract class OnboardingAbstractStep
    {
        public abstract OnboardingStepType StepType { get; }
        protected IOnboarding onboardingManager;
        protected OnboardingAssetsReferences references;
        protected Coroutine limitTimeCoroutine;

        private bool flagTap;
        protected ILanguage language;

        public OnboardingAbstractStep(IOnboarding onboardingManager, OnboardingAssetsReferences references)
        {
            this.onboardingManager = onboardingManager;
            this.references = references;
            limitTimeCoroutine = references.StartCoroutine(WaitLimitTime());

            flagTap = false;
            language = Injection.Get<ILanguage>();
        }

        public void SetParkBackground()
        {
            references.ParkBackground.SetActive(true);
            references.RioBackground.SetActive(false);
        }

        public void SetRioBackground()
        {
            references.RioBackground.SetActive(true);
            references.ParkBackground.SetActive(false);
        }

        public virtual void Update()
        {
            if (InputFunctions.GetMouseButtonDown(0))
            {
                flagTap = true;
            }

            if (InputFunctions.GetMouseButtonUp(0) && flagTap)
            {
                OnTapScreen(InputFunctions.mousePosition);
                flagTap = false;
            }
        }

        protected virtual void OnTapScreen(Vector3 mousePosition)
        {

        }

        protected float GetTimeFromWalkDestination(Vector3 startPoint, Vector3 endPoint)
        {
            Vector3 delta = endPoint - startPoint;
            float distance = delta.magnitude;
            return distance / 2f;
        }

        public virtual void Destroy()
        {
            if (limitTimeCoroutine != null)
            {
                references.StopCoroutine(limitTimeCoroutine);
            }
        }

        IEnumerator WaitLimitTime()
        {
            yield return new WaitForSeconds(14);
            limitTimeCoroutine = null;
            OnLimitTimePassed();
        }

        protected virtual void OnLimitTimePassed()
        {

        }

        public static OnboardingAbstractStep GetStepInstance(OnboardingStepType stepType, IOnboarding onboardingManager, OnboardingAssetsReferences references)
        {

            switch (stepType)
            {
                case OnboardingStepType.WELCOME_TO_YOUR_HOUSE:
                    return new OnboardingWelcomeStep(onboardingManager, references);
                case OnboardingStepType.TAP_HERE_TO_GO:
                    return new OnboardingTapHereToGoStep(onboardingManager, references);
                case OnboardingStepType.WELCOME_GIFT:
                    return new OnboardingWelcomeGift(onboardingManager, references);
                case OnboardingStepType.BUY_FURNITURE_ROOMS_AND_PETS_AND_LOOKS:
                    return new OnboardingBuyFurnitureAndPetsAndLooks(onboardingManager, references);
                case OnboardingStepType.COINS_EARNED_MINIGAMES:
                    return new OnboardingMinigames(onboardingManager, references);
                case OnboardingStepType.DISCOVER_NEW_PLACES:
                    return new OnboardingEditGoToOtherRoomsStep(onboardingManager, references);
                case OnboardingStepType.GO_TO_PARK:
                    return new OnboardingShowRoomPopUp(onboardingManager, references);
                case OnboardingStepType.LETS_INTERACT_WITH_OTHERS:
                    return new OnboardingFriendsAppearStep(onboardingManager, references);
                case OnboardingStepType.FRIEND_CARD:
                    return new OnboardingFriendProfileStep(onboardingManager, references);
                case OnboardingStepType.START_PRIVATE_CHAT:
                    return new OnboardingPrivateChatStep(onboardingManager, references);
                case OnboardingStepType.REOPEN_PRIVATE_CHAT:
                    return new OnboardingReopenPrivateChat(onboardingManager, references);
                case OnboardingStepType.CLOSE_PUBLIC_CHAT:
                    return new OnboardingClosePublicChat(onboardingManager, references);
                case OnboardingStepType.FRIENDBASE_MASTER:
                    return new OnboardingFriendbaseMaster(onboardingManager, references);
            }
            return null;
        }
    }
}
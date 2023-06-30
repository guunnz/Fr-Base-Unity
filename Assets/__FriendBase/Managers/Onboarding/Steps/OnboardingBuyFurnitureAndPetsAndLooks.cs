using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Onboarding
{
    public class OnboardingBuyFurnitureAndPetsAndLooks : OnboardingAbstractStep
    {
        private int maxMiniSteps = 2;
        private int miniSteps = 0;
        public override OnboardingStepType StepType => OnboardingStepType.BUY_FURNITURE_ROOMS_AND_PETS_AND_LOOKS;

        public OnboardingBuyFurnitureAndPetsAndLooks(IOnboarding onboardingManager, OnboardingAssetsReferences references) : base(onboardingManager, references)
        {
            references.PopUpBuyFurnituresAndRoomsOrPets.getExtraText(0).text = language.GetTextByKey(LangKeys.ONBOARDING_BUY_GEMS_AND_ROOMS);
            references.PopUpBuyFurnituresAndRoomsOrPets.getExtraText(1).text = language.GetTextByKey(LangKeys.ONBOARDING_find_places_meet_other_people);
            references.PopUpBuyFurnituresAndRoomsOrPets.getExtraText(2).text = language.GetTextByKey(LangKeys.ONBOARDING_SKINS_AND_PETS);
            references.PopUpBuyFurnituresAndRoomsOrPets.Show(language.GetTextByKey(LangKeys.ONBOARDING_WITH_GEMS_YOU_CAN_BY));
        }

        protected override void OnTapScreen(Vector3 mousePosition)
        {
            if (references.PopUpWelcomeGift.animationRunning)
                return;

            if (miniSteps >= maxMiniSteps)
            {
                references.PopUpBuyFurnituresAndRoomsOrPets.getExtraObject(2).SetActive(false);
                references.PopUpBuyFurnituresAndRoomsOrPets.getExtraObject(3).SetActive(false);
                references.PopUpBuyFurnituresAndRoomsOrPets.getExtraObject(4).SetActive(false);
                references.PopUpBuyFurnituresAndRoomsOrPets.getExtraObject(5).SetActive(false);
                onboardingManager.WaitAndNextStep();
                return;
            }
            else
            {
                if (miniSteps != 0)
                {
                    references.AvatarAnimator.SetTrigger("point 5");
                    references.PopUpBuyFurnituresAndRoomsOrPets.getExtraObject(miniSteps + 3).SetActive(false);
                }
                else
                {
                    references.AvatarAnimator.SetTrigger("point 2");
                }
                references.PopUpBuyFurnituresAndRoomsOrPets.ShowExtraObject(miniSteps);
            }
            miniSteps++;
        }

        //protected override void OnLimitTimePassed()
        //{
        //    references.PopUpBuyFurnituresAndRoomsOrPets.getExtraObject(2).SetActive(false);
        //    references.PopUpBuyFurnituresAndRoomsOrPets.getExtraObject(3).SetActive(false);
        //    onboardingManager.NextStep();
        //}

        public override void Destroy()
        {
            references.PopUpBuyFurnituresAndRoomsOrPets.Hide();
            base.Destroy();
        }
    }
}
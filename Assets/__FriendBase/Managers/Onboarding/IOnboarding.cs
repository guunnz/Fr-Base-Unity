using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Onboarding
{
    public interface IOnboarding
    {
        public void NextStep();
        public void WaitAndNextStep(float time = 0);
        public void SetCameraPositionOpenChat();
        public void SetCameraPositionOpenChat2();
        public void SetCameraPosition();
        public void ShowGuestStepAfterLinkingProvider();
    }
}
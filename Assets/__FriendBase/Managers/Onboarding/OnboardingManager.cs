
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onboarding;
using System;
using Data.Users;
using Architecture.Injector.Core;
using Data;
using Newtonsoft.Json.Linq;
using UnityEngine.SceneManagement;
using Architecture.ViewManager;
using Socket;
using UniRx;
using LocalizationSystem;
using MainMenu;

namespace Onboarding
{
    public enum OnboardingStepType
    {
        TAP_GO_HOME = -1,
        WELCOME_TO_YOUR_HOUSE = 0,
        TAP_HERE_TO_GO = 1,
        WELCOME_GIFT = 2,
        BUY_FURNITURE_ROOMS_AND_PETS_AND_LOOKS = 3,
        COINS_EARNED_MINIGAMES = 4,
        DISCOVER_NEW_PLACES = 5,
        GO_TO_PARK = 6,
        LETS_INTERACT_WITH_OTHERS = 7,
        FRIEND_CARD = 8,
        START_PRIVATE_CHAT = 9, //Need to do
        REOPEN_PRIVATE_CHAT = 10,
        CLOSE_PUBLIC_CHAT = 11,
        FRIENDBASE_MASTER = 12,
    };

    public class OnboardingManager : MonoBehaviour, IOnboarding
    {
        IAnalyticsSender analyticsSender;
        [SerializeField] private OnboardingAssetsReferences references;

        private OnboardingAbstractStep currentStep;

        private IViewManager viewManager;
        private ISocketManager socketManager;
        private ILanguage language;
        private IGameData gameData;
        private bool wasGuestModalUp;
        void Start()
        {
            language = Injection.Get<ILanguage>();
            Injection.Get<ILoading>().Unload();
            gameData = Injection.Get<IGameData>();
            analyticsSender = Injection.Get<IAnalyticsSender>();
            //FixConnectionIssues();
            if (gameData.GetWasGuest())
            {

                wasGuestModalUp = true;
                references.YouWereGuest.SetActive(true);

                references.AvatarController.transform.parent.transform.position = references.moveToGuest.position;
                references.ParkBackground.SetActive(true);
                references.RioBackground.SetActive(false);
            }

            language.SetTextByKey(references.txtSkipOnboarding, LangKeys.ONBOARDING_SKIP_TUTORIAL);
            language.SetTextByKey(references.txtNext, LangKeys.ONBOARDING_NEXT);

            //Set avatar skin
            AvatarCustomizationData avatarCustomizationData = new AvatarCustomizationData();
            avatarCustomizationData = Injection.Get<IGameData>().GetUserInformation().GetAvatarCustomizationData();
            references.AvatarController.SetAvatarCustomizationData(avatarCustomizationData.GetSerializeData());

            //Set Friend Skin
            TextAsset avatarSkinCarlos = Resources.Load("Avatar/SkinCarlos") as TextAsset;
            JObject jsonAvatar = JObject.Parse(avatarSkinCarlos.ToString());
            references.AvatarFriendController.SetAvatarCustomizationData(jsonAvatar);
            PlayerPrefs.SetInt("TutorialText", 1);
            SetCameraPosition();

            if (gameData.GetWasGuest())
            {
                return;
            }

            StartCoroutine(ShowFirstStep());
        }

        void FixConnectionIssues()
        {
            SimpleSocketManager.Instance.Connect();

            Injection.Get<IAvatarEndpoints>().GetUserInformation().Subscribe(userInformation =>
            {
                Injection.Get<IGameData>().GetUserInformation().Initialize(userInformation);
            });
        }

        public void SetCameraPositionOpenChat2()
        {
            //Vector3 cameraPosition = references.Camera.transform.position;
            //cameraPosition.x = references.AvatarFriendController.transform.position.x + 0.5f;
            //references.Camera.transform.position = cameraPosition;

            float aspectRatio = (float)Screen.width / (float)Screen.height;
            Vector3 cameraPosition = new Vector3(0f, 0, -10);

            //float widthBackground = references.RioBackground.GetComponent<SpriteRenderer>().size.x;
            float cameraSizeHeight = references.Camera.orthographicSize * 2;
            //cameraPosition.x = (widthBackground * 0.70f) - (aspectRatio * cameraSizeHeight / 2);
            references.Camera.orthographicSize = 4.05f;
            references.Camera.transform.position = cameraPosition;
        }

        public void SetCameraPositionOpenChat()
        {
            Vector3 cameraPosition = references.Camera.transform.position;
            cameraPosition.x = references.AvatarFriendController.transform.position.x + 0.5f;
            references.Camera.transform.position = cameraPosition;

            float aspectRatio = (float)Screen.width / (float)Screen.height;
            cameraPosition = new Vector3(2.5f, 0, -10);

            float widthBackground = references.RioBackground.GetComponent<SpriteRenderer>().size.x;
            float cameraSizeHeight = references.Camera.orthographicSize * 2;
            cameraPosition.x = (widthBackground * 0.70f) - (aspectRatio * cameraSizeHeight / 2);
            //references.Camera.orthographicSize = 4.05f;
            references.Camera.transform.position = cameraPosition;
        }

        public void SetCameraPosition()
        {
            float aspectRatio = (float)Screen.width / (float)Screen.height;
            Vector3 cameraPosition = new Vector3(-1.9f, 0, -10);

            float widthBackground = references.RioBackground.GetComponent<SpriteRenderer>().size.x;
            float cameraSizeHeight = references.Camera.orthographicSize * 2;
            cameraPosition.x = (widthBackground / 2) - (aspectRatio * cameraSizeHeight / 2);
            references.Camera.transform.position = cameraPosition;
        }

        IEnumerator ShowFirstStep()
        {
            yield return new WaitForSeconds(0.2f);
            currentStep = OnboardingAbstractStep.GetStepInstance(OnboardingStepType.WELCOME_TO_YOUR_HOUSE, this, references);
            //currentStep = OnboardingAbstractStep.GetStepInstance(OnboardingStepType.TAP_GO_PUBLIC_ROOM, this, references);
        }

        public void ShowGuestStepAfterLinkingProvider()
        {
            currentStep = OnboardingAbstractStep.GetStepInstance(OnboardingStepType.LETS_INTERACT_WITH_OTHERS, this, references);
            //currentStep = OnboardingAbstractStep.GetStepInstance(OnboardingStepType.TAP_GO_PUBLIC_ROOM, this, references);
        }

        void Update()
        {
            if (currentStep != null)
            {
                currentStep.Update();
            }
        }

        public void WaitAndNextStep(float time = 0f)
        {
            StartCoroutine(WaitAndNextStepCoroutine(time));
        }

        IEnumerator WaitAndNextStepCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            NextStep();
        }



        public void NextStep()
        {
            if (references.YouWereGuest.activeSelf)
                return;

            int stepIndex = (int)currentStep.StepType;
            stepIndex++;

            OnboardingStepType nextStepType = (OnboardingStepType)stepIndex;
            if (nextStepType == OnboardingStepType.GO_TO_PARK && gameData.IsGuest())
            {
                EndOnboarding();
            }
            else if (Enum.IsDefined(typeof(OnboardingStepType), nextStepType))
            {
                currentStep.Destroy();
                currentStep = OnboardingAbstractStep.GetStepInstance(nextStepType, this, references);
            }
            else
            {
                EndOnboarding();
            }
        }

        public void EndOnboarding()
        {
            StartCoroutine(EndOnboardingCoroutine());
        }

        IEnumerator EndOnboardingCoroutine()
        {
            if (currentStep != null)
            {
                currentStep.Destroy();
                currentStep = null;
            }

            gameData.SetWasGuest(false);
            Injection.Get<ILoading>().Load();
            //int idRoom = EnvironmentData.GetPublicIdRoomToJumpAfterLogin();

            //Injection.Get<IRoomListEndpoints>().GetFreePublicRoomByType(idRoom).Subscribe(roomInformation =>
            //{
            //    Injection.Get<IGameData>().SetRoomInformation(roomInformation);
            //    SceneManager.LoadScene(GameScenes.GoToRoomScene, LoadSceneMode.Additive);
            //});
            Injection.Get<IViewManager>().Show<MainMenuView>();
            analyticsSender.SendAnalytics(AnalyticsEvent.OnboardingFinish);
            references.BtnSkipOnboarding.gameObject.SetActive(false);
            //references.ScreenTransition.SetActive(true);




            yield return new WaitForEndOfFrame();

            SceneManager.UnloadSceneAsync(GameScenes.Onboarding);
        }
    }
}
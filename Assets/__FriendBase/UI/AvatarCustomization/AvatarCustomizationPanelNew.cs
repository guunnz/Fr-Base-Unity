using System.Collections;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using Audio.Music;
using Data;
using PlayerMovement;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace AvatarCustomization
{
    public class AvatarCustomizationPanelNew : MonoBehaviour
    {
        [SerializeField] UICatalogAvatarManager catalogAvatarManager;
        IAnalyticsSender analyticsSender;
        IGameData gameData;
        IMusicPlayer musicPlayer;

        void Start()
        {
            analyticsSender = Injection.Get<IAnalyticsSender>();
            Injection.Get(out musicPlayer);
            gameData = Injection.Get<IGameData>();

            Injection.Get<ILoading>().Unload();
            Open();
        }

        void Open()
        {
            //musicPlayer.Play("cafe-music");
            bool flagComeFromRegistration = gameData.GetUserInformation().Do_avatar_customization;

            if (flagComeFromRegistration)
            {
                gameData.GetUserInformation().Do_avatar_customization = false;
                catalogAvatarManager.Open(UICatalogAvatarManager.AVATAR_PANEL_TYPE.CREATE_AVATAR);
            }
            else
            {
                analyticsSender.SendAnalytics(AnalyticsEvent.EnterAvatarCustomization);
                catalogAvatarManager.Open(UICatalogAvatarManager.AVATAR_PANEL_TYPE.CHANGE_AVATAR);
            }

            catalogAvatarManager.OnCreateAvatarComplete += OnCreateAvatarComplete;
            catalogAvatarManager.OnChangeAvatarComplete += OnChangeAvatarComplete;
        }

        void OnCreateAvatarComplete()
        {
            Injection.Get<ILoading>().Load();
            StartCoroutine(OnCreateAvatarCompleteCoroutine());
        }

        IEnumerator OnCreateAvatarCompleteCoroutine()
        {
            SceneManager.LoadScene(GameScenes.Onboarding, LoadSceneMode.Additive);
            yield return new WaitForEndOfFrame();
            SceneManager.UnloadSceneAsync(GameScenes.AvatarCustomization);
        }

        void OnChangeAvatarComplete()
        {
            Injection.Get<ILoading>().Load();
            StartCoroutine(OnChangeAvatarCompleteCoroutine());
        }

        IEnumerator OnChangeAvatarCompleteCoroutine()
        {
            SceneManager.LoadScene(GameScenes.RoomScene, LoadSceneMode.Additive);
            yield return new WaitForEndOfFrame();
            SceneManager.UnloadSceneAsync(GameScenes.AvatarCustomization);
        }

        void OnDestroy()
        {
            catalogAvatarManager.OnCreateAvatarComplete -= OnCreateAvatarComplete;
            catalogAvatarManager.OnChangeAvatarComplete -= OnChangeAvatarComplete;
            catalogAvatarManager.Close();
        }
    }
}
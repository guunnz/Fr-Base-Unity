using System.Collections;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using Audio.Music;
using Data;
using PlayerMovement;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AvatarCustomization
{
    public class AvatarCustomizationPanel : ViewNode
    {
        [SerializeField] UICatalogAvatarManager catalogAvatarManager;

        IViewManager viewManager;
        IGameData gameData;
        IMusicPlayer musicPlayer;
        protected override void OnInit()
        {
            //Injection.Get(out musicPlayer);
            //gameData = Injection.Get<IGameData>();
            //viewManager = Injection.Get<IViewManager>();
            catalogAvatarManager.Close();
            StartCoroutine(Redirect());
        }

        IEnumerator Redirect()
        {
            yield return new WaitForEndOfFrame();
            SceneManager.LoadScene(GameScenes.AvatarCustomization, LoadSceneMode.Additive);
            yield return new WaitForEndOfFrame();
            HideView();
        }

        void OnCreateAvatarComplete()
        {
            SceneManager.LoadScene(GameScenes.Onboarding, LoadSceneMode.Additive);
            HideView();
        }

        void OnChangeAvatarComplete()
        {
            SceneManager.LoadScene(GameScenes.RoomScene, LoadSceneMode.Additive);
            HideView();
            //viewManager.GetOut("back");
        }

        protected override void OnShow()
        {
            //RemotePlayersPool.Current?.ClearRemotesPool();
            //musicPlayer.Play("cafe-music");
            //bool flagComeFromRegistration = gameData.GetUserInformation().Do_avatar_customization;

            //if (flagComeFromRegistration)
            //{
            //    gameData.GetUserInformation().Do_avatar_customization = false;
            //    catalogAvatarManager.Open(UICatalogAvatarManager.AVATAR_PANEL_TYPE.CREATE_AVATAR);
            //}
            //else
            //{
            //    catalogAvatarManager.Open(UICatalogAvatarManager.AVATAR_PANEL_TYPE.CHANGE_AVATAR);
            //}

            //catalogAvatarManager.OnCreateAvatarComplete += OnCreateAvatarComplete;
            //catalogAvatarManager.OnChangeAvatarComplete += OnChangeAvatarComplete;
        }

        protected override void OnHide()
        {
            //HideView();
            //RemotePlayersPool.Current?.ClearRemotesPool();
            //catalogAvatarManager.OnCreateAvatarComplete -= OnCreateAvatarComplete;
            //catalogAvatarManager.OnChangeAvatarComplete -= OnChangeAvatarComplete;
            //catalogAvatarManager.Close();
        }
    }
}
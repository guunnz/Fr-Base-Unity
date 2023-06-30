using System;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Architecture.ViewManager;
using Audio.Music;
using AuthFlow.EndAuth.Repo;
using AuthFlow.AboutYou.Core.Services;
using DeepLink.Delivery;
using Firebase;
using Firebase.Auth;
using JetBrains.Annotations;
using LoadingScreen;
using MemoryStorage.Core;
using Shared.Utils;
using Socket;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Data;
using Newtonsoft.Json;
using Data.Catalog;
using System.Linq;
using Data.Users;
using AuthFlow;
using UnityEngine.SceneManagement;
using Data.Rooms;
using LocalizationSystem;

#if UNITY_EDITOR
using UnityEditor;

#endif

namespace MainMenu
{
    [Serializable]
    public class MenuButtonInfo
    {
        public string name;
        public bool isActive = true;

        [Tooltip("this will be called before moving towards outPort (if there is)")]
        public UnityEvent onClick;
    }

    public class MainMenuView : ViewNode
    {
        public List<MenuButtonInfo> buttons;
        public RectTransform container;

        IViewManager viewManager;
        Button menuButtonPrefab;
        ISocketManager socketManager;
        IMemoryStorage memoryStorage;
        IMusicPlayer musicPlayer;
        ILanguage language;
        private IGameData gameData;

        private static bool flagAvatarEndpointsReady = false;
        public TMP_Text PlayText;
        public TMP_Text QuickPlayText;
        public TMP_Text SettingsText;
        public TMP_Text LogoutText;
        public TMP_Text CustomizeCharacterText;
        public TMP_Text QuitText;

        private void SetLanguage()
        {
            //language.SetTextByKey(PlayText, LangKeys.QUICKPLAY_PLAY);
            language.SetTextByKey(QuickPlayText, LangKeys.QUICKPLAY_PLAY);
            language.SetTextByKey(CustomizeCharacterText, LangKeys.QUICKPLAY_CUSTOMIZE_CHARACTER);
            language.SetTextByKey(LogoutText, LangKeys.QUICKPLAY_LOGOUT);
            //language.SetTextByKey(SettingsText, LangKeys.QUICKPLAY_SETTINGS);
            language.SetTextByKey(QuitText, LangKeys.QUICKPLAY_QUIT);
        }

        protected override void OnInit()
        {
            Injection.Get(out musicPlayer);
            Injection.Get(out viewManager);
            Injection.Get(out socketManager);
            Injection.Get(out memoryStorage);

            gameData = Injection.Get<IGameData>();
            language = Injection.Get<ILanguage>();
            //SetLanguage();
        }

        public void GoToOnboarding()
        {
            SceneManager.LoadScene(GameScenes.Onboarding, LoadSceneMode.Additive);
            HideView();
        }

        public void GoToMyHouse()
        {
            gameData.SetRoomInformation(gameData.GetMyHouseInformation());
            SceneManager.LoadScene(GameScenes.RoomScene, LoadSceneMode.Additive);
            HideView();
        }

        public void GoToPublicRoom(RoomInformation roomInformation)
        {
            gameData.SetRoomInformation(roomInformation);
            SceneManager.LoadScene(GameScenes.RoomScene, LoadSceneMode.Additive);
            HideView();
        }

        public void GoToAvatarCustomization()
        {
            SceneManager.LoadScene(GameScenes.AvatarCustomization, LoadSceneMode.Additive);
            HideView();
        }

        protected override void OnShow()
        {
            //musicPlayer.Play("park-music");
            CleanUp();

            ShowMenuItems();
            SimpleSocketManager.Instance.OnUserBanned += OnUserBanned;
            SimpleSocketManager.Instance.Connect();

            SimpleSocketManager.Instance.CheckBanStatus(gameData.GetUserInformation().UserStatus);
        }

        protected override void OnHide()
        {
            SimpleSocketManager.Instance.OnUserBanned -= OnUserBanned;
        }

        void OnUserBanned()
        {
            HideView();
        }

        void CleanUp()
        {
            memoryStorage.Set("currentRoomName", string.Empty);
            memoryStorage.Set("currentRoomId", string.Empty);
        }

        void ShowMenuItems()
        {
            //GoToMyHouse();
            //GoToOnboarding();
            //return;

            if (gameData.GetWasGuest())
            {
                GoToOnboarding();
                return;
            }

            int idRoom = EnvironmentData.GetPublicIdRoomToJumpAfterLogin();

            if (gameData.GetUserInformation().UserStatus.IsSuspended())
            {
                GoToMyHouse();
                return;
            }

            Injection.Get<IRoomListEndpoints>().GetFreePublicRoomByType(idRoom).Subscribe(roomInformation =>
            {
                GoToPublicRoom(roomInformation);
            });
            return;

            container.gameObject.DestroyChildren();
            menuButtonPrefab ??= Resources.Load<Button>("MainMenuButton");

            foreach (var button in buttons)
            {
                if (!(button is { isActive: true })) continue;
                var newButton = Instantiate(menuButtonPrefab, container);
                newButton.name = "button " + button.name;
                TMP_Text tmp_text = newButton.GetComponentInChildren<TMP_Text>();
                tmp_text.text = button.name;
                switch (button.name.ToLower())
                {
                    case "quick play":
                        QuickPlayText = tmp_text;
                        break;
                    case "play":
                        PlayText = tmp_text;
                        break;
                    case "settings":
                        SettingsText = tmp_text;
                        break;
                    case "log out":
                        LogoutText = tmp_text;
                        break;
                    case "customize your character":
                        CustomizeCharacterText = tmp_text;
                        break;
                    case "quit":
                        QuitText = tmp_text;
                        break;
                    default:
                        break;
                }

                newButton.OnClickAsObservable().Subscribe(() => button.onClick?.Invoke());
            }

            SetLanguage();
        }

        [UsedImplicitly]
        public void Event_GetOut(string outPort)
        {
            viewManager.GetOut(outPort);
        }

        [UsedImplicitly]
        public void Event_QuitGame()
        {
            JesseUtils.QuitGame();
        }

        [UsedImplicitly]
        public void Event_LogOut()
        {
            JesseUtils.Logout();
        }
    }
}
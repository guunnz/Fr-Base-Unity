using System;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Architecture.MVP;
using Architecture.ViewManager;
using PlayerRoom.Core.Domain;
using PlayerRoom.Presentation;
using Shared.Utils;
using Tools;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LocalizationSystem;

namespace PlayerRoom.View
{
    public class RoomListPopup : WidgetBase, IRoomListView
    {
        readonly ISubject<string> onClickRoomButton = new Subject<string>();
        public RoomListItem roomListItem;
        public RectTransform container;
        readonly CompositeDisposable disposables = new CompositeDisposable();


        public UIWidget<string> roomName;

        public GameObject menuPage;
        public GameObject roomsPage;
        public GameObject placesTab;
        public GameObject eventsTab;

        public Button placesButton;
        public Button eventsButton;
        public Button close;
        public Button back;

        public TextMeshProUGUI Events;
        public TextMeshProUGUI Places;

        public ILanguage language;

        void Awake()
        {
            this.CreatePresenter<RoomListPresenter, IRoomListView>();
            ResetTabs();
        }

        public IObservable<Unit> OnCloseButton => close.OnClickAsObservable();
        public IObservable<Unit> OnBackButton => back.OnClickAsObservable();

        public void SetLanguage()
        {
            if (language == null)
                language = Injection.Get<ILanguage>();
            language.SetTextByKey(Places, LangKeys.NAV_PLACES);
            language.SetTextByKey(Events, LangKeys.NAV_EVENTS);
        }


        void Start()
        {
            SetLanguage();
        }

        public void ResetTabs()
        {
            menuPage.SetActive(true);
            roomsPage.SetActive(false);
            disposables.Clear();
            container.gameObject.DestroyChildren(roomListItem.gameObject);
        }

        public void ShowAreas(IEnumerable<RoomInfo> rooms)
        {
            disposables.Clear();
            container.gameObject.DestroyChildren(roomListItem.gameObject);

            placesButton.OnClickAsObservable().Subscribe(ShowPlaces).AddTo(disposables);
            eventsButton.OnClickAsObservable().Subscribe(ShowEvents).AddTo(disposables);

            foreach (var roomInfo in rooms)
            {
                var newListItem = Instantiate(roomListItem, container);
                newListItem.ShowInfo(roomInfo, false);
                var info = roomInfo;
                newListItem
                    .OnClick
                    .Select(() => info.AreaId)
                    .Do(onClickRoomButton.OnNext)
                    .Subscribe()
                    .AddTo(disposables);
            }


            roomsPage.SetActive(false);
            menuPage.SetActive(true);
        }

        private void ShowEvents()
        {
            placesTab.SetActive(false);
            eventsTab.SetActive(true);
            container.gameObject.SetActive(false);
        }

        private void ShowPlaces()
        {
            placesTab.SetActive(true);
            eventsTab.SetActive(false);
            container.gameObject.SetActive(true);
        }

        public void ShowInstances(IEnumerable<RoomInfo> rooms)
        {
            disposables.Clear();
            container.gameObject.DestroyChildren(roomListItem.gameObject);

            foreach (var roomInfo in rooms)
            {
                var newListItem = Instantiate(roomListItem, container);
                newListItem.ShowInfo(roomInfo, true);
                var info = roomInfo;
                newListItem
                    .OnClick
                    .Select(() => info.InstanceId)
                    .Do(onClickRoomButton.OnNext)
                    .Subscribe()
                    .AddTo(disposables);


                roomName.Value = roomInfo.RoomName;
            }


            roomsPage.SetActive(true);
            menuPage.SetActive(false);
        }

        public IObservable<string> OnClickRoom => onClickRoomButton;

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
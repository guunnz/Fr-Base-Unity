using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using Data;
using Data.Rooms;
using FriendsView.Core.Domain;
using FriendsView.Core.Services;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;
using Notification = Managers.Avatar.Notification;

namespace FriendsView.Presentation
{
    [UsedImplicitly]
    public class FriendsPresenter
    {
        readonly IFriendsView view;
        readonly IFriendsWebClient webClient;
        readonly IGameData gameData;

        readonly IFriendsNotificationsServices friendsNotificationsServices =
            Injection.Get<IFriendsNotificationsServices>();

        readonly CompositeDisposable sectionDisposables = new CompositeDisposable();
        readonly CompositeDisposable requestButtonsDisposables = new CompositeDisposable();
        readonly CompositeDisposable reportButtonsDisposables = new CompositeDisposable();
        readonly CompositeDisposable reportOtherDisposables = new CompositeDisposable();
        readonly CompositeDisposable disposables = new CompositeDisposable();

        private ViewSection CurrentSection { get; set; }


        public FriendsPresenter(IFriendsView view, IFriendsWebClient webClient, IGameData gameData)
        {
            friendsNotificationsServices.OpenServices();

            friendsNotificationsServices.OnNewRequest += NotifyNewRequest;
            friendsNotificationsServices.OnRequestsNumberChanged += view.UpdateAvatarButtonRequestsNumber;
            friendsNotificationsServices.OnNewFriend += NotifyNewFriend;
            friendsNotificationsServices.OnRequestConfirmed += NotifyConfirmedRequest;
            InputManager.OnTapAvatar += OnTapAvatar;

            view.OnRequestButton.Subscribe(ShowRequestList).AddTo(disposables);
            view.OnFriendButton.Subscribe(ShowFriendList).AddTo(disposables);

            this.view = view;
            this.webClient = webClient;
            this.gameData = gameData;
            this.view.OnAvatarButton.Subscribe(Present).AddTo(sectionDisposables);
            this.view.OnDisposeView.Subscribe(CleanUp).AddTo(disposables);

            sectionDisposables.AddTo(disposables);

            foreach (var btn in view.OnCloseButtons)
            {
                btn.Subscribe(RollBackSection).AddTo(disposables);
            }
        }

        async void OnTapAvatar(AvatarRoomController avatarRoomController)
        {
            if (view.Card.gameObject.activeInHierarchy) return;

            view.Card.SetFriendCardLoading(avatarRoomController.AvatarData.Username);
            view.Card.gameObject.SetActive(true);
            if (avatarRoomController.AvatarData.FirebaseId.Equals(gameData.GetUserInformation().FirebaseId))
            {
                Present();
                return;
            }

            view.Card.snapshotAvatar.CreateSnaphot(null, avatarRoomController.AvatarData.AvatarCustomizationData);
            var task = webClient.GetUser(avatarRoomController.AvatarData.FirebaseId);
            await task;
            PresentAtWorldCard(task.Result);
        }

        void ShowFriendList()
        {
            SetCurrentSection(ViewSection.FriendListModal);

            requestButtonsDisposables.Clear();

            FillFriendsList().ToObservable().Subscribe();

            view.SocialList.OnFriendsTab.Subscribe(UpdateTabs)
                .AddTo(sectionDisposables);

            view.SocialList.OnRequestsTab.Subscribe(UpdateTabs)
                .AddTo(sectionDisposables);

            SubscribeCardButtons();

            SubscribeVisitButtons();

            view.ShowSection(GetCurrentSection());
        }

        void ShowRequestList()
        {
            requestButtonsDisposables.Clear();
            if (view.IsShowingFriendRows())
            {
                view.SwitchTabs();
            }

            SetCurrentSection(ViewSection.FriendListModal);

            FillRequestsList().ToObservable().Subscribe();

            SubscribeAcceptButtons();
            SubscribeRejectButtons();

            view.SocialList.OnFriendsTab.Subscribe(UpdateTabs)
                .AddTo(sectionDisposables);

            view.SocialList.OnRequestsTab.Subscribe(UpdateTabs)
                .AddTo(sectionDisposables);

            view.ShowSection(GetCurrentSection());
        }

        void Present()
        {
            sectionDisposables.Clear();
            switch (GetCurrentSection())
            {
                case ViewSection.First:
                    SetCurrentSection(ViewSection.PersonalCard);
                    PresentPersonalCard();
                    break;
                case ViewSection.PersonalCard:
                    SetCurrentSection(ViewSection.FriendListModal);
                    PresentFriendsList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Close()
        {
            view.HideSections();
            sectionDisposables.Clear();
            requestButtonsDisposables.Clear();
            view.OnAvatarButton.Subscribe(Present).AddTo(sectionDisposables);
        }

        void RollBackSection()
        {
            view.HideSections();
            RollBackCurrentSection();

            switch (GetCurrentSection())
            {
                case ViewSection.First:
                    sectionDisposables.Clear();
                    requestButtonsDisposables.Clear();
                    view.OnAvatarButton.Subscribe(Present).AddTo(sectionDisposables);
                    break;
                case ViewSection.PersonalCard:
                    sectionDisposables.Clear();
                    PresentPersonalCard();
                    break;
                case ViewSection.FriendListModal:
                    sectionDisposables.Clear();
                    PresentFriendsList();
                    break;
                case ViewSection.UnfriendModal:
                    PresentFriendCard();
                    break;
                case ViewSection.OkBox:
                    break;
                case ViewSection.StartReportBox:
                    reportButtonsDisposables.Clear();
                    PresentFriendCard();
                    break;
                case ViewSection.BlockBox:
                    reportButtonsDisposables.Clear();
                    PresentFriendCard();
                    break;
                case ViewSection.FriendCard:
                    reportButtonsDisposables.Clear();
                    PresentFriendCard();
                    break;
                case ViewSection.ReasonBoxFromStart:
                    reportButtonsDisposables.Clear();
                    PresentFriendCard();
                    break;
                case ViewSection.StrangerCard:
                    PresentStrangerCard();
                    break;
                case ViewSection.AlsoOkBox:
                    sectionDisposables.Clear();
                    reportButtonsDisposables.Clear();
                    view.OnAvatarButton.Subscribe(Present).AddTo(sectionDisposables);
                    break;
                case ViewSection.DoneBox:
                    sectionDisposables.Clear();
                    reportButtonsDisposables.Clear();
                    view.OnAvatarButton.Subscribe(Present).AddTo(sectionDisposables);
                    break;
                case ViewSection.OtherBox:
                    SetCurrentSection(ViewSection.ReasonBox);
                    view.ShowSection(GetCurrentSection());
                    break;
            }
        }

        void RollBackCurrentSection()
        {
            switch (CurrentSection)
            {
                case ViewSection.PersonalCard:
                    CurrentSection = ViewSection.First;
                    break;
                case ViewSection.FriendListModal:
                    CurrentSection = ViewSection.PersonalCard;
                    break;
                case ViewSection.FriendCard:
                    CurrentSection = ViewSection.First;
                    break;
                case ViewSection.FriendReqCard:
                    CurrentSection = ViewSection.FriendListModal;
                    break;
                case ViewSection.StrangerCard:
                    CurrentSection = ViewSection.First;
                    break;
                case ViewSection.OkBox:
                    CurrentSection = ViewSection.FriendListModal;
                    break;
                case ViewSection.AlsoReportBox:
                    CurrentSection = ViewSection.First;
                    break;
                case ViewSection.AlsoBlockBox:
                    CurrentSection = ViewSection.First;
                    break;
                case ViewSection.AlsoOkBox:
                    CurrentSection = ViewSection.First;
                    break;
                case ViewSection.UnableVisitOkCard:
                    CurrentSection = ViewSection.FriendCard;
                    break;
                case ViewSection.UnableVisitOkList:
                    CurrentSection = ViewSection.FriendListModal;
                    break;
                case ViewSection.ReasonBox:
                    CurrentSection = ViewSection.First;
                    break;
                case ViewSection.ReasonBoxFromStart:
                    CurrentSection = ViewSection.FriendCard;
                    break;
                case ViewSection.DoneBox:
                    CurrentSection = ViewSection.First;
                    break;
                case ViewSection.ConfirmAddFriend:
                    CurrentSection = ViewSection.StrangerCard;
                    break;
            }
        }

        void NotifyNewRequest(string newUser, List<FriendRequestData> requestList)
        {
            view.PlayNotification(newUser, true);

            //view.SetFriendRequestModal(requestList);//Todo: ask Mati about avatar snapshot state property
        }

        void NotifyNewFriend(string newUser, string firebaseUid, List<FriendData> friendsList)
        {
            view.PlayNotification(newUser, false);

            if (CurrentRoom.Instance.IsMyRoom()) return;

            var avatarRoomController = CurrentRoom.Instance.AvatarsManager.GetAvatarById(firebaseUid);

            if (avatarRoomController == null) return;

            avatarRoomController.AvatarNotificationController.PlayNotification(Notification.NotificationType.NewFriend);
        }

        void NotifyConfirmedRequest(string firebaseUid)
        {
            if (CurrentRoom.Instance.IsMyRoom()) return;

            var avatarRoomController = CurrentRoom.Instance.AvatarsManager.GetAvatarById(firebaseUid);

            if (avatarRoomController == null) return;

            avatarRoomController.AvatarNotificationController.PlayNotification(Notification.NotificationType.NewFriend);
        }

        void PresentAtWorldCard(UserData oData)
        {
            webClient.GetUsersRelationship(oData)
                .Do(r =>
                {
                    switch (r.relationship)
                    {
                        case UserRelationship.Friends:
                            PresentFriendCard(r.friendData);
                            break;
                        case UserRelationship.RequestReceived:
                            PresentRequestCard(oData.ToRequestData(r.requestData.friendRequestID,
                                RequestStatusOptions.Pending));
                            break;
                        case UserRelationship.RequestSent:
                            PresentStrangerCard(oData, true);
                            break;
                        case UserRelationship.Strangers:
                            PresentStrangerCard(oData, false);
                            break;
                    }
                })
                .DoOnError(Debug.Log)
                .Subscribe()
                .AddTo(sectionDisposables);
        }

        void PresentPersonalCard()
        {

            view.ShowSection(GetCurrentSection());
            view.SetPersonalCard(UserData.FromUserInfoToUserData(gameData.GetUserInformation()));

            view.Card.snapshotAvatar.CreateSnapshot();

            view.OnFriendsButton
                .Subscribe(Present).AddTo(sectionDisposables);
        }


        void PresentFriendsList()
        {
            if (view.IsShowingFriendRows())
            {
                FillFriendsList().ToObservable().Subscribe();
            }
            else
            {
                FillRequestsList().ToObservable().Subscribe();
                SubscribeAcceptButtons();
                SubscribeRejectButtons();
            }

            view.SocialList.OnFriendsTab.Subscribe(UpdateTabs)
                .AddTo(sectionDisposables);

            view.SocialList.OnRequestsTab.Subscribe(UpdateTabs)
                .AddTo(sectionDisposables);

            SubscribeCardButtons();

            SubscribeVisitButtons();

            view.ShowSection(GetCurrentSection());
        }

        void PresentFriendCard()
        {
            SetCurrentSection(ViewSection.FriendCard);
            view.ShowSection(GetCurrentSection());
        }

        void PresentFriendCard(FriendData friendData)
        {
            SetCurrentSection(ViewSection.FriendCard);
            view.ShowSection(GetCurrentSection());
            view.SetFriendCard(friendData);

            view.OnVisitFriendsBtn.Do(_ => VisitFriend(friendData)).Subscribe()
                .AddTo(sectionDisposables);
            view.OnUnFriendsButton.Do(_ => PresentUnfriendModal(friendData))
                .Subscribe().AddTo(sectionDisposables);

            foreach (var btn in view.OnReportButtons)
            {
                btn.Do(_ => PresentReportBox(friendData.ToUserData(), UserRelationship.Friends)).Subscribe()
                    .AddTo(sectionDisposables);
            }
            if (friendData != null)
                view.DialogsLayout.SetUsernameFields(friendData.username);
        }

        void PresentRequestCard(FriendRequestData requestData)
        {
            SetCurrentSection(ViewSection.FriendReqCard);

            view.SetRequestCard(requestData);
            view.ShowSection(GetCurrentSection());

            view.OnReqCardAcceptBtn
                .Select(t => webClient.GetFriend(requestData.requesterUserID.ToString()).ToObservable())
                .Do(PresentFriendCard)
                .Subscribe(_ => AcceptFriendRequest(requestData))
                .AddTo(sectionDisposables);

            view.OnReqCardRejectBtn
                .Do(_ => RejectFriendRequest(requestData))
                .Subscribe()
                .AddTo(sectionDisposables);
        }

        void PresentStrangerCard()
        {
            SetCurrentSection(ViewSection.StrangerCard);
            view.ShowSection(GetCurrentSection());
        }

        void PresentStrangerCard(UserData userData, bool isRequestSent)
        {
            //If local user sent a request it should be updated in case of blocking user.
            var relationship = isRequestSent ? UserRelationship.RequestSent : UserRelationship.Unknown;

            foreach (var btn in view.OnReportButtons)
            {
                btn.Do(_ => PresentReportBox(userData, relationship)).Subscribe().AddTo(sectionDisposables);
            }

            SetCurrentSection(ViewSection.StrangerCard);

            view.ShowSection(GetCurrentSection());
            view.SetStrangerCard(userData, isRequestSent);

            if (!isRequestSent)
            {
                view.OnAddFriendButton
                    .Do(view.Card.SendRequest)
                    .Subscribe(
                        _ => PresentAddFriend(userData))
                    .AddTo(sectionDisposables);
            }
        }

        void PresentUnfriendModal(FriendData data)
        {
            SetCurrentSection(ViewSection.UnfriendModal);
            view.DialogsLayout.UnfriendDialog.SetUnfriendBox(data);
            view.ShowSection(GetCurrentSection());
            view.DialogsLayout.UnfriendDialog.OnConfirmBtn
                .Do(_ => PresentUnfriendOk(data)).Subscribe()
                .AddTo(sectionDisposables);
        }

        void PresentUnfriendOk(FriendData friendData)
        {
            DeleteFriend(friendData);
            SetCurrentSection(ViewSection.OkBox);
            view.ShowSection(GetCurrentSection());
        }

        void PresentUnableVisitOk()
        {
            SetCurrentSection(GetCurrentSection().Equals(ViewSection.FriendListModal)
                ? ViewSection.UnableVisitOkList
                : ViewSection.UnableVisitOkCard);

            view.ShowSection(GetCurrentSection());
        }

        void PresentReportBox(UserData data, UserRelationship userRelationship)
        {
            view.DialogsLayout.OnBlock.Subscribe(_ => PresentBlockBox(data, userRelationship))
                .AddTo(reportButtonsDisposables);
            view.DialogsLayout.OnReport.Subscribe(_ => PresentReasonBox(data, userRelationship))
                .AddTo(reportButtonsDisposables);
            SetCurrentSection(ViewSection.StartReportBox);
            view.ShowSection(GetCurrentSection());
        }

        void PresentBlockBox(UserData userData, UserRelationship userRelationship)
        {
            view.DialogsLayout.OnConfirmBlocking.Do(_ => BlockUser(userData, userRelationship)
                ).Subscribe(_ => PresentAlsoReportBox(userData, userRelationship))
                .AddTo(reportButtonsDisposables);
            SetCurrentSection(ViewSection.BlockBox);
            view.ShowSection(GetCurrentSection());
        }

        void BlockUser(UserData userData, UserRelationship relationship)
        {
            webClient.BlockUser(userData)
                .Do(response => Debug.Log("Block user response: " + response))
                .DoOnError(Debug.Log)
                .Subscribe().AddTo(sectionDisposables);

            if (relationship.Equals(UserRelationship.Friends))
            {
                DeleteFriend(userData.ToFriendData());
            }

            if (relationship.Equals(UserRelationship.RequestSent))
            {
                webClient.GetUsersRelationship(userData)
                    .Do(r =>
                    {
                        //If user blocks another user the request status is updated as "rejected"
                        UpdateFriendRequest(
                            userData.ToRequestData(r.requestData.friendRequestID, RequestStatusOptions.Rejected),
                            RequestStatusOptions.Rejected, true);
                    })
                    .DoOnError(Debug.Log)
                    .Subscribe()
                    .AddTo(sectionDisposables);
            }
        }

        void PresentAlsoReportBox(UserData friendData, UserRelationship userRelationship)
        {
            view.DialogsLayout.OnJustBlock.Subscribe(PresentAlsoOkBox)
                .AddTo(reportButtonsDisposables);

            view.DialogsLayout.OnAlsoReport
                .Subscribe(_ => PresentReasonBox(friendData, userRelationship))
                .AddTo(reportButtonsDisposables);

            SetCurrentSection(ViewSection.AlsoReportBox);
            view.ShowSection(GetCurrentSection());
        }

        void PresentAlsoOkBox()
        {
            SetCurrentSection(ViewSection.AlsoOkBox);
            view.ShowSection(GetCurrentSection());
        }

        void PresentReasonBox(UserData userData, UserRelationship userRelationship)
        {
            if (GetCurrentSection().Equals(ViewSection.AlsoReportBox))
            {
                SetCurrentSection(ViewSection.ReasonBox);

                var reasonBtns = view.DialogsLayout.ReportReasonBtns.ToList();

                foreach (var reasonBtn in reasonBtns)
                {
                    var index = reasonBtns.FindIndex(0, x => x.Equals(reasonBtn));
                    var reason = view.DialogsLayout
                        .ReportReason[index];

                    if (reason.Equals(ReportReasons.other))
                    {
                        reasonBtn.Do(_ => PresentReportOtherBox(userData, reason))
                            .Subscribe()
                            .AddTo(reportButtonsDisposables);
                    }
                    else
                    {
                        reasonBtn.Do(_ => ReportUser(userData, reason, null).ToObservable().Subscribe())
                            .Do(PresentDoneBox)
                            .Subscribe()
                            .AddTo(reportButtonsDisposables);
                    }
                }

                view.ShowSection(GetCurrentSection());
            }
            else if (GetCurrentSection().Equals(ViewSection.StartReportBox))
            {
                SetCurrentSection(ViewSection.ReasonBox);

                var reasonBtns = view.DialogsLayout.ReportReasonBtns.ToList();
                foreach (var reasonBtn in reasonBtns)
                {
                    var index = reasonBtns.FindIndex(0, x => x.Equals(reasonBtn));
                    var reason = view.DialogsLayout
                        .ReportReason[index];

                    if (reason.Equals(ReportReasons.other))
                    {
                        reasonBtn.Do(_ => PresentReportOtherBox(userData, reason))
                            .Subscribe()
                            .AddTo(reportButtonsDisposables);
                    }
                    else
                    {
                        reasonBtn.Do(_ => ReportUser(userData, reason, null).ToObservable().Subscribe())
                            .Do(_ => PresentAlsoBlock(userData, userRelationship))
                            .Subscribe()
                            .AddTo(reportButtonsDisposables);
                    }
                }

                view.ShowSection(GetCurrentSection());
            }
        }


        void PresentAlsoBlock(UserData userData, UserRelationship userRelationship)
        {
            view.DialogsLayout.OnJustReport.Subscribe(PresentDoneBox)
                .AddTo(reportButtonsDisposables);

            view.DialogsLayout.OnAlsoBlock
                .Do(PresentAlsoOkBox)
                .Subscribe(_ => BlockUser(userData, userRelationship))
                .AddTo(reportButtonsDisposables);

            SetCurrentSection(ViewSection.AlsoBlockBox);
            view.ShowSection(GetCurrentSection());
        }

        private void PresentDoneBox()
        {
            SetCurrentSection(ViewSection.DoneBox);
            view.ShowSection(GetCurrentSection());
        }

        private async Task ReportUser(UserData data, ReportReasons reason, string description)
        {
            await webClient.ReportUser(data, reason.ToString(), description);
        }

        private void PresentReportOtherBox(UserData userData1, ReportReasons reason)
        {
            reportOtherDisposables.Clear();
            view.DialogsLayout.OtherReasonField.onSubmit.RemoveAllListeners();

            view.DialogsLayout.OtherReasonField.text = "";

            view.DialogsLayout.OtherReasonButton.Subscribe(_ =>
            {
                ReportUser(userData1, reason, view.DialogsLayout.OtherReasonField.text).ToObservable().Subscribe();
                PresentDoneBox();
            }).AddTo(reportOtherDisposables);

            view.DialogsLayout.OtherReasonField.onSubmit.AddListener(delegate
            {
                ReportUser(userData1, reason, view.DialogsLayout.OtherReasonField.text).ToObservable().Subscribe();
                PresentDoneBox();
            });

            SetCurrentSection(ViewSection.OtherBox);
            view.ShowSection(GetCurrentSection());
        }

        async void VisitFriend(FriendData friendData)
        {
            var updatedData = await webClient.GetFriend(friendData.userID.ToString());

            if (updatedData.IsInPublicRoom)
            {
                if (string.IsNullOrEmpty(updatedData.roomInstanceId) ||
                    string.IsNullOrEmpty(updatedData.roomNamePrefab) ||
                    string.IsNullOrEmpty(updatedData.roomType)) return;

                RoomInformation room = new RoomInformation(updatedData.roomInstanceId, null, 0, 0,
                    updatedData.roomNamePrefab, false, 0, 0, updatedData.roomType, 0, "", RoomInformation.EVENT_STATE.NONE);

                CurrentRoom.Instance.GoToNewRoom(room);
            }
            else
            {
                PresentUnableVisitOk();
            }
        }

        async Task FillFriendsList()
        {
            var data = await webClient.GetFriendsList();
            view.SetFriendModal(data);
            gameData.SetFriendlist(data);
            if (data.Count == 0)
                await FillRequestsList();
        }

        async Task SetFriendsListButton()
        {
            var data = await webClient.GetFriendsList();
            view.SetFriendListButton(data.Count);
        }

        async Task FillRequestsList()
        {
            var data = await webClient.GetFriendRequestsList();
            view.SetFriendRequestModal(data);
            await SetFriendsListButton();
        }

        void AcceptFriendRequest(FriendRequestData data)
        {
            friendsNotificationsServices.InsertTrimNewFriend(data.requesterUserID, data.username, data.fireBaseUid);

            UpdateFriendRequest(data, RequestStatusOptions.Accepted, false);
            CreateFriend(data);
        }

        void RejectFriendRequest(FriendRequestData data)
        {
            UpdateFriendRequest(data, RequestStatusOptions.Rejected, false);
        }

        void UpdateFriendRequest(FriendRequestData requestData, RequestStatusOptions answer, bool sent)
        {
            webClient.UpdateFriendRequest(new FriendRequestData(friendRequestID: requestData.friendRequestID,
                    username: requestData.username, realName: requestData.realName,
                    requesterUserID: requestData.requesterUserID, fireBaseUid: requestData.fireBaseUid,
                    requestStatus: answer, friendCount: requestData.friendCount, null), sent)
                .DoOnError(Debug.Log)
                .Subscribe().AddTo(sectionDisposables);
        }
        //Todo: assure null value never will be needed!

        void CreateFriend(FriendRequestData data)
        {
            webClient.CreateFriend(data)
                .Do(response => Debug.Log("Creating friend response: " + response))
                .DoOnError(Debug.Log)
                .Subscribe().AddTo(sectionDisposables);
        }

        void PresentAddFriend(UserData friendRequestData)
        {
            view.DialogsLayout.OnConfirmAdd
                .Subscribe(_ => CreateFriendRequest(friendRequestData)).AddTo(sectionDisposables);

            SetCurrentSection(ViewSection.ConfirmAddFriend);
            view.ShowSection(GetCurrentSection());
        }


        void CreateFriendRequest(UserData requestData)
        {
            webClient.AddFriendRequest(requestData)
                .Do(response => Debug.Log("Create friend request response: " + response))
                .DoOnError(Debug.Log)
                .Subscribe().AddTo(sectionDisposables);

            SetCurrentSection(ViewSection.StrangerCard);
            view.ShowSection(GetCurrentSection());
        }

        void DeleteFriend(FriendData friendData)
        {
            webClient.DeleteFriend(friendData)
                .Do(response => Debug.Log("Delete friend user response: " + response))
                .DoOnError(Debug.Log)
                .Subscribe().AddTo(sectionDisposables);
        }

        void CleanUp()
        {
            disposables.Clear();
            friendsNotificationsServices.OnNewRequest -= NotifyNewRequest;
            friendsNotificationsServices.OnRequestsNumberChanged -= view.UpdateAvatarButtonRequestsNumber;

            friendsNotificationsServices.OnNewFriend -= NotifyNewFriend;
            friendsNotificationsServices.OnRequestConfirmed -= NotifyConfirmedRequest;
            InputManager.OnTapAvatar -= OnTapAvatar;

            friendsNotificationsServices.CloseServices();

            reportOtherDisposables.Clear();
            view.DialogsLayout.OtherReasonField.onSubmit.RemoveAllListeners();
        }

        void SetCurrentSection(ViewSection section)
        {
            CurrentSection = section;
        }

        ViewSection GetCurrentSection()
        {
            return CurrentSection;
        }

        void UpdateTabs()
        {
            requestButtonsDisposables.Clear();
            view.SwitchTabs();
            if (view.IsShowingFriendRows())
            {
                FillFriendsList().ToObservable().Subscribe();
            }
            else
            {
                FillRequestsList().ToObservable().Subscribe();
                SubscribeAcceptButtons();
                SubscribeRejectButtons();
            }
        }


        private void SubscribeVisitButtons()
        {
            var btns = view.SocialList.OnVisitFriendBtns.ToList();
            foreach (var btn in btns)
            {
                btn.Do(_ => VisitFriend(view.SocialList.friendRows[btns.FindIndex(0, x => x.Equals(btn))]
                        .GetFriendRowData())).Subscribe()
                    .AddTo(sectionDisposables);
            }
        }

        private void SubscribeCardButtons()
        {
            var btns = view.SocialList.OnFriendCardBtns.ToList();
            foreach (var btn in btns)
            {
                btn.Do(_ =>
                    {
                        var rowData = view.SocialList.friendRows[btns.FindIndex(0, x => x.Equals(btn))]
                            .GetFriendRowData();
                        view.Card.snapshotAvatar.CreateSnaphot(null, rowData.avatarCustomizationData);
                        PresentFriendCard(rowData);
                    })
                    .Subscribe()
                    .AddTo(sectionDisposables);
            }

            var rowBtns = view.SocialList.OnFriendRowBtns.ToList();
            foreach (var btn in rowBtns)
            {
                btn.Do(_ =>
                    {
                        var rowData = view.SocialList.friendRows[rowBtns.FindIndex(0, x => x.Equals(btn))]
                            .GetFriendRowData();
                        view.Card.snapshotAvatar.CreateSnaphot(null, rowData.avatarCustomizationData);
                        PresentFriendCard(rowData);
                    }).Subscribe()
                    .AddTo(sectionDisposables);
            }

            var reqRowBtns = view.SocialList.OnRequestRowBtns.ToList();
            foreach (var btn in reqRowBtns)
            {
                btn.Do(_ =>
                    {
                        var rowData = view.SocialList.requestsRows[reqRowBtns.FindIndex(0, x => x.Equals(btn))]
                            .GetFriendRequestData();
                        view.Card.snapshotAvatar.CreateSnaphot(null, rowData.avatarCustomizationData);
                        PresentRequestCard(rowData);
                    }).Subscribe()
                    .AddTo(sectionDisposables);
            }
        }

        void SubscribeAcceptButtons()
        {
            var btns = view.SocialList.OnAcceptBtn.ToList();
            foreach (var btn in btns)
            {
                btn.Do(_ => AcceptFriendRequest(view.SocialList.requestsRows[btns.FindIndex(0, x => x.Equals(btn))]
                        .GetFriendRequestData()))
                    .Subscribe(view.SocialList.requestsRows[btns.FindIndex(0, x => x.Equals(btn))].AcceptRequest)
                    .AddTo(requestButtonsDisposables);
            }
        }

        void SubscribeRejectButtons()
        {
            var btns = view.SocialList.OnRejectBtn.ToList();
            foreach (var btn in btns)
            {
                btn.Do(_ => RejectFriendRequest(view.SocialList.requestsRows[btns.FindIndex(0, x => x.Equals(btn))]
                        .GetFriendRequestData()))
                    .Subscribe(view.SocialList.requestsRows[btns.FindIndex(0, x => x.Equals(btn))].RejectRequest)
                    .AddTo(requestButtonsDisposables);
            }
        }
    }
}
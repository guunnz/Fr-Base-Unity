using System;
using System.Collections.Generic;
using Architecture.MVP;
using FriendsView.Core.Domain;
using FriendsView.View;
using UniRx;

namespace FriendsView.Presentation
{
    public interface IFriendsView : IPresentable
    {
        IObservable<Unit> OnAvatarButton { get; }
        IObservable<Unit> OnFriendsButton { get; }
        IObservable<Unit> OnUnFriendsButton { get; }

        IEnumerable<IObservable<Unit>> OnCloseButtons { get; }
        IEnumerable<IObservable<Unit>> OnReportButtons { get; }

        DialogsLayout DialogsLayout { get; }
        ModalCard Card { get; }

        ModalSocial SocialList { get; }
        IObservable<Unit> OnVisitFriendsBtn { get; }
        IObservable<Unit> OnReqCardAcceptBtn { get; }
        IObservable<Unit> OnReqCardRejectBtn { get; }
        IObservable<Unit> OnAddFriendButton { get; }
        IObservable<Unit> OnStart { get; }
        IObservable<Unit> OnRequestButton { get; }
        IObservable<Unit> OnFriendButton { get; }

        void ShowSection(ViewSection viewSection);

        void HideSections();
        void SetPersonalCard(UserData userData);
        void SetFriendCard(FriendData friendData);

        void SetFriendModal(List<FriendData> friend);
        void SetFriendRequestModal(List<FriendRequestData> friendReq);
        void SwitchTabs();
        bool IsShowingFriendRows();
        void SetRequestCard(FriendRequestData data);
        void SetStrangerCard(UserData data, bool requested);
        void UpdateAvatarButtonRequestsNumber(int request);
        void PlayNotification(string newUser, bool isRequest);

        void SetFriendListButton(int friendCount);
    }
}
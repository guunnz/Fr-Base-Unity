using System;
using System.Collections.Generic;
using Architecture.MVP;
using PlayerRoom.Core.Domain;
using UniRx;

namespace PlayerRoom.Presentation
{
    public interface IRoomListView : IPresentable
    {
        void ShowAreas(IEnumerable<RoomInfo> rooms);
        void ShowInstances(IEnumerable<RoomInfo> rooms);
        IObservable<string> OnClickRoom { get; }
        void Hide();
        IObservable<Unit> OnCloseButton { get; }
        IObservable<Unit> OnBackButton { get; }
        void ResetTabs();
    }
}
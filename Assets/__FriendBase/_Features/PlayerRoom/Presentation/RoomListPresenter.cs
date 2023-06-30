using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MemoryStorage.Core;
using PlayerRoom.Core.Services;
using UniRx;
using WorldNode.View;

namespace PlayerRoom.Presentation
{
    [UsedImplicitly]
    public class RoomListPresenter
    {
        readonly IRoomListView view;
        readonly IRoomLoader roomLoader;
        readonly IPauseManager pause;
        readonly IRoomsService roomsService;
        readonly IMemoryStorage memoryStorage;

        readonly CompositeDisposable disposables = new CompositeDisposable();
        readonly CompositeDisposable showDisposables = new CompositeDisposable();


        string roomShowing;

        string CurrentRoomName
        {
            get => memoryStorage.Get("currentRoomName");
            set => memoryStorage.Set("currentRoomName", value);
        }

        string CurrentRoomId
        {
            get => memoryStorage.Get("currentRoomId");
            set => memoryStorage.Set("currentRoomId", value);
        }

        public RoomListPresenter(IRoomListView view, IRoomLoader roomLoader, IPauseManager pause,
            IRoomsService roomsService, IMemoryStorage memoryStorage)
        {
            this.view = view;
            this.roomLoader = roomLoader;
            this.pause = pause;
            this.roomsService = roomsService;
            this.memoryStorage = memoryStorage;

            this.view.OnShowView
                .Do(_ => roomShowing = string.Empty) // to avoid go back on instances options
                .Do(_ => Present())
                .Subscribe()
                .AddTo(disposables);

            this.view.OnHideView
                .Do(_ => Hide())
                .Subscribe()
                .AddTo(disposables);

            this.view.OnDisposeView.Do(_ => CleanUp()).Subscribe().AddTo(disposables);
            showDisposables.AddTo(disposables);
        }

        void Present()
        {
            view.ResetTabs(); //jic reset tabs

            if (string.IsNullOrEmpty(roomShowing)) //case showing the rooms
            {
                view.OnClickRoom
                    .Do(roomID => roomShowing = roomID)
                    .Do(() =>
                    {
                        showDisposables.Clear();
                        Present();
                    })
                    .Subscribe()
                    .AddTo(showDisposables);

                roomsService.GetRoomsIDs()
                    .Do(rooms => view.ShowAreas(rooms))
                    .Subscribe()
                    .AddTo(showDisposables);
            }
            else //case showing a room instances
            {
                roomsService.GetRoomInstances(roomShowing)
                    .Do(rooms => view.ShowInstances(rooms.OrderBy(r1 => -r1.PlayersOnRoom)))
                    .Subscribe()
                    .AddTo(showDisposables);

                view.OnClickRoom
                    .Do(() => view.Hide())
                    .SelectMany(instanceId => ConnectNewRoomAsync(instanceId).ToObservable())
                    .Subscribe()
                    .AddTo(showDisposables);

                view.OnBackButton
                    .Do(() =>
                    {
                        roomShowing = string.Empty;
                        showDisposables.Clear();
                        Present();
                    })
                    .Subscribe()
                    .AddTo(showDisposables);
            }


            view.OnCloseButton
                .Do(() => view.Hide())
                .Do(() => roomShowing = string.Empty)
                .Subscribe()
                .AddTo(showDisposables);
        }


        async Task ConnectNewRoomAsync(string instanceId)
        {
            //Behavior moved to RoomViewManager Class:

            // Debug.Log($"Connect to chat room --> {roomShowing} :: {instanceId}");
            //
            // if (!string.IsNullOrEmpty(CurrentRoomName) && !string.IsNullOrEmpty(CurrentRoomId))
            // {
            //     await socketManager.LeaveChatRoom(CurrentRoomName, CurrentRoomId);
            // }
            //
            // var pos = playerWorldData.Position;
            //
            // await socketManager.JoinChatRoom(roomShowing, instanceId, pos.x, pos.y);
            //
            // //cache new room info
            // CurrentRoomName = roomShowing;
            // CurrentRoomId = instanceId;
            //
            // await roomLoader.LoadRoom(instanceId);

            await roomLoader.ConnectNewRoomAsync(roomShowing, instanceId);
            roomShowing = string.Empty;
        }

        void Hide()
        {
            showDisposables.Clear();
        }

        void CleanUp()
        {
            disposables.Clear();
        }
    }
}
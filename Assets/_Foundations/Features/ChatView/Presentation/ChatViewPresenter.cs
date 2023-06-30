using Architecture.Injector.Core;
using ChatView.Core.Domain;
using ChatView.Core.Services;
using ChatView.Infrastructure;
using Data;
using JetBrains.Annotations;
using Socket;
using UniRx;

namespace ChatView.Presentation
{
    [UsedImplicitly]
    public class ChatViewPresenter
    {
        readonly IChatView view;
        readonly IChatServices services;
        private readonly IGameData gameData;

        readonly CompositeDisposable disposables = new CompositeDisposable();
        readonly CompositeDisposable buttonDisposables = new CompositeDisposable();

        public ChatViewPresenter(IChatView view)
        {
            this.view = view;
            services = new ChatServices();

            gameData = Injection.Get<IGameData>();

            view
                .OnEnabled
                .Subscribe(_ => Present())
                .AddTo(disposables);
            view
                .OnDisabled
                .Subscribe(_ => Hide())
                .AddTo(disposables);
            view
                .OnDisposed
                .Subscribe(_ => CleanUp())
                .AddTo(disposables);

            buttonDisposables.AddTo(disposables);


            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.CHAT_MESSAGE, OnNewMessage);
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.PRIVATE_CHAT_MESSAGE, OnNewPrivateMessage);
            InputManager.OnTapBubblePrivateChat += OnTapBubblePrivateChat;
        }


        private void OnTapBubblePrivateChat(AvatarRoomController avatarRoomController)
        {
            UnityEngine.Debug.Log("BUBBLETAP");
            view.SetPrivateChatUser(avatarRoomController.AvatarData.Username, int.Parse(avatarRoomController.AvatarData.UserId));
            view.ShowPrivateChat();
        }

        private void OnNewMessage(AbstractIncomingSocketEvent incomingEvent)
        {
            IncomingEventChatMessage incomingEventChatMessage = incomingEvent as IncomingEventChatMessage;
            if (incomingEventChatMessage != null &&
                incomingEventChatMessage.State == SocketEventResult.OPERATION_SUCCEED && CurrentRoom.Instance.AvatarsManager.GetAvatarById(incomingEventChatMessage.ChatMessageData.firebaseUid) != null)
            {
                ReceiveMessage(incomingEventChatMessage.ChatMessageData);
            }
        }

        private void OnNewPrivateMessage(AbstractIncomingSocketEvent incomingEvent)
        {
            IncomingEventPrivateChatMessage incomingEventChatMessage = incomingEvent as IncomingEventPrivateChatMessage;
            if (incomingEventChatMessage != null &&
                incomingEventChatMessage.State == SocketEventResult.OPERATION_SUCCEED && CurrentRoom.Instance.AvatarsManager.GetAvatarById(incomingEventChatMessage.ChatMessageData.firebaseUid) != null)
            {
                ReceivePrivateMessage(incomingEventChatMessage.ChatMessageData);
            }
        }

        private void Present()
        {
            view.Username = GetUserName();

            view
                .OnSend
                .Subscribe(_ => OnSend())
                .AddTo(buttonDisposables);
        }

        void ReceiveMessage(ChatData chatData)
        {
            if (chatData == null) return;

            view.SetMessages(chatData);

            var animationSeconds = services.MessageToSeconds(chatData.message);

            if (chatData.username.Equals(view.Username))
            {
                var avatarRoomController = CurrentRoom.Instance.AvatarsManager.GetMyAvatar();
                if (avatarRoomController == null ||
                    !avatarRoomController.CustomizationController.IsAvatarReady()) return;
                {
                    avatarRoomController.AnimationController.SetTalkAnimation(animationSeconds);
                }
            }
            else
            {
                var avatarRoomController = CurrentRoom.Instance.AvatarsManager.GetAvatarById(chatData.firebaseUid);

                if (avatarRoomController == null ||
                    !avatarRoomController.CustomizationController.IsAvatarReady()) return;
                avatarRoomController.AnimationController.SetTalkAnimation(animationSeconds);
            }
        }


        void ReceivePrivateMessage(ChatData chatData)
        {
            if (chatData == null) return;

            view.SetMessagePrivateChat(chatData);

            var animationSeconds = services.MessageToSeconds(chatData.message);


            var avatarRoomController = CurrentRoom.Instance.AvatarsManager.GetAvatarById(chatData.firebaseUid);

            if (avatarRoomController == null ||
                !avatarRoomController.CustomizationController.IsAvatarReady()) return;
            avatarRoomController.AnimationController.SetTalkAnimation(animationSeconds);
        }

        private void OnSend()
        {
            var textToSend = view.GetTextToSend();
            if (textToSend.Equals(""))
            {
                return;
            }

            if (!view.OnPrivateChat)
            {
                SimpleSocketManager.Instance.SendChatMessage(CurrentRoom.Instance.RoomInformation.RoomName,
              CurrentRoom.Instance.RoomInformation.RoomIdInstance, textToSend, GetUserName(),
              view.UsernameColor);
            }
            else
            {
                SimpleSocketManager.Instance.SendPrivateChat(this.view.GetPrivateChatUserId(), this.view.GetPrivateChatUsername(),
      CurrentRoom.Instance.RoomInformation.RoomIdInstance, textToSend);
            }
        }

        string GetUserName()
        {
            return gameData.GetUserInformation().UserName;
        }

        void Hide()
        {
            buttonDisposables.Clear();
        }

        void CleanUp()
        {
            SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.CHAT_MESSAGE, OnNewMessage);
            SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.PRIVATE_CHAT_MESSAGE, OnNewPrivateMessage);
            InputManager.OnTapBubblePrivateChat -= OnTapBubblePrivateChat;
            disposables.Clear();
        }
    }
}
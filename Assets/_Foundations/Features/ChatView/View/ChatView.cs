using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Architecture.Injector.Core;
using Architecture.MVP;
using ChatView.Core.Domain;
using ChatView.Infrastructure;
using ChatView.Presentation;
using Data;
using ResponsiveUtilities;
using Socket;
using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChatView.View
{
    [System.Serializable]
    public class PrivateChat
    {
        public string Chat;
        public string FirebaseUserId; //UserId of the user you are chatting with;
        public string Username; //UserId of the user you are chatting with;
        public bool IsChatOpen;
    }


    public class ChatView : WidgetBase, IChatView
    {
        string username;
        const int messagesFieldLimit = 1000;
        public string submitKey = "Submit";

        [SerializeField] GameObject chatUI;
        [SerializeField] Button sendButton;
        [SerializeField] Button closeButton;
        [SerializeField] TMP_InputField userMessage;
        [SerializeField] StringWidget receivedMessages;
        [SerializeField] StringWidget receivedMessagesPrivate;
        [SerializeField] ChatResponsiveInputUtilities chatResponsive;
        [SerializeField] GameObject welcomeMessage;
        
        [SerializeField] GameObject messagesBack;
        IGameData gameData;

        [SerializeField] Color userMessagesColor;


        [SerializeField] GameObject guestObject;

        [SerializeField] ChatAnimationPlayer chatAnimationPlayer;

        RandomColorUtilities randomColor;
        
        readonly ISubject<Unit> onEnabled = new Subject<Unit>();
        readonly ISubject<Unit> onDisabled = new Subject<Unit>();
        readonly ISubject<Unit> onDisposed = new Subject<Unit>();
        readonly ISubject<Unit> sendButtonSubject = new Subject<Unit>();

        readonly CompositeDisposable disposables = new CompositeDisposable();
        private Image buttonImage;
        string usernameColor;

        private string privateChatUsername;
        private int privateChatUserId;
        public TextMeshProUGUI ChatHeader;

        public static event Action OnCloseChat;

        public bool OnPrivateChat { get; set; }

        List<PrivateChat> privateChatList = new List<PrivateChat>();

        public void SetPrivateChatUser(string username, int userId)
        {
            privateChatUserId = userId;
            privateChatUsername = username;
        }

        public int GetPrivateChatUserId()
        {
            return privateChatUserId;
        }

        public string GetPrivateChatUsername()
        {
            return privateChatUsername;
        }

        void Awake()
        {
            gameData = Injection.Get<IGameData>();
            this.CreatePresenter<ChatViewPresenter, IChatView>();

            randomColor = new RandomColorUtilities(ChatServices.GetChatColorsList());
            ColorUtility.TryParseHtmlString(ChatServices.friendbaseBlackHex, out userMessagesColor);

            sendButton
                .OnClickAsObservable()
                .Subscribe(sendButtonSubject).AddTo(disposables);

            closeButton.onClick.AddListener(CloseChat);

            usernameColor = randomColor.RandomColorAsString();
            buttonImage = sendButton.GetComponent<Image>();

        }

        public IObservable<Unit> OnSend => sendButtonSubject;
        public IObservable<Unit> OnEnabled => onEnabled;
        public IObservable<Unit> OnDisabled => onDisabled;
        public IObservable<Unit> OnDisposed => onDisposed;

        public bool IsVisible => chatUI.activeSelf;

        public string UsernameColor => usernameColor;

        private Coroutine OnSubmitCoroutine;
        public string Username
        {
            get => username;
            set => username = value;
        }

        public void ShowChat()
        {
            //This is old Chat -> we do not show it
            return;

            if (gameData.IsGuest())
            {
                GuestBehaviour();
            }

            OnPrivateChat = false;
            receivedMessagesPrivate.gameObject.SetActive(false);
            receivedMessages.gameObject.SetActive(true);
            ChatHeader.text = "Chat<sprite=0>";
            CinemachineCamera.Singleton.SetCameraChatMode();
            chatAnimationPlayer.ChangeAnimationState(ChatAnimationStates.chat_bubble_static);
            chatUI.SetActive(true);
        }

        public void GuestBehaviour()
        {
            messagesBack.SetActive(false);
            userMessage.interactable = false;
            sendButton.interactable = false;
            guestObject.SetActive(true);
        }

        public void ShowPrivateChat()
        {
            //New private chat implementation
            CurrentRoom.Instance.chatManager.StartPrivateChatFromCard(privateChatUserId.ToString());

            return;
            //SimpleSocketManager.Instance.JoinPrivateChat(GetPrivateChatUserId());
            receivedMessagesPrivate.gameObject.SetActive(true);
            receivedMessages.gameObject.SetActive(false);
            OnPrivateChat = true;
            ChatHeader.text = privateChatUsername + "<sprite=0>";


            privateChatList.ForEach(x => x.IsChatOpen = false);

            if (privateChatList.Any(x => x.Username == privateChatUsername))
            {
                PrivateChat chat = privateChatList.Single(x => x.Username == privateChatUsername);
                chat.IsChatOpen = true;
                receivedMessagesPrivate.Value = chat.Chat;
            }
            else
            {
                receivedMessagesPrivate.Value = "";
            }

            CinemachineCamera.Singleton.SetCameraChatMode();
            chatAnimationPlayer.ChangeAnimationState(ChatAnimationStates.chat_bubble_static);
            chatUI.SetActive(true);
        }

        public void CloseChat()
        {
            privateChatList.ForEach(x => x.IsChatOpen = false);
            CinemachineCamera.Singleton.SetCameraNormalMode();
            chatAnimationPlayer.ChangeAnimationState(ChatAnimationStates.chat_bubble_static);
            chatUI.SetActive(false);
            OnCloseChat?.Invoke();
        }

        private PrivateChat GetPrivateChat(ChatData data)
        {
            PrivateChat chat = new PrivateChat();
            if (data.username == username)
            {
                chat = privateChatList.FirstOrDefault(x => x.FirebaseUserId == data.firebaseUid);
            }
            else
            {
                chat = privateChatList.FirstOrDefault(x => x.Username == data.username);
            }
            string SenderUsername = CurrentRoom.Instance.AvatarsManager.GetAvatarById(data.firebaseUid).AvatarData.Username;


            if (chat == null)
            {
                if (data.username == username)
                {
                    if (privateChatList.Any(x => x.Username == SenderUsername))
                    {
                        chat = privateChatList.Single(x => x.Username == SenderUsername);
                        chat.FirebaseUserId = data.firebaseUid;
                        chat.Username = SenderUsername;
                    }
                    else
                    {
                        chat = new PrivateChat();
                        chat.Chat = "";
                        chat.FirebaseUserId = data.firebaseUid;
                        chat.Username = SenderUsername;
                        privateChatList.Add(chat);
                    }
                }
                else
                {
                    chat = new PrivateChat();
                    chat.Chat = "";
                    chat.FirebaseUserId = "";
                    chat.Username = data.username;
                    privateChatList.Add(chat);
                }

            }
            //else
            //{
            //    if (privateChatList.Any(x => x.Username == ReceiverUsername))
            //    {
            //        chat = privateChatList.Single(x => x.Username == ReceiverUsername);
            //        chat.FirebaseUserId = data.firebaseUid;
            //        chat.Username = ReceiverUsername;
            //    }
            //}

            return chat;
        }

        public bool IsChatOpen(string FirebaseUserId)
        {
            if (!privateChatList.Any(x => x.FirebaseUserId == FirebaseUserId))
            {
                return false;
            }

            PrivateChat chat = privateChatList.Single(x => x.FirebaseUserId == FirebaseUserId);
            return chat.IsChatOpen;
        }

        public void SetMessagePrivateChat(ChatData data)
        {
            //ClearMessagesOverLimit();

            PrivateChat chat = GetPrivateChat(data);

            var dataUsername = CurrentRoom.Instance.AvatarsManager.GetAvatarById(data.firebaseUid).AvatarData.Username;
            var dataUsernameColor = data.usernameColor;
            var message = data.message;

            if (message.Equals(""))
            {
                return;
            }

            receivedMessagesPrivate.Value = chat.Chat;

            if (dataUsername.Equals(username))
            {
                receivedMessagesPrivate.Value += "<align=right><b><color=#" + "000000" + ">" +
                                          dataUsername + Environment.NewLine + "</color></b>" + " " + message + Environment.NewLine;
            }
            else
            {
                receivedMessagesPrivate.Value += "<align=left><b><color=#" + "000000" + ">" +
                                          dataUsername + "</color></b>" + ": " + Environment.NewLine + message + Environment.NewLine;
            }

            chat.Chat = receivedMessagesPrivate.Value;
        }

        public void SetMessages(ChatData data)
        {
            //ClearMessagesOverLimit();
            var dataUsername = data.username;
            var dataUsernameColor = data.usernameColor;
            var message = data.message;

            if (message.Equals(""))
            {
                return;
            }

            if (dataUsername.Equals(username))
            {
                receivedMessages.Value += "<align=right><b><color=#" + ColorUtility.ToHtmlStringRGBA(userMessagesColor) + ">" +
                                          dataUsername + Environment.NewLine + "</color></b>" + " " + message + Environment.NewLine;
            }
            else
            {
                if (!chatAnimationPlayer.CurrentAnimationState.Equals(ChatAnimationStates.chat_bubble_idle))
                {
                    chatAnimationPlayer.PlaysNewMessageSequence();
                }

                receivedMessages.Value += "<align=left><b><color=#" + ColorUtility.ToHtmlStringRGBA(dataUsernameColor) + ">" +
                                          dataUsername + "</color></b>" + ": " + Environment.NewLine + message + Environment.NewLine;
            }
        }

        void ClearMessagesOverLimit()
        {
            if (receivedMessages.Value.Length > messagesFieldLimit)
            {
                receivedMessages.Value = receivedMessages.Value.Remove(0, messagesFieldLimit);
            }
        }

        public void SetMessageField()
        {
            //Todo: Remove if and flag, when RoomViewManager loading twice gets fixed if possible 
            if (string.IsNullOrEmpty(ChatServices.ChatHistory)) return;

            receivedMessages.Value = ChatServices.ChatHistory;
            welcomeMessage.SetActive(false);

            if (!ChatServices.ClearHistory)
            {
                ChatServices.ClearHistory = true;
            }
            else
            {
                ChatServices.ClearHistory = false;
                ChatServices.ChatHistory = "";
            }
        }

        public string GetTextToSend()
        {
            var userMessageText = userMessage.text;
            ClearInputField();
            return userMessageText;
        }

        void ClearInputField()
        {
            if (!string.IsNullOrWhiteSpace(userMessage.text)) welcomeMessage.SetActive(false);
            userMessage.text = "";
        }

        void OnEnable()
        {
            userMessage.characterLimit = ChatServices.maxMessageLenght;
            userMessage.onSubmit.AddListener(delegate { Submit(); });
            userMessage.onEndEdit.AddListener(delegate { OnEndEdit(); });
            userMessage.onSelect.AddListener(delegate { chatResponsive.RelocateInputField(true); });

            onEnabled.OnNext(Unit.Default);

            FriendsView.View.FriendsView.OnCustomization += SaveChatHistory;
        }

        void OnDisable()
        {
            userMessage.onSubmit.RemoveAllListeners();
            userMessage.onEndEdit.RemoveAllListeners();
            userMessage.onSelect.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();

            onDisabled.OnNext(Unit.Default);

            FriendsView.View.FriendsView.OnCustomization -= SaveChatHistory;
        }

        private void SaveChatHistory()
        {
            ChatServices.ChatHistory = receivedMessages.Value;
        }

        void OnDestroy()
        {
            disposables.Clear();
            onDisposed.OnNext(Unit.Default);
            closeButton.onClick.RemoveAllListeners();
        }

        void Submit()
        {
            OnSubmitCoroutine = StartCoroutine(SubmitCoroutine());
        }

        void OnEndEdit()
        {
            if (OnSubmitCoroutine != null)
                return; /*If is submitting don't do on end edit behaviour*/
            //ToggleSendButton(true);
            chatResponsive.RelocateInputField(false);
        }

        IEnumerator SubmitCoroutine()
        {
            yield return null; //Wait a frame so keyboard closes
            sendButton.onClick.Invoke();
            yield return null; // wait a frame to reactivate keyboard
            userMessage.ActivateInputField();
            TouchScreenKeyboard.Open("");
            //ToggleSendButton(false);
            OnSubmitCoroutine = null;
        }
    }
}
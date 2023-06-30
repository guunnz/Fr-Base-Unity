using System;
using Architecture.MVP;
using ChatView.Core.Domain;
using UniRx;
using UnityEngine.EventSystems;

namespace ChatView.Presentation
{
    public interface IChatView : IPresentable
    {
        void SetPrivateChatUser(string username, int userId);
        IObservable<Unit> OnSend { get; }
        IObservable<Unit> OnEnabled { get; }
        IObservable<Unit> OnDisabled { get; }
        IObservable<Unit> OnDisposed { get; }
        bool IsVisible { get; }
        bool OnPrivateChat { get; set; }
        string Username { get; set; }
        string UsernameColor { get; }
        void ShowChat();

        void ShowPrivateChat();
        void CloseChat();
        void SetMessages(ChatData messageData);
        string GetTextToSend();
        int GetPrivateChatUserId();

        string GetPrivateChatUsername();


        void SetMessagePrivateChat(ChatData messageData);
    }
}
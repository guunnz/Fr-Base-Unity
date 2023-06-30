using System;
using NativeWebSocket;
using Newtonsoft.Json.Linq;
using UniRx;

namespace Socket
{
    public interface ISocketManager
    {
        IObservable<byte[]> OnMessage();
        IObservable<string> OnError();
        IObservable<WebSocketCloseCode> OnClose();
        IObservable<Unit> Connect();
        IObservable<Unit> SendEvent(string topic, string eventType, JObject payload, string eventRef = "none");
        IObservable<Unit> SendEvent(string topic, string eventType, string payloadString, string eventRef = "none");

        IObservable<Unit> JoinChatRoom(string roomName, string roomId, float positionX, float positionY);
        IObservable<Unit> SendMessage(string roomName, string roomId, string content);
        IObservable<Unit> SendChatMessage(string roomName, string roomId, string content, string username, string usernameColor);
        IObservable<Unit> GetPlayerPositions(string roomName, string roomId, bool forceRefesh = false);
        IObservable<Unit> UpdatePlayerPosition(string roomName, string roomId, float positionX, float positionY);
        IObservable<Unit> UpdatePlayerDestination(string roomName, string roomId, float positionX, float positionY);
        IObservable<Unit> UpdatePlayerState(string roomName, string roomId, string state);
        IObservable<Unit> LeaveChatRoom(string roomName, string roomId);

        IObservable<Unit> LeaveCurrentChatRoom();
    }
}
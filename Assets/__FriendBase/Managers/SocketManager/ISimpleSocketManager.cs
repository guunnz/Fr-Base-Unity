using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Socket
{
    public interface ISimpleSocketManager
    {
        void Connect();
        void Disconnect(bool hardDisconnect = false);
        //bool ReConnect();
        //ConnectionState GetConnectionState();
        bool Suscribe(string eventId, Action<AbstractIncomingSocketEvent> suscriber);
        bool Unsuscribe(string eventId, Action<AbstractIncomingSocketEvent> suscriber);
        void DeliverSocketEventToSuscribers(AbstractIncomingSocketEvent trama);
        void JoinChatRoom(string roomName, string roomId, float positionX, float positionY);
        void LeaveChatRoom(string roomName, string roomId);
    }
}


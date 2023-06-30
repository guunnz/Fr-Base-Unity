using UnityEngine;
using System;

namespace Multiuser
{
    public interface IMultiuser
    {
        void Send(AbstractTrama trama);
        void Connect(ConnectMultiuserParams parameters, UnityEngine.Object objectData);
        bool Disconnect();
        bool ReConnect();
        ConnectionState GetConnectionState();
        void SetConnectionState(ConnectionState connectionState);
        bool Suscribe(Action<AbstractIncomingTrama> suscriber, string[] tramaIds);
        bool Suscribe(Action<AbstractIncomingTrama> suscriber, string tramaId);
        bool Unsuscribe(Action<AbstractIncomingTrama> suscriber, string[] tramaIds);
        bool Unsuscribe(Action<AbstractIncomingTrama> suscriber, string tramaId);
        void DeliverTramaToSuscribers(AbstractIncomingTrama trama);
        bool SuscribeSendTrama(Action<AbstractTrama> suscriber, string tramaId); 
        bool UnsuscribeSendTrama(Action<AbstractTrama> suscriber, string tramaId);
        void DeliverSendTramaToSuscribers(AbstractTrama trama);
        void DeliverTriggerTramaToSuscribers(AbstractIncomingTrama trama);
        IObservable<AbstractIncomingTrama> OnTrama(string tramaId);
    }
}


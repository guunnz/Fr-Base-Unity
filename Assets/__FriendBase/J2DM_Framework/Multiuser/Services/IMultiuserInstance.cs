using UnityEngine;

namespace Multiuser
{
    public interface IMultiuserInstance
    {
        void Send(AbstractTrama trama);
        void Connect(ConnectMultiuserParams parameters, Object objectData);
        bool Disconnect();
        bool ReConnect();
        ConnectionState GetConnectionState();
        void SetConnectionState(ConnectionState connectionState);
    }
}
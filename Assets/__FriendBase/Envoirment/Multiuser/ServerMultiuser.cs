using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiuser;
using Architecture.Injector.Core;
using DebugConsole;

public class ServerMultiuser : IMultiuserInstance
{
    private ConnectionState stateConnection;

    public ServerMultiuser()
    {
        stateConnection = ConnectionState.NONE;
    }

    public void Send(AbstractTrama trama)
    {
        if (trama == null)
        {
            return;
        }
        if (Injection.Get<IDebugConsole>().isLogTypeEnable(LOG_TYPE.TRAMA_OUT)) Injection.Get<IDebugConsole>().TraceLog(LOG_TYPE.TRAMA_OUT, trama.TramaID);

        trama.Send();
    }

    public void Connect(ConnectMultiuserParams parameters, Object objectData)
    {
        //Here we initialize the socket/callbacks with the server...

        stateConnection = ConnectionState.CONNECTED;
    }

    public bool Disconnect()
    {
        //Implement disconnect
        stateConnection = ConnectionState.CONNECTION_LOST;
        return true;
    }

    public bool ReConnect()
    {
        //Implement Reconnection
        stateConnection = ConnectionState.CONNECTION_LOST;
        return true;
    }

    public ConnectionState GetConnectionState()
    {
        return stateConnection;
    }

    public void SetConnectionState(ConnectionState connectionState)
    {
        stateConnection = connectionState;
    }
}

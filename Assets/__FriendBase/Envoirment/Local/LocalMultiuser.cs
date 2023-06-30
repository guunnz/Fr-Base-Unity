
using UnityEngine;
using Multiuser;
using Architecture.Injector.Core;
using Data.Users;

public class LocalMultiuser : IMultiuserInstance
{
    private ConnectionState stateConnection;

    public LocalMultiuser()
    {
        Debug.Log("------ CREATE LocalMultiuser");
        stateConnection = ConnectionState.NONE;
    }

    public void Send(AbstractTrama trama)
    {
        if (trama == null)
        {
            return;
        }
        switch (trama.TramaID)
        {
            case IDTramas.USER_LOGIN:
                //UserInformation userInformation = new UserInformation("0", "Matias Ini", 100, 100, false, false, new AvatarCustomizationData());
                //Injection.Get<IMultiuser>().DeliverTramaToSuscribers(new IncomingUserLogin(TramaResult.OPERATION_SUCCEED, userInformation));
                break;
        }
    }

    public void Connect(ConnectMultiuserParams parameters, Object objectData)
    {
    }

    public bool Disconnect()
    {
        return true;
    }

    public bool ReConnect()
    {
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


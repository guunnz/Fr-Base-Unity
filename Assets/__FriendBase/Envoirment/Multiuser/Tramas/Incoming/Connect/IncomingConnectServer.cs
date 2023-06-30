using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiuser;

public class IncomingConnectServer : AbstractServerIncomingTrama
{
    public override string TramaID => IDTramas.CONNECT_SERVER;

    public IncomingConnectServer(int state)
    {
        State = state;
    }

    public override void Serialize()
    {

    }
}

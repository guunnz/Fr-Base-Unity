using Data;
using Socket;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameMultiplayerManager : MonoBehaviour
{
    internal IGameData gameData;

    internal string matchId;

    public virtual void FromMultiplayerInvite(AbstractIncomingSocketEvent incomingEvent)
    {

    }
}

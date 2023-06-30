using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Snapshots;
using Socket;
using UnityEngine;

public class AutomaticAvatarSnapshots : MonoBehaviour
{
    [SerializeField] private AvatarCustomizationController avatarPrefab;
    [SerializeField] private GameObject container;

    private RoomSnapshotAvatarManager roomSnapshotAvatarManager;
    private ISnapshot snapshotManager;

    void Start()
    {
        snapshotManager = Injection.Get<ISnapshot>();
        RoomJoinManager.OnRoomReady += OnRoomReady;
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.USER_LEAVE, OnUserLeaveMessage);
        SimpleSocketManager.Instance.Suscribe(SocketEventTypes.USER_ENTER, OnUserEnteressage);
    }

    void OnRoomReady()
    {
        RoomJoinManager.OnRoomReady -= OnRoomReady;
        roomSnapshotAvatarManager = CurrentRoom.Instance.RoomSnapshotAvatarManager;

        CreateSnapshotsAvatarsInRoom();
    }

    private void OnDestroy()
    {
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.USER_LEAVE, OnUserLeaveMessage);
        SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.USER_ENTER, OnUserEnteressage);
    }

    void CreateSnapshotsAvatarsInRoom()
    {
        List<AvatarRoomController> listAvatars = CurrentRoom.Instance.AvatarsManager.GetListAvatars();
        int amount = listAvatars.Count;
        for (int i=0; i<amount; i++)
        {
            new AutomaticSnapshotUnit(this, container, roomSnapshotAvatarManager, avatarPrefab, listAvatars[i].AvatarData);
        }
    }

    void Update()
    {
        
    }

    private void OnUserLeaveMessage(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventUserLeave incomingEventUserLeave = incomingEvent as IncomingEventUserLeave;
        if (incomingEventUserLeave != null && incomingEventUserLeave.State == SocketEventResult.OPERATION_SUCCEED)
        {
            //We do not remove the snapshot becasue we need them to show public chats although the user has left
            //roomSnapshotAvatarManager.RemoveSnapshot(incomingEventUserLeave.UserId);
        }
    }

    private void OnUserEnteressage(AbstractIncomingSocketEvent incomingEvent)
    {
        IncomingEventUserEnter incomingEventUserEnter = incomingEvent as IncomingEventUserEnter;
        if (incomingEventUserEnter != null && incomingEventUserEnter.State == SocketEventResult.OPERATION_SUCCEED)
        {
            new AutomaticSnapshotUnit(this, container, roomSnapshotAvatarManager, avatarPrefab, incomingEventUserEnter.AvatarData);
        }
    }
}

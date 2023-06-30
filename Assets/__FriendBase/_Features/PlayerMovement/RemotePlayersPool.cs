using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Architecture.Injector.Core;
using AuthFlow.Firebase.Core.Actions;
using MemoryStorage.Core;
using Newtonsoft.Json.Linq;
using PlayerMovement.Socket;
using PlayerRoom.View;
using Socket;
using UniRx;
using UnityEngine;

namespace PlayerMovement
{
    public class RemotePlayersPool : RoomViewComponent
    {
        static RemotePlayersPool current;
        public static RemotePlayersPool Current => current ? current : current = FindObjectOfType<RemotePlayersPool>();
        
        

        //prefab of the remote player view
        public RemotePlayer prefab;

        //a pool of availables remote player instances (those are not shown) ready to be used
        readonly Queue<RemotePlayer> pool = new Queue<RemotePlayer>();

        //the current displayed remote users hashed by it's id
        readonly Dictionary<string, RemotePlayer> onUsage = new Dictionary<string, RemotePlayer>();

        //local player ID
        string playerId;


        readonly SocketListener socketListener = new SocketListener();

        IMemoryStorage memoryStorage;

        string lastRoomName;
        string lastRoomId;
        string CurrentRoomName => memoryStorage.Get("currentRoomName");

        string CurrentRoomId => memoryStorage.Get("currentRoomId");

        readonly CompositeDisposable disposables = new CompositeDisposable();


        IEnumerator Start()
        {
            //first, retrieves the player id
            yield return Injection.Get<GetFirebaseUid>().Execute().Do(id => playerId = id).ToYieldInstruction();


            memoryStorage = Injection.Get<IMemoryStorage>();

            // waits until player connects into a chat room
            while (string.IsNullOrEmpty(CurrentRoomName) || string.IsNullOrEmpty(CurrentRoomId))
            {
                yield return null;
            }

            lastRoomName = CurrentRoomName;
            lastRoomId = CurrentRoomId;

            //if an user joins the room then update his info
            socketListener.OnUserJoinsRoom().Do(OnUpdate).Subscribe().AddTo(disposables);
            //if an user leave the room then remove it
            socketListener.OnUserLeavesRoom().Do(OnLeave).Subscribe().AddTo(disposables);
            //if an users info changes then update it
            socketListener.OnInfoChanges().Do(OnUpdate).Subscribe().AddTo(disposables);
            //check if the current user leaves the room and goes same other place on application to dispose the pool
            Observable.EveryUpdate().Do(CheckChangeRoom).Subscribe().AddTo(disposables);
        }

        void OnDisable()
        {
            ClearRemotesPool();
        }

        void OnEnable()
        {
            ClearRemotesPool();
        }


        void CheckChangeRoom()
        {
            //if not on the last known room
            if (lastRoomName != CurrentRoomName || lastRoomId != CurrentRoomId)
            {
                //remove the remote players
                ClearRemotesPool();
                //set the new one as the last known room
                lastRoomName = CurrentRoomName;
                lastRoomId = CurrentRoomId;
            }
        }

        public void ClearRemotesPool()
        {
            //don't perform it if there is no remote players
            if (onUsage.Keys.Count < 0)
            {
                return;
            }

            //copy into a list to avoid edit iterated collection
            var allRemotePLayerIds = onUsage.Keys.ToList();

            foreach (var remotePLayerID in allRemotePLayerIds)
            {
                //if there is a remote player active for the given ID
                if (onUsage.TryGetValue(remotePLayerID, out var player))
                {
                    //then remove it from the cache
                    onUsage.Remove(remotePLayerID);
                    //and destroy the gameobject
                    Destroy(player.gameObject);
                }
            }
        }

        void OnDestroy()
        {
            disposables.Clear();
        }


        void OnLeave(string id)
        {
            //if there is no player active for the given ID, then just print a warning
            if (!onUsage.TryGetValue(id, out var player))
            {
                Debug.LogWarning($"leave room event for unknown player id: {id}");
                return;
            }

            //--given a player active for the id--

            //remove it from cache
            onUsage.Remove(id);
            //clear the instance data
            player.remotePlayerId = "";


            const int maxAvailable = 20;
            if (pool.Count < maxAvailable)
            {
                //if the pool has room for more objects
                //just add it and deactivate the object
                pool.Enqueue(player);
                player.gameObject.SetActive(false);
            }
            else
            {
                //if the pool is full just destroy the object
                Destroy(player.gameObject);
            }
        }

        void OnUpdate(UserInfoDTO info)
        {
            //first of all check if the player belongs to the same room

            if (info.roomId != CurrentRoomId || info.roomName != CurrentRoomName)
            {
                //if the player info doesn't belong to the same room

                if (info.user_firebase_uid == playerId)
                {
                    //if the event corresponds to the local player ID 
                    //then clear everything cause the player moves to another room and it could be 
                    //an outdated message
                    ClearRemotesPool();
                }

                if (onUsage.TryGetValue(info.user_firebase_uid, out var player))
                {
                    //if the event corresponds to a remote player
                    //then remove it from cache and destroy his object
                    onUsage.Remove(info.user_firebase_uid);
                    Destroy(player.gameObject);
                }

                return;
            }

            if (info.user_firebase_uid != playerId)
            {
                //by pooling retrieves an instance for the player with the id (creates or retrieves)
                var remotePlayer = GetPlayer(info.user_firebase_uid, out var isNew);
                //update info for the remote player
                remotePlayer.UpdateInfo(new RemotePlayer.Info
                {
                    userFirebaseUid = info.user_firebase_uid,
                    username = info.username,
                    position = new Vector2(info.position_x, info.position_y),
                    destination = new Vector2(info.destination_x, info.destination_y),
                    customizationData = info.customizationInfo
                }, isNew);
            }
        }

        public RemotePlayer GetOnUsedPlayer(string remotePLayerID)
        {
            if (onUsage.TryGetValue(remotePLayerID, out var player))
            {
                return player;
            }

            return null;
        }


        RemotePlayer GetPlayer(string id, out bool isNew)
        {
            // a flag to identify if an instance represents a new remote
            // player or just will retrieve
            // an existing one

            if (onUsage.TryGetValue(id, out var player))
            {
                isNew = false;
                return player;
            }

            isNew = true;

            // if there is not available pooled
            // remote player instance, create some
            // and add to the pool

            if (pool.Count == 0)
            {
                for (var i = 0; i < 10; i++)
                {
                    var newPlayer = Instantiate(prefab, transform);
                    newPlayer.gameObject.SetActive(false);
                    pool.Enqueue(newPlayer);
                }
            }

            // then dequeue a player from the pool
            // and initialize it to be used
            player = pool.Dequeue();
            player.gameObject.SetActive(true);
            player.remotePlayerId = id;
            onUsage[id] = player;
            return player;
        }

        protected override void DidLoadRoom()
        {
            ClearRemotesPool();
        }
    }
}
using System;
using System.Collections;
using Architecture.Injector.Core;
using MemoryStorage.Core;
using Socket;
using UniRx;
using UnityEngine;

namespace PlayerMovement
{
    public class LocalPlayerSocketManager : MonoBehaviour
    {
        public Transform position;
        public Transform destination;

        ISocketManager socketManager;
        IMemoryStorage memoryStorage;

        
        const double DestinationMaxDistance = 0.0001;
        const double PositionMaxDistance = 1;
        const double TimeMaxDistance = 5;


        string CurrentRoomName
        {
            get => memoryStorage.Get("currentRoomName");
            set => memoryStorage.Set("currentRoomName", value);
        }

        string CurrentRoomId
        {
            get => memoryStorage.Get("currentRoomId");
            set => memoryStorage.Set("currentRoomId", value);
        }


        void Start()
        {
            Injection.Get(out socketManager);
            Injection.Get(out memoryStorage);
            StartCoroutine(UpdateSocket());
        }


        IEnumerator UpdateSocket()
        {
            //for lyfe-time cycle
            yield return null;
            yield return null;
            
            while (string.IsNullOrEmpty(CurrentRoomName) || string.IsNullOrEmpty(CurrentRoomId))
            {
                yield return null;
            }

            while (gameObject)
            {
                var pos = position.position;
                yield return socketManager.UpdatePlayerPosition(CurrentRoomName, CurrentRoomId, pos.x, pos.y).First()
                    .ToYieldInstruction();
                var des = destination.position;
                yield return socketManager.UpdatePlayerDestination(CurrentRoomName, CurrentRoomId, des.x, des.y).First()
                    .ToYieldInstruction();

                var lastPosition = pos;
                var lastDestination = des;
                var lastUpdate = Time.time;

                /*
                 * wait until any of this conditions
                 * player position changes enough
                 * player destination changes enough
                 * TimeMaxDistance seconds passes
                 */
                while (
                       Vector2.Distance(position.position, lastPosition) < PositionMaxDistance &&
                       Vector2.Distance(destination.position, lastDestination) < DestinationMaxDistance &&
                       Mathf.Abs(lastUpdate - Time.time) < TimeMaxDistance
                       )
                {
                    yield return null;
                }

                Debug.Log($"<color=blue> sending position {pos} </color>");
            }
        }
    }
}


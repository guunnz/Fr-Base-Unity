using System;
using System.Collections;
using Managers.Avatar;
using Newtonsoft.Json.Linq;
using Pathfinding;
using PlayerRoom.View;
using UnityEngine;

namespace PlayerMovement
{
    public class RemotePlayer : MonoBehaviour
    {
        public Vector3 destination;
        //public Transform position;
        public Transform playerFlip;
        public Transform destinationTransform;

        public AvatarCustomizationController customization;

        AvatarAnimationController animationsCache;

        AvatarNotificationController notificationsCache;

        // ReSharper disable once NotAccessedField.Global
        public string remotePlayerId;
        public string username;
        public float maxSpeed = 2;
        [SerializeField] Seeker seeker;
        [SerializeField] AIPath aiPath;

        private Chair destinationChair;

        private bool lastUpdateForce;

        private Vector2 lastCalculatedPos;

        private bool Sitting;
        private bool roomHasChairs;

        private Vector2 sittingPos;

        string lastCustomizationData;

        public AvatarAnimationController Animations => animationsCache
            ? animationsCache
            : animationsCache = gameObject.GetComponentInChildren<AvatarAnimationController>();

        public AvatarNotificationController Notifications => notificationsCache
            ? notificationsCache
            : notificationsCache = gameObject.GetComponentInChildren<AvatarNotificationController>();

        IEnumerator Start()
        {
            aiPath = GetComponent<AIPath>();
            //wait a frame to let customization be initialized
            yield return null;
            //just to handle the case, you don't have a datamodel loaded from customization
            //don't look the ugly prefab without any modification
            if (string.IsNullOrEmpty(lastCustomizationData))
            {
                customization.SetAvatarCustomizationData(AvatarCustomizationSimpleData.Default);
            }


        }

        void OnDisable()
        {
            remotePlayerId = string.Empty;
            username = string.Empty;
            lastCustomizationData = string.Empty;
        }

        public void UpdateInfo(Info info, bool force = false)
        {
            var pos = info.position;
            //setup the info
            remotePlayerId = info.userFirebaseUid;
            username = info.username;
            destination = info.destination;

            lastCalculatedPos = pos;
            //when a player first appears
            //just setup the destination and the position
            //so it spawns on the point and don't need to move there
            Vector3 spawnPos = SpawnPointManager.Singleton.GetSpawnPoint().position;
            if (force)
            {
                seeker.transform.position = destination;
                lastUpdateForce = true;
            }
            else if (lastUpdateForce) //this is used because spawn point destination gets received on the second update call
            {
                bool isOnSpawn = destination == spawnPos;
                if (isOnSpawn)
                {
                    seeker.transform.position = destination;
                }
            }

            SetupCustomization(info);
        }

        void SetupCustomization(Info info)
        {
            //don't parse empty data
            if (string.IsNullOrEmpty(info.customizationData)) return;
            //don't parse same data again
            if (info.customizationData == lastCustomizationData) return;
            //cache the current customization data
            lastCustomizationData = info.customizationData;
            //then parse the info and setup to the avatar
            customization.SetAvatarCustomizationDataFromJoinRoom(JObject.Parse(info.customizationData));
        }

        void Update()
        {
            UpdateMovement(Time.deltaTime);
            UpdateAnimation();
        }

        void UpdateMovement(float deltaTime)
        {
            if (new Vector3(lastCalculatedPos.x, lastCalculatedPos.y, 0) == GetChairSitPoint() && !destinationChair.IsOccupied())
            {
                return;
            }
            //just a seek behaviour to the destination
            //todo: use Seeker class for it (so it uses navigation mesh)
            //position.position = Vector2.MoveTowards(position.position, destination, deltaTime * maxSpeed);
            destinationTransform.position = destination;
            //seeker.transform.position = destination;
        }
        Vector3 GetChairSitPoint()
        {
            if (destinationChair == null)
            {
                destinationChair = ChairsManager.Singleton.GetClosestChair(destination);
            }
            var buttOffset = Animations.ButtOffset;
            if (destinationChair.transform.lossyScale.x > 0)
            {
                buttOffset.x = -Mathf.Abs(buttOffset.x);
            }
            else
            {
                buttOffset.x = +Mathf.Abs(buttOffset.x);
            }

            return destinationChair.GetSitpoint() - buttOffset;
        }
        void UpdateAnimation()
        {
            destinationChair = ChairsManager.Singleton.GetClosestChair(destination);
            if (!destinationChair.IsOccupied())
            {


                if (new Vector3(lastCalculatedPos.x, lastCalculatedPos.y, 0) == GetChairSitPoint() && Sitting)
                {
                    return;
                }
                else if (Sitting)
                {
                    aiPath.canMove = true;
                    Sitting = false;
                    //destinationChair.SetChairOccupied(false);
                }

                if (destinationChair != null && !Sitting)
                {
                    Sitting = new Vector3(lastCalculatedPos.x, lastCalculatedPos.y, 0) == GetChairSitPoint() /*&& (new Vector2(aiPath.position.x, aiPath.position.y) != lastCalculatedPos && aiPath.reachedEndOfPath == true*/ && !destinationChair.IsOccupied();

                    if (Sitting)
                    {
                        float distanceFromChair = Vector2.Distance(new Vector2(aiPath.position.x, aiPath.position.y), destinationChair.GetSitpoint());
                        //Debug.LogError(distanceFromChair);
                        aiPath.canMove = false;
                        playerFlip.localScale = new Vector3(destinationChair.transform.lossyScale.x, 1, 1);
                        //destinationChair.SetChairOccupied(true);
                        this.transform.position = GetChairSitPoint();
                        Animations.SetSeatState();
                    }
                }
            }
            Vector2 vel = aiPath.desiredVelocity;
            {
                if (vel.magnitude > 0.01 && !Sitting)
                {
                    //if the destination is away from the position
                    //then the character must be walking there
                    aiPath.canMove = true;
                    Animations.SetWalkState();
                    //aiPath.canMove = true;
                    if (Mathf.Abs(vel.x) > 0.2f)
                    {
                        playerFlip.localScale = new Vector3(Mathf.Sign(vel.x), 1, 1);
                    }
                }
                else
                {
                    if (!Sitting)
                    {
                        Animations.SetIdleState();
                    }
                }
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(destination, seeker.transform.position);
        }

#endif

        public struct Info
        {
            public string userFirebaseUid;
            public string username;
            public Vector2 position;
            public Vector2 destination;
            public string customizationData;
            //todo: add state
        }
    }
}
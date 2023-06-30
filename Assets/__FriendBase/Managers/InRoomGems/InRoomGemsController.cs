using System.Collections;
using Architecture.Injector.Core;
using Data;
using Managers.Avatar;
using Managers.InRoomGems.Infrastructure;
using Socket;
using UnityEngine;

namespace Managers.InRoomGems
{
    public class InRoomGemsController : MonoBehaviour
    {
        [SerializeField] private GameObject gem;
        [SerializeField] private GemsAnimationController gemsAnimationController;
        private Audio.AudioModule AM;

        private string gemToken;
        private InRoomGemsSFXController sfxController;
        private string localFirebaseId;

        private bool canPick;

        private const float GemSpawnTime = 30f;

        Coroutine endGemCoroutine;

        private void Start()
        {
            sfxController = GetComponent<InRoomGemsSFXController>();
            AM = FindObjectOfType<Audio.AudioModule>();
            localFirebaseId = Injection.Get<IGameData>().GetUserInformation().FirebaseId;
        }

        public void Init()
        {
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.NEW_GEM, OnNewGem);
            SimpleSocketManager.Instance.Suscribe(SocketEventTypes.GEM_PICKED, OnRemoteUserClaimedGem);
        }

        private void OnNewGem(AbstractIncomingSocketEvent incomingEvent)
        {
            IncomingEventNewGem incomingEventNewGem = incomingEvent as IncomingEventNewGem;

            if (incomingEventNewGem != null &&
                incomingEventNewGem.State == SocketEventResult.OPERATION_SUCCEED)
            {
                SetNewGem(CurrentRoom.Instance.CurrentRoomManager.GemPointsContainer.GetRandomGemPoint(),
                    incomingEventNewGem.Token);
            }
        }

        void OnRemoteUserClaimedGem(AbstractIncomingSocketEvent incomingEvent)
        {
            IncomingEventUserClaimedGem incomingEventUserClaimedGem = incomingEvent as IncomingEventUserClaimedGem;

            if (incomingEventUserClaimedGem != null &&
                incomingEventUserClaimedGem.State == SocketEventResult.OPERATION_SUCCEED)
            {
                if (!localFirebaseId.Equals(incomingEventUserClaimedGem.RewardedUserFirebaseId))
                {
                    CurrentRoom.Instance.AvatarsManager
                        .GetAvatarById(incomingEventUserClaimedGem.RewardedUserFirebaseId).AvatarNotificationController
                        .PlayNotification(Notification.NotificationType.Gem);
                }
            }
        }

        void SetNewGem(Vector3 position, string token)
        {
            if (!CurrentRoom.Instance.IsPublicRoom()) return;

            sfxController.PlayNewGem();
            gemToken = token;
            gem.transform.position = position;
            gem.SetActive(true);
            canPick = true;
            gemsAnimationController.PlaysNewGemSequence();

            ProgramEndGemState();
        }

        void ProgramEndGemState()
        {
            if (endGemCoroutine != null)
            {
                StopCoroutine(endGemCoroutine);
            }

            endGemCoroutine = StartCoroutine(EndGem());
        }

        IEnumerator EndGem()
        {
            yield return new WaitForSeconds(GemSpawnTime - gemsAnimationController.GetEndAnimationTime());

            canPick = false;
            gemsAnimationController.PlayGemEnd();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(Tags.PlayerFeet) || !canPick) return;
            var avatar = collision.GetComponentInParent<AvatarRoomController>();
            if (!avatar.IsLocalPlayer()) return;

            StopCoroutine(EndGem());
            RewardGem();
        }

        private async void RewardGem()
        {
            var task = InRoomGemsWebClient.RewardGems(gemToken);
            gemToken = null;
            await task;

            if (!task.Result.Item1) return;

            StartCoroutine(gemsAnimationController.PickGem(gem));

            var avatarRoomController = CurrentRoom.Instance.AvatarsManager.GetMyAvatar();
            if (avatarRoomController == null) return;

            avatarRoomController.AvatarNotificationController.PlayNotification(Notification.NotificationType.Gem);
            Injection.Get<IGameData>().GetUserInformation().SetGems(task.Result.Item2);

            sfxController.PlayPickGem();
            gem.gameObject.SetActive(false);
        }


        private void OnDestroy()
        {
            SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.NEW_GEM, OnNewGem);
            SimpleSocketManager.Instance.Unsuscribe(SocketEventTypes.GEM_PICKED, OnRemoteUserClaimedGem);
        }
    }
}
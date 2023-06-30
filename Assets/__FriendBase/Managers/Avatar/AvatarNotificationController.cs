using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using Architecture.Injector.Core;
using Data;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers.Avatar
{
    [System.Serializable]
    public class NotificationPosition
    {
        public Transform transform;
        public bool isTaken = false;
    }


    [System.Serializable]
    public class Notification
    {
        public enum NotificationType
        {
            Dialogue,
            NewFriend,
            Gem
        }

        public NotificationType type;
        public Transform transform;
        public int priority;
        public float notificationDuration;
        public int currentPos = -1;

        public void EnableGameObject()
        {
            transform.gameObject.SetActive(true);
        }

        public void DisableGameObject()
        {
            transform.gameObject.SetActive(false);
        }

        public Vector3 GetLocalPosition()
        {
            return transform.localPosition;
        }

        public void SetLocalPosition(Vector3 newLocalPosition)
        {
            transform.localPosition = newLocalPosition;
        }
    }

    [System.Serializable]
    public class DialogueSprite
    {
        public Sprite spriteForSize;
        public float sizeRequired;
        public bool isPrivateChat = false;
    }

    public class AvatarNotificationController : MonoBehaviour
    {
        private Vector3 diamondPos;
        [SerializeField] private GameObject greenBubblePrivateChat;
        [SerializeField] private GameObject whiteBubblePrivateChat;
        [SerializeField] private Transform basePosition;
        [SerializeField] private Transform midPosition;
        [SerializeField] private Transform topPosition;

        [SerializeField] private List<Notification> notificationsList;

        [SerializeField]
        private List<NotificationPosition> notificationsPositionsList = new List<NotificationPosition>();

        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private TMP_Text dialogueTextPrivate;
        [SerializeField] private List<DialogueSprite> dialogueSpriteList = new List<DialogueSprite>();
        [SerializeField] private SpriteRenderer dialogueSpriteRenderer;
        [SerializeField] private AvatarRoomController avatarRoomController;
        private ChatManager cm;

        private IGameData gameData;


        private void Start()
        {
            gameData = Injection.Get<IGameData>();
            StartCoroutine(SuscribeToDialogue());
        }

        IEnumerator SuscribeToDialogue()
        {
            while (!CurrentRoom.Instance.IsReady)
            {
                yield return null;
            }
            
            while (CurrentRoom.Instance == null  || cm == null || !this.gameObject.activeSelf)
            {
                cm = CurrentRoom.Instance.chatManager;
                yield return null;
            } 
            
            

            cm.OnNewPublicChat += PlayPublicDialogue;
            cm.OnNewPrivateChat += PlayPrivateDialogue;
        }

        private void OnDestroy()
        {
            cm.OnNewPublicChat -= PlayPublicDialogue;
            cm.OnNewPrivateChat -= PlayPrivateDialogue;
        }

        private Sprite GetDialogueSpriteBasedOnSize(string textWanted, bool privateChat = false)
        {
            float totalWidth = 0f;
            string text = textWanted;

            if (privateChat)
            {
                dialogueTextPrivate.text = textWanted;
            }
            else
            {
                dialogueText.text = textWanted;
            }

            string textBuilt = "";

            List<DialogueSprite> spriteList = dialogueSpriteList.Where(x => x.isPrivateChat == privateChat).ToList();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                TMP_Character character = null;
                textBuilt += text[i];
                // Use TryAddCharacters to make sure the character exists in the font asset
                if (!dialogueText.font.TryAddCharacters(c.ToString()))
                {
                    // Find the character in the character table
                    foreach (var charItem in dialogueText.font.characterTable)
                    {
                        if (charItem.unicode == c)
                        {
                            character = charItem;
                            break;
                        }
                    }

                    if (character != null)
                    {
                        totalWidth += character.glyph.metrics.horizontalAdvance;
                    }

                    if (totalWidth > spriteList[spriteList.Count - 1].sizeRequired)
                    {
                        if (privateChat)
                        {
                            dialogueTextPrivate.text = textBuilt + "...";
                        }
                        else
                        {
                            dialogueText.text = textBuilt + "...";
                        }

                        break;
                    }
                }
            }

            if (totalWidth > spriteList[spriteList.Count - 2].sizeRequired)
            {
                StartCoroutine(SetPositionsForBigText());
            }

            if (privateChat)
            {
                dialogueTextPrivate.enabled = true;
                dialogueText.enabled = false;
            }
            else
            {
                dialogueTextPrivate.enabled = false;
                dialogueText.enabled = true;
            }

            foreach (var dialogueSprite in spriteList.OrderBy(x => x.sizeRequired).ToList())
            {
                if (totalWidth < dialogueSprite.sizeRequired)
                {
                    return dialogueSprite.spriteForSize;
                }
            }


            return spriteList.Last().spriteForSize;
        }


        private IEnumerator SetPositionsForBigText()
        {
            List<Vector3> extraPositions =
                notificationsPositionsList.GetRange(1, notificationsPositionsList.Count - 1)
                    .Select(x => x.transform.localPosition).ToList();

            foreach (var notifPosition in notificationsPositionsList.GetRange(1, notificationsPositionsList.Count - 1))
            {
                var localPosition = notifPosition.transform.localPosition;
                localPosition = new Vector3(localPosition.x, localPosition.y + 0.25f,
                    localPosition.z);
                notifPosition.transform.localPosition = localPosition;
            }

            yield return new WaitForSeconds(notificationsList
                .Single(x => x.type == Notification.NotificationType.Dialogue).notificationDuration);

            int index = 0;

            foreach (var notifPosition in notificationsPositionsList.GetRange(1, notificationsPositionsList.Count - 1))
            {
                var localPosition = notifPosition.transform.localPosition;

                localPosition = extraPositions[index];
                notifPosition.transform.localPosition = localPosition;
                index++;
            }
        }

        private Vector3 GetNotificationLocalPosition(int maxPosition)
        {
            int index = 0;
            foreach (var notification in notificationsPositionsList)
            {
                if (notification.isTaken)
                {
                    for (int i = maxPosition; i < notificationsPositionsList.Count; i++)
                    {
                        if (!notificationsPositionsList[i].isTaken)
                        {
                            notificationsPositionsList[i].isTaken = true;
                            return notificationsPositionsList[i].transform.localPosition;
                        }
                    }
                }
                else
                {
                    notificationsPositionsList[maxPosition].isTaken = true;
                    return notificationsPositionsList[maxPosition].transform.localPosition;
                }
            }


            throw new Exception(
                "Add a new position to the notification system, or remember to free the notification after using it");
        }


        private void FreeNotificationPosition(int notificationPos)
        {
            notificationsPositionsList[notificationPos].isTaken = false;
        }

        int GetNextAvailablePosition()
        {
            for (int i = 0; i < notificationsPositionsList.Count; i++)
            {
                if (!notificationsPositionsList[i].isTaken)
                {
                    return i;
                }
            }

            return -1;
        }

        private void PlayPrivateDialogue(string idUser, string channelUserId, UserChatData userChatData)
        {
            if (idUser == avatarRoomController.AvatarData.UserId)
            {
                PlayDialogue(userChatData.ChatText, true);
            }
        }

        private void PlayPublicDialogue(UserChatData userChatData)
        {
            if (userChatData.AvatarData.UserId == avatarRoomController.AvatarData.UserId)
            {
                PlayDialogue(userChatData.ChatText);
            }
        }

        public void PlayDialogue(string dialogue, bool isPrivateChat = false)
        {
            dialogueSpriteRenderer.color = new Color(1, 1, 1, 0);
            dialogueText.color = new Color(31 / 255f, 41 / 255f, 55 / 255f, 0);
            dialogueSpriteRenderer.DOColor(Color.white, 0.6f);
            dialogueText.DOColor(new Color(31 / 255f, 41 / 255f, 55 / 255f, 1), 0.6f);
            dialogueSpriteRenderer.sprite = GetDialogueSpriteBasedOnSize(dialogue, isPrivateChat);
            PlayNotification(Notification.NotificationType.Dialogue);
        }

        public void PlayNotification(Notification.NotificationType notifType)
        {
            StartCoroutine(PlayNotificationCoroutine(notificationsList.Single(x => x.type == notifType)));
        }

        private int FindNotificationPosition(Notification notification)
        {
            for (int i = 0; i < notificationsPositionsList.Count; i++)
            {
                Notification existingNotification = notificationsList.FirstOrDefault(n =>
                    n.transform.localPosition == notificationsPositionsList[i].transform.localPosition);
                // If this position is not taken or can be occupied by higher priority notification, return this position
                if (!notificationsPositionsList[i].isTaken || (existingNotification != null &&
                                                               existingNotification.priority > notification.priority))
                {
                    return i;
                }
            }

            return -1;
        }

        private int GetNextFreePosition(int priority)
        {
            for (int i = 0; i < notificationsPositionsList.Count; i++)
            {
                if (!notificationsPositionsList[i].isTaken)
                {
                    return i;
                }

                if (i == priority)
                {
                    MoveNotification(notificationsList[i]);
                    return priority;
                }
            }

            return -1;
        }

        IEnumerator PlayNotificationCoroutine(Notification notification)
        {
            if (notificationsList.Single(x => x.type == notification.type).currentPos > -1)
                yield break;

            for (int i = 0; i < notificationsPositionsList.Count; i++)
            {
                Notification existingNotification = notificationsList.FirstOrDefault(n =>
                    n.currentPos == i);


                // If the position is not taken, place the notification there and break the loop
                if (!notificationsPositionsList[i].isTaken)
                {
                    notification.transform.localPosition = notificationsPositionsList[i].transform.localPosition;

                    if (notification.type == Notification.NotificationType.Dialogue)
                    {
                        notification.transform.localPosition = new Vector3(notification.transform.localPosition.x,
                            notification.transform.localPosition.y - 0.4f, notification.transform.localPosition.z);

                        notification.transform.DOLocalMove(notificationsPositionsList[i].transform.localPosition, 0.6f);
                    }

                    notification.EnableGameObject();
                    notificationsPositionsList[i].isTaken = true;
                    notification.currentPos = i;
                    yield return new WaitForSeconds(notification.notificationDuration);

                    FreeNotificationPosition(notification.currentPos);
                    notification.DisableGameObject();
                    notification.currentPos = -1;
                    var position = notification.transform.position;

                    position = new Vector3(position.x, 0,
                        position.z);

                    notification.transform.position = position;
                    break;
                }
                // If the position is taken and the priority of the notification matches the position, displace the existing notification
                else if (i == notification.priority || notification.priority < existingNotification.priority)
                {
                    // Find a new position for the existing notification
                    int newPos = GetNextFreePosition(existingNotification.priority);

                    // If there's no free position, display a warning and break the coroutine
                    if (newPos == -1)
                    {
                        Debug.LogWarning(
                            "No free positions found to move the existing notification. Notification queue might be full.");
                        yield break;
                    }

                    // Move the existing notification to the new position
                    existingNotification.transform.localPosition =
                        notificationsPositionsList[newPos].transform.localPosition;
                    notificationsPositionsList[newPos].isTaken = true;
                    existingNotification.currentPos = newPos;

                    notificationsPositionsList[i].isTaken = false; // Set the old position to not taken

                    // Place the incoming notification
                    notification.transform.localPosition = notificationsPositionsList[i].transform.localPosition;
                    if (notification.type == Notification.NotificationType.Dialogue)
                    {
                        notification.transform.localPosition = new Vector3(notification.transform.localPosition.x,
                            notification.transform.localPosition.y - 0.4f, notification.transform.localPosition.z);

                        notification.transform.DOLocalMove(notificationsPositionsList[i].transform.localPosition, 0.6f);
                    }

                    notification.EnableGameObject();
                    notification.currentPos = i;
                    notificationsPositionsList[i].isTaken = true;

                    yield return new WaitForSeconds(notification.notificationDuration);
                    if (notification.type == Notification.NotificationType.Dialogue)
                    {
                        dialogueText.DOColor(new Color(31 / 255f, 41 / 255f, 55 / 255f, 0), 0.6f);
                        dialogueSpriteRenderer.DOColor(new Color(1, 1, 1, 0), 0.6f);
                        yield return new WaitForSeconds(0.6f);
                    }

                    FreeNotificationPosition(notification.currentPos);
                    notification.currentPos = -1;
                    notification.DisableGameObject();

                    break;
                }
            }
        }


        void MoveNotification(Notification notification)
        {
            for (int i = 0; i < notificationsPositionsList.Count; i++)
            {
                Notification existingNotification = notificationsList.FirstOrDefault(n =>
                    n.currentPos == i);


                // If the position is not taken, place the notification there and break the loop
                if (!notificationsPositionsList[i].isTaken)
                {
                    notification.transform.localPosition = notificationsPositionsList[i].transform.localPosition;
                    notification.EnableGameObject();
                    notificationsPositionsList[i].isTaken = true;
                    notification.currentPos = i;
                    break;
                }
                // If the position is taken and the priority of the notification matches the position, displace the existing notification
                else if (i == notification.priority || notification.priority < existingNotification.priority)
                {
                    // Find a new position for the existing notification
                    int newPos = GetNextFreePosition(existingNotification.priority);

                    // If there's no free position, display a warning and break the coroutine
                    if (newPos == -1)
                    {
                        Debug.LogWarning(
                            "No free positions found to move the existing notification. Notification queue might be full.");
                        return;
                    }

                    // Move the existing notification to the new position
                    existingNotification.transform.localPosition =
                        notificationsPositionsList[newPos].transform.localPosition;
                    notificationsPositionsList[newPos].isTaken = true;
                    existingNotification.currentPos = newPos;

                    notificationsPositionsList[i].isTaken = false; // Set the old position to not taken

                    // Place the incoming notification
                    notification.transform.localPosition = notificationsPositionsList[i].transform.localPosition;
                    notification.EnableGameObject();
                    notification.currentPos = i;
                    notificationsPositionsList[i].isTaken = true;
                    break;
                }
            }
        }


        private void OnEnable()
        {
            notificationsPositionsList.ForEach(x => x.isTaken = false);
            notificationsList.ForEach(x => x.DisableGameObject());
        }

        //-----------------------------------------------
        //----------------- PRIVATE CHAT ----------------
        //-----------------------------------------------

        public void ShowWhitePrivateChat()
        {
            // isPrivateChatEnable = true;
            // greenBubblePrivateChat.SetActive(false);
            // whiteBubblePrivateChat.SetActive(true);
            // PrivateChatCheck();
        }

        public void ShowGreenPrivateChat()
        {
            // isPrivateChatEnable = true;
            // greenBubblePrivateChat.SetActive(true);
            // whiteBubblePrivateChat.SetActive(false);
            // PrivateChatCheck();
        }

        public void HidePrivateChat()
        {
            // isPrivateChatEnable = false;
            // greenBubblePrivateChat.SetActive(false);
            // whiteBubblePrivateChat.SetActive(false);
        }

        private void PrivateChatCheck()
        {
        }

        //------------------------------------------------------
        //----------------- FRIEND NOTIFICATION ----------------
        //------------------------------------------------------


        //---------------------------------------------------

        public void SetOrientation(float orientation)
        {
            Vector3 scale = transform.localScale;

            scale.x = Mathf.Abs(scale.x) * orientation;

            transform.localScale = scale;
        }
    }
}
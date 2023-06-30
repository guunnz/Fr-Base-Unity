using Architecture.Injector.Core;
using LocalizationSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsView.View
{
    public class NotificationsAnimationsController : MonoBehaviour
    {
        [SerializeField] Animator request;
        [SerializeField] Animator friend;

        public Button requestButton;
        public Button friendButton;

        [SerializeField] private TextMeshProUGUI newRequesterText;
        [SerializeField] private TextMeshProUGUI newFriendText;

        private const string AnimatorSlideState = "Slide";
        private const string NewRequestMessage = "wants to be your friend";
        private const string NewFriendMessage = "is your friend!";
        private ILanguage language;
        bool isRunningNotifications;
        readonly Queue<(string, bool)> notificationsNames = new Queue<(string, bool)>();

        private void Start()
        {
            language = Injection.Get<ILanguage>();
            requestButton.gameObject.SetActive(false);
            friendButton.gameObject.SetActive(false);

            requestButton.onClick.AddListener(() => requestButton.gameObject.SetActive(false));
            friendButton.onClick.AddListener(() => friendButton.gameObject.SetActive(false));
        }

        private void OnEnable()
        {
            StartNotifications();
        }

        private void OnDisable()
        {
            PauseNotifications();
        }

        public void PlayNotification(string username, bool isRequest)
        {
            notificationsNames.Enqueue((username, isRequest));
            if (!isRunningNotifications) StartCoroutine(CoroutineDispatchNotification());
        }

        void PauseNotifications()
        {
            requestButton.gameObject.SetActive(false);
            friendButton.gameObject.SetActive(false);
            StopCoroutine(CoroutineDispatchNotification());
            isRunningNotifications = false;
        }

        void StartNotifications()
        {
            if (!isRunningNotifications) StartCoroutine(CoroutineDispatchNotification());
        }

        IEnumerator CoroutineDispatchNotification()
        {
            isRunningNotifications = true;

            while (notificationsNames.Count > 0)
            {
                var data = notificationsNames.Dequeue();
                float seconds;

                if (data.Item2)
                {
                    language.SetText(newRequesterText, string.Format(language.GetTextByKey(LangKeys.FRIEND_NAME_WANTS_TO_BE_YOUR_FRIEND), data.Item1));

                    requestButton.gameObject.SetActive(true);

                    request.Play(AnimatorSlideState, 0, 0);
                    seconds = request.GetCurrentAnimatorStateInfo(0).length;
                }
                else
                {
                    language.SetText(newFriendText, string.Format(language.GetTextByKey(LangKeys.FRIEND_NAME_IS_YOUR_FRIEND), data.Item1));

                    friendButton.gameObject.SetActive(true);

                    friend.Play(AnimatorSlideState, 0, 0);
                    seconds = friend.GetCurrentAnimatorStateInfo(0).length;
                }

                yield return new WaitForSeconds(seconds);
            }

            isRunningNotifications = false;
        }
    }
}
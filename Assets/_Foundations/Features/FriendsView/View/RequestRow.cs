using Architecture.Injector.Core;
using FriendsView.Core.Domain;
using LocalizationSystem;
using Snapshots;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsView.View
{
    public class RequestRow : MonoBehaviour
    {
        FriendRequestData data;

        public Button acceptButton;
        public Button rejectButton;
        public Button rowButton;

        public GameObject requestedState;
        public GameObject acceptedState;

        public TextMeshProUGUI usernameText;
        public TextMeshProUGUI FRIEND_NAME_IS_YOUR_FRIEND_NOW;

        public SnapshotAvatar snapshotAvatar;
        private ILanguage language;
        public void SetFriendRequestRow(FriendRequestData data)
        {
            this.data = data;
            usernameText.SetText(data.username);
            snapshotAvatar.CreateSnaphot(null, data.avatarCustomizationData);
            SetLanguageKeys();
        }

        public void SetLanguageKeys()
        {
            language = Injection.Get<ILanguage>();
            FRIEND_NAME_IS_YOUR_FRIEND_NOW.text = string.Format(language.GetTextByKey(LangKeys.FRIEND_NAME_IS_YOUR_FRIEND_NOW), usernameText.text);
            //Number of friends //Number of friends Not Text
        }

        public void AcceptRequest()
        {
            acceptedState.SetActive(true);
            requestedState.SetActive(false);
        }

        public void RejectRequest()
        {
            gameObject.SetActive(false);
        }

        public void ResetRow()
        {
            acceptedState.SetActive(false);
            requestedState.SetActive(true);
        }


        public FriendRequestData GetFriendRequestData()
        {
            return data;
        }
    }
}
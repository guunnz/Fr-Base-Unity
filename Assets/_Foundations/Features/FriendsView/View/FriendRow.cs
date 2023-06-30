using Architecture.Injector.Core;
using FriendsView.Core.Domain;
using LocalizationSystem;
using Snapshots;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsView.View
{
    public class FriendRow : MonoBehaviour
    {
        public FriendData data;
        public Button friendCardButton;
        public Button rowButton;
        public Button visitFriendButton;

        public TextMeshProUGUI usernameText;
        public TextMeshProUGUI connectionStatusText;

        public Image connectionStatusImage;
        public SnapshotAvatar snapshotAvatar;

        public Sprite onlineStatusImage;

        public Sprite offlineStatusImage;
        public Sprite offlineVisitButton;
        public Sprite onlineVisitButton;
        private ILanguage language;


        private void Start()
        {
            language = Injection.Get<ILanguage>();
        }

        public void SetFriendRow(FriendData friendData)
        {
            data = friendData;
            language = Injection.Get<ILanguage>();
            usernameText.SetText(data.username);
            if (data.IsInPublicRoom)
            {
                language.SetTextByKey(connectionStatusText, LangKeys.FRIEND_PUBLIC_AREA);//ChangeLookText
                //connectionStatusText.SetText("IN PUBLIC AREA");
                connectionStatusImage.sprite = onlineStatusImage;
                visitFriendButton.image.sprite = onlineVisitButton;
            }
            else
            {
                language.SetTextByKey(connectionStatusText, LangKeys.FRIEND_PRIVATE_AREA);//ChangeLookText
                //connectionStatusText.SetText("IN PRIVATE AREA");
                connectionStatusImage.sprite = offlineStatusImage;
                visitFriendButton.image.sprite = offlineVisitButton;
            }

            snapshotAvatar.CreateSnaphot(null, friendData.avatarCustomizationData);
        }


        public FriendData GetFriendRowData()
        {
            return data;
        }
    }
}
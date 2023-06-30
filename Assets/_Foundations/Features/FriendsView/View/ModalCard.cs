using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using FriendsView.Core.Domain;
using LocalizationSystem;
using Snapshots;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsView.View
{
    public class ModalCard : MonoBehaviour
    {
        const int maxRequestShown = 99;

        public List<Button> reportButtons;
        public GameObject personalSubCard;
        public TextMeshProUGUI realName;
        public TextMeshProUGUI username;
        public TextMeshProUGUI gems;
        public TextMeshProUGUI coins;
        public Button toFriendsButton;
        public Image AddFriendIcon;
        public SnapshotAvatar snapshotAvatar;
        [SerializeField] GameObject requestCountGO;
        [SerializeField] TextMeshProUGUI requestCount;
        [SerializeField] TextMeshProUGUI friendsCount;
        [SerializeField] TextMeshProUGUI addFriendText;
        [SerializeField] TextMeshProUGUI registerText;
        private ILanguage language;
        public Button visitFriendButton;
        public Button privateChatButton;
        public Button strangerPrivateChatButton;
        public Button unfriendButton;
        public GameObject friendSubCard;
        public GameObject online;
        public GameObject offline;

        public ChatView.View.ChatView ChatView;
        public Button acceptRequestButton;
        public Button rejectRequestButton;
        public Button addFriendButton;
        public Button pendingButton;
        public Button petsButton;
        public GameObject requestSubCard;
        public GameObject requestFooter;
        public GameObject strangerFooter;
        public GameObject requestOnline;
        public GameObject requestOffline;

        public Sprite visitFriendOnline;
        public Sprite visitFriendOffline;
        public Image PetProfileImage;
        public Load2DObject PetProfileLoadImageFromAddressables;
        public IGameData gameData;
        public string LastPlayerUsername;

        public GameObject GuestObject;
        public GameObject NonGuestObject;
        public GameObject GuestOther;

        void Awake()
        {
            language = Injection.Get<ILanguage>();
            gameData = Injection.Get<IGameData>();
        }

        private void Start()
        {
            registerText.text = language.GetTextByKey(LangKeys.NAUTH_REGISTER);
        }

        private void OnEnable()
        {
            gameData = Injection.Get<IGameData>();
        }

        private void OnDisable()
        {
            GuestObject.SetActive(false);
            addFriendButton.interactable = true;
            reportButtons.ForEach(x => x.interactable = true);
            privateChatButton.interactable = true;
            strangerPrivateChatButton.interactable = true;
            addFriendText.color = new Color(addFriendText.color.r, addFriendText.color.g, addFriendText.color.b, 1f);
            username.text = "";
        }

        public void SetPersonalCard(UserData userData)
        {
            gameData = Injection.Get<IGameData>();
            if (gameData.IsGuest())
            {
                GuestBehaviour();
            }
            else
            {
                username.text = userData.username;
                NonGuestBehaviour();
            }

            language = Injection.Get<ILanguage>();
            HideSubCars();
            realName.text = userData.realName;

            gems.text = userData.gems.ToString();
            coins.text = userData.gold.ToString();
            requestCountGO.SetActive(userData.friendRequestsCount > 0);
            toFriendsButton.gameObject.SetActive(userData.friendRequestsCount > 0 || userData.friendCount > 0);
            requestCount.SetText(userData.friendRequestsCount >= maxRequestShown
                ? "+" + maxRequestShown
                : userData.friendRequestsCount.ToString());

            if (userData != null && language != null)
                language.SetText(friendsCount, string.Format(language.GetTextByKey(LangKeys.PLAYER_X_FRIENDS), userData.friendCount));//Number of friends //Number of friends Not Text
            personalSubCard.SetActive(true);
            PetProfileImage.sprite = null;
            gameObject.SetActive(true);
            SetPersonalCardPet();
        }

        public void SetPersonalCardLoading()
        {
            HideSubCars();
            realName.text = language.GetTextByKey(LangKeys.Loading) + "...";
            username.text = language.GetTextByKey(LangKeys.Loading) + "...";
            gems.text = "----";
            coins.text = "----";
            language.SetText(friendsCount, string.Format(language.GetTextByKey(LangKeys.PLAYER_X_FRIENDS), "-"));//Number of friends //Number of friends Not Text

            personalSubCard.SetActive(true);
        }

        public void GuestBehaviour()
        {
            username.text = language.GetTextByKey(LangKeys.GUEST_USER);
            AddFriendIcon.color = new Color(AddFriendIcon.color.r, AddFriendIcon.color.g, AddFriendIcon.color.b, 0.5f);
            GuestOther.SetActive(false);
            friendsCount.color = new Color(friendsCount.color.r, friendsCount.color.g, friendsCount.color.b, 0.5f);
            GuestObject.SetActive(true);
            NonGuestObject.SetActive(false);
            toFriendsButton.interactable = false;
        }

        public void SeeingGuest()
        {
            AddFriendIcon.color = new Color(AddFriendIcon.color.r, AddFriendIcon.color.g, AddFriendIcon.color.b, 0.5f);
            reportButtons.ForEach(x => x.interactable = false);
            addFriendButton.interactable = false;
            privateChatButton.interactable = false;
            strangerPrivateChatButton.interactable = false;
            addFriendText.color = new Color(addFriendText.color.r, addFriendText.color.g, addFriendText.color.b, 0.2f);
            username.text = language.GetTextByKey(LangKeys.GUEST_USER);
        }

        public void NonGuestBehaviour()
        {
            AddFriendIcon.color = new Color(AddFriendIcon.color.r, AddFriendIcon.color.g, AddFriendIcon.color.b, 1f);
            toFriendsButton.interactable = true;
            GuestOther.SetActive(false);
            friendsCount.color = new Color(friendsCount.color.r, friendsCount.color.g, friendsCount.color.b, 1f);
            GuestObject.SetActive(false);
            NonGuestObject.SetActive(true);
        }

        public void SeeingNonGuestAsGuest()
        {
            reportButtons.ForEach(x => x.interactable = false);
            addFriendButton.interactable = false;
            privateChatButton.interactable = false;
            strangerPrivateChatButton.interactable = false;
            AddFriendIcon.color = new Color(AddFriendIcon.color.r, AddFriendIcon.color.g, AddFriendIcon.color.b, 0.5f);
            toFriendsButton.interactable = false;
            GuestOther.SetActive(false);
            friendsCount.color = new Color(friendsCount.color.r, friendsCount.color.g, friendsCount.color.b, 0.5f);
            GuestObject.SetActive(false);
            NonGuestObject.SetActive(true);
        }

        public void SetFriendCardLoading(string username)
        {
            if (language == null)
                language = Injection.Get<ILanguage>();
            HideSubCars();
            this.username.text = username;
            realName.text = language.GetTextByKey(LangKeys.Loading) + "...";
            friendsCount.SetText("");

            //friendSubCard.SetActive(true);
        }

        public void SetFriendCard(FriendData friendData)
        {
            HideSubCars();
            gameData = Injection.Get<IGameData>();
            if (gameData.IsGuest())
            {
                GuestOther.SetActive(true);
                return;
            }
            else
            {
                NonGuestBehaviour();
            }
            username.text = friendData.username;
            realName.text = friendData.realName;
            LastPlayerUsername = friendData.username;
            ChatView.SetPrivateChatUser(LastPlayerUsername, friendData.userID);
            language.SetText(friendsCount, string.Format(language.GetTextByKey(LangKeys.PLAYER_X_FRIENDS), friendData.friendCount));//Number of friends //Number of friends Not Text

            if (friendData.IsInPublicRoom)
            {
                online.SetActive(true);
                offline.SetActive(false);
            }
            else
            {
                visitFriendButton.image.sprite = visitFriendOffline;

                online.SetActive(false);
                offline.SetActive(true);
            }

            if (CurrentRoom.Instance.AvatarsManager.GetAvatarById(friendData.fireBaseUID) != null)
            {
                privateChatButton.gameObject.SetActive(true);
                visitFriendButton.gameObject.SetActive(false);
            }
            else
            {
                privateChatButton.gameObject.SetActive(false);
                visitFriendButton.gameObject.SetActive(true);
                visitFriendButton.image.sprite = visitFriendOnline;
            }
            PetProfileImage.sprite = null;
            friendSubCard.SetActive(true);
            SetFriendCardPet(friendData);
        }

        void SetFriendCardPet(FriendData friendData)
        {
            var friendAvatarRoomController = CurrentRoom.Instance.AvatarsManager.GetAvatarById(friendData.fireBaseUID);
            if (friendAvatarRoomController != null)
            {
                if (!string.IsNullOrEmpty(friendAvatarRoomController.avatarPetManager.PetName))
                {
                    PetProfileImage.enabled = true;
                    PetProfileLoadImageFromAddressables.Load(friendAvatarRoomController.avatarPetManager.PetName + "_Profile");
                }
                else
                {
                    PetProfileImage.enabled = false;
                }
            }
        }

        void SetStrangerCardPet(UserData userData)
        {
            var strangerAvatarRoomController = CurrentRoom.Instance.AvatarsManager.GetAvatarById(userData.firebaseUid);
            if (!string.IsNullOrEmpty(strangerAvatarRoomController.avatarPetManager.PetName))
            {
                PetProfileImage.enabled = true;
                PetProfileLoadImageFromAddressables.Load(strangerAvatarRoomController.avatarPetManager.PetName + "_Profile");
            }
            else
            {
                PetProfileImage.enabled = false;
            }
        }

        void SetPersonalCardPet()
        {
            gameData = Injection.Get<IGameData>();
            AvatarRoomController myAvatarRoomController = CurrentRoom.Instance.AvatarsManager.GetMyAvatar();

            if (!string.IsNullOrEmpty(myAvatarRoomController.avatarPetManager.PetName))
            {
                PetProfileImage.enabled = true;

                PetProfileLoadImageFromAddressables.Load(myAvatarRoomController.avatarPetManager.PetName + "_Profile");
            }
            else
            {
                PetProfileImage.enabled = false;
            }
        }

        public void SetReqCard(FriendRequestData userData)
        {
            HideSubCars();
            username.text = userData.username;
            realName.text = userData.username;
            language.SetText(friendsCount, string.Format(language.GetTextByKey(LangKeys.PLAYER_X_FRIENDS), userData.friendCount));//Number of friends //Number of friends Not Text



            if (true)
            {
                visitFriendButton.image.sprite = visitFriendOnline;
                requestOnline.SetActive(true);
                requestOffline.SetActive(false);
            }
            else
            {
                visitFriendButton.image.sprite = visitFriendOffline;

                requestOnline.SetActive(false);
                requestOffline.SetActive(true);
            }

            requestSubCard.SetActive(true);
            requestFooter.SetActive(true);
        }
        public void LinkProvider()
        {
            CurrentRoom.Instance.LinkProvider();
        }
        public void SetStrangerCard(UserData userData, bool requested)
        {
            gameData = Injection.Get<IGameData>();

            bool imGuest = gameData.IsGuest();
            bool strangerIsGuest = CurrentRoom.Instance.IsGuest(userData.firebaseUid);
            if (strangerIsGuest)
            {
                username.text = language.GetTextByKey(LangKeys.GUEST_USER);
                realName.text = "";
                if (imGuest)
                    GuestBehaviour();
                else
                    NonGuestBehaviour();
            }
            else
            {
                if (imGuest)
                {
                    SeeingNonGuestAsGuest();
                }
                else
                {
                    NonGuestBehaviour();
                }
                username.text = userData.username;
                realName.text = userData.username;
                LastPlayerUsername = userData.username;
            }

            ChatView.SetPrivateChatUser(LastPlayerUsername, userData.userID);
            language.SetText(friendsCount, string.Format(language.GetTextByKey(LangKeys.PLAYER_X_FRIENDS), userData.friendCount));//Number of friends //Number of friends Not Text


            if (true)
            {
                if (CurrentRoom.Instance.AvatarsManager.GetAvatarByUserId(userData.userID.ToString()) != null)
                {
                    privateChatButton.gameObject.SetActive(true);
                }
                else
                {
                    privateChatButton.gameObject.SetActive(false);
                }
                visitFriendButton.image.sprite = visitFriendOnline;
                requestOnline.SetActive(true);
                requestOffline.SetActive(false);
            }
            else
            {
                requestOnline.SetActive(false);
                requestOffline.SetActive(true);
            }

            if (requested)
            {
                pendingButton.gameObject.SetActive(true);
                requestSubCard.SetActive(true);
                strangerFooter.SetActive(true);
            }
            else
            {
                addFriendButton.gameObject.SetActive(true);
                requestSubCard.SetActive(true);
                strangerFooter.SetActive(true);
            }

            PetProfileImage.sprite = null;
            gameObject.SetActive(true);

            if (strangerIsGuest)
            {
                SeeingGuest();
            }

            SetStrangerCardPet(userData);
        }

        public void SendRequest()
        {
            addFriendButton.gameObject.SetActive(false);
            pendingButton.gameObject.SetActive(true);
        }

        void HideSubCars()
        {
            personalSubCard.SetActive(false);
            friendSubCard.SetActive(false);
            requestSubCard.SetActive(false);
            requestFooter.SetActive(false);
            strangerFooter.SetActive(false);
            addFriendButton.gameObject.SetActive(false);
            pendingButton.gameObject.SetActive(false);
        }
    }
}

//Todo: add online && in public area check to allow visiting friend
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Snapshots;
using Data;
using Architecture.Injector.Core;
using DG.Tweening;
using Data.Users;
using LocalizationSystem;

namespace Onboarding
{
    public class OnboardingProfileCardManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI TxtUserName;
        [SerializeField] private TextMeshProUGUI TxtName;
        [SerializeField] private TextMeshProUGUI TxtNumberOfFriends;
        [SerializeField] private TextMeshProUGUI TxtGemsAmount;
        [SerializeField] private TextMeshProUGUI TxtGoldAmount;
        [SerializeField] private SnapshotAvatar SnapshotAvatar;
        [SerializeField] private GameObject PersonalSubcard;
        [SerializeField] private GameObject FriendSubcard;
        [SerializeField] private GameObject CloseContainer;
        [SerializeField] private GameObject Hand;
        [SerializeField] private TextMeshProUGUI TxtClose;
        [SerializeField] private GameObject imgPet;
        [SerializeField] private TextMeshProUGUI TxtChangeLook;
        [SerializeField] private TextMeshProUGUI TxtOnline;
        [SerializeField] private GameObject PrivateChatContainer;
        [SerializeField] private TextMeshProUGUI TxtPrivateChat;

        private IGameData gameData;
        private ILanguage language;
        private void OnDisable()
        {
            Hand.SetActive(false);
        }
        public void ShowMyProfile()
        {
            this.gameObject.SetActive(true);

            gameData = Injection.Get<IGameData>();
            language = Injection.Get<ILanguage>();

            language.SetText(TxtUserName, gameData.GetUserInformation().UserName);
            language.SetText(TxtName, gameData.GetUserInformation().UserName);
            string amountFriends = language.GetTextByKey(LangKeys.PLAYER_X_FRIENDS);
            language.SetText(TxtNumberOfFriends, string.Format(amountFriends, 0));
            language.SetText(TxtGemsAmount, gameData.GetUserInformation().Gems.ToString());
            language.SetText(TxtGoldAmount, gameData.GetUserInformation().Gold.ToString());
            language.SetTextByKey(TxtChangeLook, LangKeys.PLAYER_CHANGE_LOOK );

            SnapshotAvatar.CreateSnapshot();
            PersonalSubcard.SetActive(true);
            FriendSubcard.SetActive(false);

            imgPet.gameObject.SetActive(false);

            PrivateChatContainer.SetActive(false);
            CloseContainer.SetActive(true);
            language.SetTextByKey(TxtClose, LangKeys.CLOSE);
            CloseContainer.transform.localScale = new Vector3(0f, 0f, 0f);
            CloseContainer.gameObject.transform.DOScale(1, 0.3f).SetDelay(0.4f).SetEase(Ease.OutExpo);
        }

        public void ShowFriendProfile(AvatarCustomizationData avatarCustomizationData)
        {
            this.gameObject.SetActive(true);

            Hand.SetActive(true);
            gameData = Injection.Get<IGameData>();
            language = Injection.Get<ILanguage>();

            language.SetTextByKey(TxtUserName, LangKeys.AUTH_FRIENDBASE_LEGAL_NAME);
            language.SetTextByKey(TxtName, LangKeys.ONBOARDING_YOUR_FRIENDS_IN_FRIENDBASE);
            string amountFriends = language.GetTextByKey(LangKeys.PLAYER_X_FRIENDS);
            language.SetText(TxtNumberOfFriends, string.Format(amountFriends, 220));
            language.SetTextByKey(TxtOnline, LangKeys.PLAYER_ONLINE);
            
            SnapshotAvatar.CreateSnaphot(null, avatarCustomizationData);
            PersonalSubcard.SetActive(false);
            FriendSubcard.SetActive(true);

            imgPet.gameObject.SetActive(false);

            CloseContainer.SetActive(false);
            PrivateChatContainer.SetActive(true);
            language.SetTextByKey(TxtPrivateChat, LangKeys.ONBOARDING_START_PRIVATE_CHAT);
            PrivateChatContainer.transform.localScale = new Vector3(0f, 0f, 0f);
            PrivateChatContainer.gameObject.transform.DOScale(1, 0.3f).SetDelay(0.4f).SetEase(Ease.OutExpo);
        }

        public void Hide()
        {
            Hand.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}

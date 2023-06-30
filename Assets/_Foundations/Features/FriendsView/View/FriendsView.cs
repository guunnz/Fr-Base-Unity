using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.Injector.Core;
using Architecture.MVP;
using Data;
using FriendsView.Core.Domain;
using FriendsView.Presentation;
using LocalizationSystem;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsView.View
{
    public class FriendsView : WidgetBase, IFriendsView
    {
        public static event Action OnCustomization;
        [SerializeField] Button avatarButton;
        [SerializeField] Button customizationButton;

        [SerializeField] ModalCard card;

        [SerializeField] List<Button> closeButtons;

        [SerializeField] GameObject mainRequestNumberGO;
        [SerializeField] TextMeshProUGUI mainRequestNumber;

        [SerializeField] GameObject occluder;
        [SerializeField] ModalSocial socialList;
        [SerializeField] DialogsLayout dialogsLayout;

        public DialogsLayout DialogsLayout => dialogsLayout;
        public ModalCard Card => card;
        public ModalSocial SocialList => socialList;

        [SerializeField] private NotificationsAnimationsController notificationsAnimator;

        public IGameData gameData;
        public IObservable<Unit> OnAvatarButton => avatarButton.OnClickAsObservable();
        public IObservable<Unit> OnFriendsButton => card.toFriendsButton.OnClickAsObservable();
        public IObservable<Unit> OnVisitFriendsBtn => card.visitFriendButton.OnClickAsObservable();
        public IObservable<Unit> OnReqCardAcceptBtn => card.acceptRequestButton.OnClickAsObservable();
        public IObservable<Unit> OnReqCardRejectBtn => card.rejectRequestButton.OnClickAsObservable();
        public IObservable<Unit> OnAddFriendButton => card.addFriendButton.OnClickAsObservable();
        public IObservable<Unit> OnUnFriendsButton => card.unfriendButton.OnClickAsObservable();
        public IObservable<Unit> OnRequestButton => notificationsAnimator.requestButton.OnClickAsObservable();
        public IObservable<Unit> OnFriendButton => notificationsAnimator.friendButton.OnClickAsObservable();

        public IEnumerable<IObservable<Unit>> OnCloseButtons =>
            closeButtons.Select(button => button.OnClickAsObservable());

        public IEnumerable<IObservable<Unit>> OnReportButtons =>
            card.reportButtons.Select(button => button.OnClickAsObservable());

        readonly ISubject<Unit> onStart = new Subject<Unit>();
        public IObservable<Unit> OnStart => onStart;

        private List<GameObject> sections = new List<GameObject>();

        private ILanguage language;

        public TextMeshProUGUI PLAYER_CHANGE_LOOK;//ChangeLookText
        public TextMeshProUGUI PLAYER_FRIENDS;//FriendsText
        public TextMeshProUGUI PLAYER_YOU_CAN_ONLY_VISIT_FRIEND_IN_PUBLIC;//YouOnlyCanVisitText
        public TextMeshProUGUI PLAYER_OK;//OkText //OkText //OkText
        public TextMeshProUGUI PLAYER_OK1;//OkText //OkText //OkText
        public TextMeshProUGUI PLAYER_OK2;//OkText //OkText //OkText

        public TextMeshProUGUI PLAYER_NO_CANCEL;//No,CancelText
        public TextMeshProUGUI PLAYER_YES_UNFRIEND;//Yes, UnfriendText
        public TextMeshProUGUI PLAYER_REQUESTS;//RequestsText
        public TextMeshProUGUI PLAYER_YOU_DO_NOT_HAVE_ANY_REQUESTS;  //YouDon'tHaveFriendRequestText
        public TextMeshProUGUI PLAYER_WHEN_SOMEONE_ADDS_YOU_AS_FRIEND; //WhenSomeoneAddsText
        public TextMeshProUGUI FRIEND_NO_CANCEL;//FriendNoCancelText //No,CancelText
        public TextMeshProUGUI FRIEND_YES_ADD; //YesAddText
        public TextMeshProUGUI FRIEND_PENDING;//PendingText
        public TextMeshProUGUI FRIEND_REJECT;//RejectFriend
        public TextMeshProUGUI FRIEND_ACCEPT;//AcceptFriend;
        public TextMeshProUGUI REPORT_WELL_CHECK_IF_THEY_ARE_VIOLATING_RULES;//CheckViolatingRulesText
        public TextMeshProUGUI REPORT_DID_YOU_HAVE_A_BAD_EXPERIENCE; //BadExperienceText
        public TextMeshProUGUI REPORT_YOU_WONT_BE_ABLE_TO_CONTACT_EACH_OTHER;//wont able to contact
        public TextMeshProUGUI REPORT_NO_CANCEL;//No,CancelText 
        public TextMeshProUGUI REPORT_YES_BLOCK;//Yes,BlockText //YesBlockText
        public TextMeshProUGUI REPORT_YES_BLOCK2;//Yes,BlockText //YesBlockText
        public TextMeshProUGUI REPORT_IF_YOU_THINK_THEY_VIOLATED_RULES;//IfThinkViolatedText
        public TextMeshProUGUI REPORT_ALSO_REPORT;//AlsoReportText
        public TextMeshProUGUI REPORT_JUST_BLOCK;//JustBlockText
        public TextMeshProUGUI REPORT_HELP_US_UNDERSTAND_THE_PROBLEM;//HelpUsUnderstandText //HelpUsUnderstandText
        public TextMeshProUGUI REPORT_HELP_US_UNDERSTAND_THE_PROBLEM2;//HelpUsUnderstandText //HelpUsUnderstandText
        public TextMeshProUGUI REPORT_INAPPROPRIATE_CONTENT;//InappropiateText
        public TextMeshProUGUI REPORT_SPAM;//SpamText
        public TextMeshProUGUI REPORT_FAKE_ACCOUNT;//FakeAccountText
        public TextMeshProUGUI REPORT_ABUSIVE_BEHAVIOUR;//AbusiveBehaviourText
        public TextMeshProUGUI REPORT_FOUL_LANGUAGE;//FoulText
        public TextMeshProUGUI REPORT_OTHER;//OtherText
        public TextMeshProUGUI REPORT_THANKS_FOR_LETTING_US_KNOW;
        public TextMeshProUGUI REPORT_IF_WE_FIND_THE_USER_IS_VIOLATING;//IfWeFindUserViolatingText //IfWeFindViolatingText
        public TextMeshProUGUI REPORT_IF_WE_FIND_THE_USER_IS_VIOLATING2;//IfWeFindUserViolatingText //IfWeFindViolatingText
        public TextMeshProUGUI REPORT_DONE;//DoneText //DoneText
        public TextMeshProUGUI REPORT_DONE2;//DoneText //DoneText
        public TextMeshProUGUI REPORT_JUST_REPORT;//JustReportText
        public TextMeshProUGUI FRIEND_ADD_FRIEND; //AddFriendText


        private void Start()
        {
            gameData = Injection.Get<IGameData>();
            SetLanguageKeys();
            onStart.OnNext(Unit.Default);
            this.CreatePresenter<FriendsPresenter, IFriendsView>();
        }
        public void SetLanguageKeys()
        {
            //language.SetTextByKey(PLAYER_YESTERDAY, LangKeys.PLAYER_YESTERDAY);
            //language.SetTextByKey(PLAYER_ONLINE, LangKeys.PLAYER_ONLINE);//Online Text //OnlineNotFriend
            //language.SetTextByKey(PLAYER_OFFLINE, LangKeys.PLAYER_OFFLINE);//Offline Text //Offline TextNotFriend
            //language.SetTextByKey(FRIEND_NEW_FRIEND, LangKeys.FRIEND_NEW_FRIEND);
            language = Injection.Get<ILanguage>();

            language.SetTextByKey(PLAYER_CHANGE_LOOK, LangKeys.PLAYER_CHANGE_LOOK);//ChangeLookText
            language.SetTextByKey(PLAYER_FRIENDS, LangKeys.PLAYER_FRIENDS);//FriendsText
            language.SetTextByKey(PLAYER_YOU_CAN_ONLY_VISIT_FRIEND_IN_PUBLIC, LangKeys.PLAYER_YOU_CAN_ONLY_VISIT_FRIEND_IN_PUBLIC);//YouOnlyCanVisitText
            language.SetTextByKey(PLAYER_OK, LangKeys.PLAYER_OK);//OkText //OkText //OkText
            language.SetTextByKey(PLAYER_OK1, LangKeys.PLAYER_OK);//OkText //OkText //OkText
            language.SetTextByKey(PLAYER_OK2, LangKeys.PLAYER_OK);//OkText //OkText //OkText
            language.SetTextByKey(PLAYER_NO_CANCEL, LangKeys.PLAYER_NO_CANCEL);//OkText //OkText //OkText

            language.SetTextByKey(PLAYER_YES_UNFRIEND, LangKeys.PLAYER_YES_UNFRIEND);//Yes, UnfriendText
            language.SetTextByKey(PLAYER_REQUESTS, LangKeys.PLAYER_REQUESTS);//RequestsText
            language.SetTextByKey(PLAYER_YOU_DO_NOT_HAVE_ANY_REQUESTS, LangKeys.PLAYER_YOU_DO_NOT_HAVE_ANY_REQUESTS);  //YouDon'tHaveFriendRequestText
            language.SetTextByKey(PLAYER_WHEN_SOMEONE_ADDS_YOU_AS_FRIEND, LangKeys.PLAYER_WHEN_SOMEONE_ADDS_YOU_AS_FRIEND); //WhenSomeoneAddsText //YouDontHaveFriend
            language.SetTextByKey(FRIEND_NO_CANCEL, LangKeys.FRIEND_NO_CANCEL);//FriendNoCancelText //No,CancelText
            language.SetTextByKey(FRIEND_YES_ADD, LangKeys.FRIEND_YES_ADD); //YesAddText
            language.SetTextByKey(FRIEND_PENDING, LangKeys.FRIEND_PENDING);//PendingText
            language.SetTextByKey(FRIEND_REJECT, LangKeys.FRIEND_REJECT);//RejectFriend
            language.SetTextByKey(FRIEND_ACCEPT, LangKeys.FRIEND_ACCEPT);//AcceptFriend_each_other";//TextYouWontBeAbleToSeeText
            language.SetTextByKey(REPORT_WELL_CHECK_IF_THEY_ARE_VIOLATING_RULES, LangKeys.REPORT_WELL_CHECK_IF_THEY_ARE_VIOLATING_RULES); //BadExperienceText
            language.SetTextByKey(REPORT_DID_YOU_HAVE_A_BAD_EXPERIENCE, LangKeys.REPORT_DID_YOU_HAVE_A_BAD_EXPERIENCE); //BadExperienceText
            language.SetTextByKey(REPORT_YOU_WONT_BE_ABLE_TO_CONTACT_EACH_OTHER, LangKeys.REPORT_YOU_WONT_BE_ABLE_TO_CONTACT_EACH_OTHER);//CheckViolatingRulesText
            language.SetTextByKey(REPORT_NO_CANCEL, LangKeys.REPORT_NO_CANCEL);//No,CancelText 
            language.SetTextByKey(REPORT_YES_BLOCK, LangKeys.REPORT_YES_BLOCK);//Yes,BlockText //YesBlockText
            language.SetTextByKey(REPORT_YES_BLOCK2, LangKeys.REPORT_YES_BLOCK);//Yes,BlockText //YesBlockText
            language.SetTextByKey(REPORT_IF_YOU_THINK_THEY_VIOLATED_RULES, LangKeys.REPORT_IF_YOU_THINK_THEY_VIOLATED_RULES);//IfThinkViolatedText
            language.SetTextByKey(REPORT_ALSO_REPORT, LangKeys.REPORT_ALSO_REPORT);//AlsoReportText
            language.SetTextByKey(REPORT_JUST_BLOCK, LangKeys.REPORT_JUST_BLOCK);//JustBlockText
            language.SetTextByKey(REPORT_HELP_US_UNDERSTAND_THE_PROBLEM, LangKeys.REPORT_HELP_US_UNDERSTAND_THE_PROBLEM);//HelpUsUnderstandText //HelpUsUnderstandText
            language.SetTextByKey(REPORT_HELP_US_UNDERSTAND_THE_PROBLEM2, LangKeys.REPORT_HELP_US_UNDERSTAND_THE_PROBLEM);//HelpUsUnderstandText //HelpUsUnderstandText
            language.SetTextByKey(REPORT_INAPPROPRIATE_CONTENT, LangKeys.REPORT_INAPPROPRIATE_CONTENT);//InappropiateText
            language.SetTextByKey(REPORT_SPAM, LangKeys.REPORT_SPAM);//SpamText
            language.SetTextByKey(REPORT_FAKE_ACCOUNT, LangKeys.REPORT_FAKE_ACCOUNT);//FakeAccountText
            language.SetTextByKey(REPORT_ABUSIVE_BEHAVIOUR, LangKeys.REPORT_ABUSIVE_BEHAVIOUR);//AbusiveBehaviourText
            language.SetTextByKey(REPORT_FOUL_LANGUAGE, LangKeys.REPORT_FOUL_LANGUAGE);//FoulText
            language.SetTextByKey(REPORT_OTHER, LangKeys.REPORT_OTHER);//OtherText
            language.SetTextByKey(REPORT_THANKS_FOR_LETTING_US_KNOW, LangKeys.REPORT_THANKS_FOR_LETTING_US_KNOW);
            language.SetTextByKey(REPORT_IF_WE_FIND_THE_USER_IS_VIOLATING, LangKeys.REPORT_IF_WE_FIND_THE_USER_IS_VIOLATING);//IfWeFindUserViolatingText //IfWeFindViolatingText
            language.SetTextByKey(REPORT_IF_WE_FIND_THE_USER_IS_VIOLATING2, LangKeys.REPORT_IF_WE_FIND_THE_USER_IS_VIOLATING);//IfWeFindUserViolatingText //IfWeFindViolatingText
            language.SetTextByKey(REPORT_DONE, LangKeys.REPORT_DONE);//DoneText //DoneText
            language.SetTextByKey(REPORT_DONE2, LangKeys.REPORT_DONE);//DoneText //DoneText
            language.SetTextByKey(REPORT_JUST_REPORT, LangKeys.REPORT_JUST_REPORT);//JustReportText
            language.SetTextByKey(FRIEND_ADD_FRIEND, LangKeys.FRIEND_ADD_FRIEND); //AddFriendText
        }

        void OnEnable()
        {
            customizationButton.onClick.AddListener(OpenCustomization);
        }

        void OnDisable()
        {
            customizationButton.onClick.RemoveListener(OpenCustomization);
        }

        private void OpenCustomization()
        {
            OnCustomization?.Invoke();
            CurrentRoom.Instance.GoToAvatarCustomization();
        }

        public void ShowSection(ViewSection section)
        {
            HideSections();
            occluder.SetActive(true);

            dialogsLayout.ShowSection(section);

            switch (section)
            {
                case ViewSection.PersonalCard:
                    Card.gameObject.SetActive(true);
                    Card.personalSubCard.gameObject.SetActive(true);
                    break;
                case ViewSection.FriendListModal:
                    SocialList.gameObject.SetActive(true);
                    break;
                case ViewSection.FriendCard:
                    Card.gameObject.SetActive(true);
                    if (gameData.IsGuest())
                    {
                        return;
                    }
                    Card.friendSubCard.gameObject.SetActive(true);
                    break;
                case ViewSection.FriendReqCard:
                    Card.gameObject.SetActive(true);
                    if (gameData.IsGuest())
                    {
                        return;
                    }
                    Card.requestSubCard.gameObject.SetActive(true);
                    Card.requestFooter.gameObject.SetActive(true);
                    break;
                case ViewSection.StrangerCard:
                    Card.gameObject.SetActive(true);
                    Card.requestSubCard.gameObject.SetActive(true);
                    //Card.strangerFooter.gameObject.SetActive(true);
                    break;
            }
        }

        //Todo: can those be on an IEnumerable method?
        public void HideSections()
        {
            Card.gameObject.SetActive(false);
            Card.personalSubCard.gameObject.SetActive(false);
            Card.friendSubCard.gameObject.SetActive(false);
            SocialList.gameObject.SetActive(false);
            occluder.SetActive(false);
            DialogsLayout.HideSections();
            Card.requestSubCard.gameObject.SetActive(false);
            Card.requestFooter.gameObject.SetActive(false);
            Card.strangerFooter.gameObject.SetActive(false);
        }

        public void SetPersonalCard(UserData userData)
        {
            Card.SetPersonalCard(userData);
        }

        public void SetFriendCard(FriendData friendData)
        {
            Card.SetFriendCard(friendData);
        }

        public bool IsShowingFriendRows()
        {
            return socialList.IsShowingFriendRows();
        }

        public void SetRequestCard(FriendRequestData data)
        {
            Card.SetReqCard(data);
        }

        public void SetStrangerCard(UserData data, bool requested)
        {
            Card.SetStrangerCard(data, requested);
        }


        public void SetFriendModal(List<FriendData> friend)
        {
            if (friend.Count > 0)
            {
                SocialList.SetFriendRows(friend);
                socialList.SetFriendListButton(true);
                if (!IsShowingFriendRows())
                {
                    SwitchTabs();
                }
               
            }
            else
            {
                socialList.SetFriendListButton(false);
                if (IsShowingFriendRows())
                {
                    SwitchTabs();
                }
            }
        }

        public void SetFriendListButton(int friendCount)
        {
            if (friendCount > 0)
            {
                socialList.SetFriendListButton(true);
            }
            else
            {
                socialList.SetFriendListButton(false);
            }
        }

        public void SetFriendRequestModal(List<FriendRequestData> friendReq)
        {
            SocialList.SetRequestRows(friendReq);
        }

        public void SwitchTabs()
        {
            SocialList.SwitchTabs();
        }

        public void UpdateAvatarButtonRequestsNumber(int request)
        {
            if (request > 0)
            {
                mainRequestNumberGO.SetActive(true);
                mainRequestNumber.SetText(request.ToString());
            }
            else
            {
                mainRequestNumberGO.SetActive(false);
                mainRequestNumber.SetText("0");
            }
        }

        public void PlayNotification(string username, bool isRequest)
        {
            notificationsAnimator.PlayNotification(username, isRequest);
        }
    }
}
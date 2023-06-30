using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.Injector.Core;
using FriendsView.Core.Domain;
using LocalizationSystem;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsView.View
{
    public class DialogsLayout : MonoBehaviour
    {
        [SerializeField] Button selectBlockBtn;
        [SerializeField] Button selectReportBtn;
        [SerializeField] Button confirmBlockingBtn;
        [SerializeField] Button alsoReportBtn;
        [SerializeField] Button alsoBlockBtn;
        [SerializeField] Button justBlockBtn;
        [SerializeField] Button justReportBtn;
        [SerializeField] Button confirmAddButton;
        [SerializeField] Button otherReasonButton;

        [SerializeField] Transform startBox;
        [SerializeField] Transform blockBox;
        [SerializeField] Transform alsoOk;
        [SerializeField] Transform reasonBox;
        [SerializeField] Transform alsoReport;
        [SerializeField] Transform alsoBlock;
        [SerializeField] Transform done;
        [SerializeField] Transform okGO;
        [SerializeField] Transform unableVisitBox;
        [SerializeField] Transform confirmAddBox;
        [SerializeField] Transform OtherReasonBox;

        [SerializeField] private TMP_InputField otherReasonField;

        [SerializeField] UnfriendDialog unfriendDialog;

        [SerializeField] List<Button> reportReasonBtns;

        [SerializeField] TextMeshProUGUI DoYouWantToAdd;
        [SerializeField] TextMeshProUGUI BlockName;
        [SerializeField] TextMeshProUGUI YouBlockedAlsoReport;

        [SerializeField] TextMeshProUGUI YouBlockedAlsoOk;
        [SerializeField] TextMeshProUGUI WantToBlock;
        [SerializeField] TextMeshProUGUI IfWeFindViolatingText;
        [SerializeField] TextMeshProUGUI AlsoBlock;
        [SerializeField] TextMeshProUGUI ReportAccount;
        [SerializeField] TextMeshProUGUI NoLongerAble;

        [SerializeField] TextMeshProUGUI NoLongerFriendlist;
        [SerializeField] TextMeshProUGUI PLAYER_DO_YOU_WANT_TO_UNFRIEND;//WantToUnfriendText
        private ILanguage language;

        public IObservable<Unit> OnBlock => selectBlockBtn.OnClickAsObservable();
        public IObservable<Unit> OnReport => selectReportBtn.OnClickAsObservable();
        public IObservable<Unit> OnConfirmBlocking => confirmBlockingBtn.OnClickAsObservable();
        public IObservable<Unit> OnAlsoReport => alsoReportBtn.OnClickAsObservable();
        public IObservable<Unit> OnAlsoBlock => alsoBlockBtn.OnClickAsObservable();
        public IObservable<Unit> OnJustBlock => justBlockBtn.OnClickAsObservable();
        public IObservable<Unit> OnJustReport => justReportBtn.OnClickAsObservable();
        public IObservable<Unit> OnConfirmAdd => confirmAddButton.OnClickAsObservable();
        public IObservable<Unit> OtherReasonButton => otherReasonButton.OnClickAsObservable();


        public UnfriendDialog UnfriendDialog => unfriendDialog;

        public IEnumerable<IObservable<Unit>> ReportReasonBtns => reportReasonBtns
            .Select(b => b.OnClickAsObservable());

        public TMP_InputField OtherReasonField => otherReasonField;

        readonly List<string> startDialogsTexts = new List<string>();

        public void HideSections()
        {
            UnfriendDialog.gameObject.SetActive(false);
            okGO.gameObject.SetActive(false);
            unableVisitBox.gameObject.SetActive(false);
            startBox.gameObject.SetActive(false);
            blockBox.gameObject.SetActive(false);
            alsoReport.gameObject.SetActive(false);
            alsoOk.gameObject.SetActive(false);
            reasonBox.gameObject.SetActive(false);
            done.gameObject.SetActive(false);
            alsoBlock.gameObject.SetActive(false);
            confirmAddBox.gameObject.SetActive(false);
            OtherReasonBox.gameObject.SetActive(false);

        }

        public void ShowSection(ViewSection section)
        {
            switch (section)
            {
                case ViewSection.UnfriendModal:
                    UnfriendDialog.gameObject.SetActive(true);
                    break;
                case ViewSection.OkBox:
                    okGO.gameObject.SetActive(true);
                    break;
                case ViewSection.StartReportBox:
                    startBox.gameObject.SetActive(true);
                    break;
                case ViewSection.BlockBox:
                    blockBox.gameObject.SetActive(true);
                    break;
                case ViewSection.AlsoReportBox:
                    alsoReport.gameObject.SetActive(true);
                    break;
                case ViewSection.AlsoBlockBox:
                    alsoBlock.gameObject.SetActive(true);
                    break;
                case ViewSection.AlsoOkBox:
                    alsoOk.gameObject.SetActive(true);
                    break;
                case ViewSection.ReasonBox:
                    reasonBox.gameObject.SetActive(true);
                    break;
                case ViewSection.ReasonBoxFromStart:
                    reasonBox.gameObject.SetActive(true);
                    break;
                case ViewSection.DoneBox:
                    done.gameObject.SetActive(true);
                    break;
                case ViewSection.UnableVisitOkCard:
                    unableVisitBox.gameObject.SetActive(true);
                    break;
                case ViewSection.UnableVisitOkList:
                    unableVisitBox.gameObject.SetActive(true);
                    break;
                case ViewSection.ConfirmAddFriend:
                    confirmAddBox.gameObject.SetActive(true);
                    break;
                case ViewSection.OtherBox:
                    OtherReasonBox.gameObject.SetActive(true);
                    break;

            }
        }

        private void Start()
        {

        }

        public void SetUsernameFields(string username)
        {
            language = Injection.Get<ILanguage>();
            if (language != null)
            {
                string name = username;

                //language.SetText(DoYouWantToAdd, string.Format(language.GetTextByKey(LangKeys.FRIEND_DO_YOU_WANT_TO_ADD_NAME), username)); // Do you want to add //To your friends

                language.SetText(PLAYER_DO_YOU_WANT_TO_UNFRIEND, string.Format(language.GetTextByKey(LangKeys.PLAYER_DO_YOU_WANT_TO_UNFRIEND), username));
                language.SetText(BlockName, string.Format(language.GetTextByKey(LangKeys.REPORT_BLOCK_NAME), username));//Block text (no name)
                language.SetText(YouBlockedAlsoReport, string.Format(language.GetTextByKey(LangKeys.REPORT_YOU_BLOCKED_NAME), username));//YouBlockedText //YouBlockedText
                language.SetText(YouBlockedAlsoOk, string.Format(language.GetTextByKey(LangKeys.REPORT_YOU_BLOCKED_NAME), name));//YouBlockedText //YouBlockedText
                language.SetText(WantToBlock, string.Format(language.GetTextByKey(LangKeys.REPORT_DO_YOU_WANT_TO_BLOCK_NAME), username));//WantToBlockText (no name)
                language.SetTextByKey(IfWeFindViolatingText, LangKeys.REPORT_WELL_CHECK_IF_THEY_ARE_VIOLATING_RULES);
                language.SetText(AlsoBlock, string.Format(language.GetTextByKey(LangKeys.REPORT_WOULD_YOU_ALSO_LIKE_TO_BLOCK_NAME), username));//AlsoBlockText
                language.SetText(ReportAccount, string.Format(language.GetTextByKey(LangKeys.REPORT_REPORT_NAME), username)); //ReportAccountText
                //language.SetText(NoLongerAble, string.Format(language.GetTextByKey(LangKeys.REPORT_NAME_WILL_NO_LONGER_BE_ABLE_TO), username));//NoLongerAbleToText //SeeYouPublic,AddFriendText //BlockingWillUnfriendText //BlockingWillUnfriendText

                language.SetText(NoLongerFriendlist, string.Format(language.GetTextByKey(LangKeys.PLAYER_NAME_IS_NO_LONGER_FRIEND), "")); //NoLongerFriendlist
            }
        }

        public Dictionary<int, ReportReasons> ReportReason { get; }
            = new Dictionary<int, ReportReasons>()
            {
                {0, ReportReasons.inappropriate_content},
                {1, ReportReasons.spam},
                {2, ReportReasons.fake_account},
                {3, ReportReasons.abusive_behavior},
                {4, ReportReasons.foul_language},
                {5, ReportReasons.other}
            };

    }
}
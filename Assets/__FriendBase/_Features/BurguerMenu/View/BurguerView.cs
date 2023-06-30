using System;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Architecture.MVP;
using BurguerMenu.Core.Domain;
using BurguerMenu.Presentation;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UI;
using UI.EditAccount;
using TMPro;
using LocalizationSystem;
using System.Collections;
using Data;

namespace BurguerMenu.View
{
    public class BurguerView : WidgetBase, IBurguerView
    {
        static public string friendbaseFaqUrl = "https://friendbase.com/faq/";
        static public string termsAndConditionsUrl = "https://friendbase.com/terms-of-use-cookie-policy-app/";
        static public string codeOfConductUrl = "https://friendbase.com/code-of-conduct-app/";
        static public string privacyPolicyUrl = "https://friendbase.com/privacy-policy/";

        //WebViewObject webViewObject;

        public CanvasWebView webView;
        public GameObject webViewContainer;

        [SerializeField] GameObject screenBlocker;
        private Audio.AudioModule AM;


        [Header("Menu Buttons")]
        [Space(10)]
        [SerializeField]
        private Button helpButton;

        [SerializeField] private Button editAccountButton;
        [SerializeField] private Button registerButton;

        [SerializeField] Button logOutButton;

        [Header("Help Buttons")]
        [Space(10)]
        [SerializeField]
        Button supportButton;

        [SerializeField] Button termsButton;

        [SerializeField] Button codeOfConduct;

        [SerializeField] Button privacyPolicy;
        [SerializeField] Button logoutGuestModal;
        [SerializeField] Button logoutGuest;

        [SerializeField] Button closeWebViewButton;
        [SerializeField] GameObject AudioOnGraphic;
        [SerializeField] GameObject AudioOffGraphic;
        [SerializeField] TextMeshProUGUI SoundText;


        [Header("Sections")]
        [Space(10)]
        [SerializeField]
        GameObject menuSection;

        [SerializeField] GameObject helpSection;
        [SerializeField] GameObject editSection;


        [SerializeField] GameObject ChangePassword;
        [SerializeField] GameObject ChangeEmail;

        [SerializeField] Transform ThinLineLanguage;
        [SerializeField] Transform EditAccountContent;
        [SerializeField] Transform ThinLineDeleteAccount;



        [SerializeField] List<GameObject> sections;

        readonly ISubject<Unit> onEnabled = new Subject<Unit>();
        readonly ISubject<Unit> onDisabled = new Subject<Unit>();

        public IObservable<Unit> OnEnabled => onEnabled;
        public IObservable<Unit> OnDisabled => onDisabled;
        public IObservable<Unit> OnHelpButton => helpButton.OnClickAsObservable();
        public IObservable<Unit> OnEditAccountButton => editAccountButton.OnClickAsObservable();
        public IObservable<Unit> OnLogOutButton => logOutButton.OnClickAsObservable();
        public IObservable<Unit> OnLogOutGuestButton => logoutGuestModal.OnClickAsObservable();
        public IObservable<Unit> OnSupportButton => supportButton.OnClickAsObservable();
        public IObservable<Unit> OnTermsButton => termsButton.OnClickAsObservable();
        public IObservable<Unit> OnCodeOfConductButton => codeOfConduct.OnClickAsObservable();
        public IObservable<Unit> OnPrivacyPolicyButton => privacyPolicy.OnClickAsObservable();

        readonly CompositeDisposable disposables = new CompositeDisposable();


        public IGameData gameData;



        [Header("Localization Texts")]
        public TextMeshProUGUI MAIN_ABOUT;
        public TextMeshProUGUI REGISTER_TEXT;
        public TextMeshProUGUI MAIN_ABOUT1;
        public TextMeshProUGUI MAIN_CHANGE_EMAIL;
        public TextMeshProUGUI MAIN_CHANGE_EMAIL2;
        public TextMeshProUGUI MAIN_CHANGE_EMAIL3;
        public TextMeshProUGUI MAIN_CHANGE_LANGUAGE;
        public TextMeshProUGUI MAIN_CHANGE_PASSWORD;
        public TextMeshProUGUI MAIN_CHANGE_PASSWORD2;
        public TextMeshProUGUI MAIN_CHANGE_PASSWORD3;
        public TextMeshProUGUI MAIN_COPYRIGHT_INFO;
        public TextMeshProUGUI MAIN_DELETE_ACCOUNT;
        public TextMeshProUGUI MAIN_EDIT_ACCOUNT;
        public TextMeshProUGUI MAIN_EDIT_ACCOUNT1;
        public TextMeshProUGUI MAIN_ENTER_CURRENT_PASSWORD;
        public TextMeshProUGUI MAIN_ENTER_CURRENT_PASSWORD1;
        public TextMeshProUGUI MAIN_ENTER_YOUR_NEW_EMAIL;
        public TextMeshProUGUI MAIN_ENTER_YOUR_NEW_EMAIL1;
        public TextMeshProUGUI MAIN_HELP;
        public TextMeshProUGUI MAIN_HELP2;
        public TextMeshProUGUI MAIN_LOG_OUT;
        public TextMeshProUGUI MAIN_NEW_EMAIL;
        public TextMeshProUGUI MAIN_OLD_PASSWORD;

        public TextMeshProUGUI MAIN_SOUND;
        public TextMeshProUGUI MAIN_SUPPORT;

        public TextMeshProUGUI MAIN_YOUVE_SUCCESSFULLY_CHANGED_YOUR_PASSWORD;
        public TextMeshProUGUI MAIN_YOUR_PASSWORD;
        public TextMeshProUGUI MAIN_YOUR_PASSWORD2;
        public TextMeshProUGUI MAIN_ENTER_YOUR_NEW_PASSWORD;
        public TextMeshProUGUI MAIN_ENTER_YOUR_NEW_PASSWORD2;
        public TextMeshProUGUI MAIN_ENTER_YOUR_NEW_PASSWORD3;
        public TextMeshProUGUI MAIN_CLICK_ON_LINK_TO_CONFIRM;
        public TextMeshProUGUI MAIN_YOUVE_SUCCESSFULLY_CHANGED_YOUR_EMAIL;
        public TextMeshProUGUI MAIN_GO_TO_PLAY;
        public TextMeshProUGUI MAIN_GO_TO_PLAY2;
        public TextMeshProUGUI MAIN_ARE_YOU_SURE_DELETE_ACCOUNT;
        public TextMeshProUGUI MAIN_DELETE_ACCOUNT_WARNING;
        public TextMeshProUGUI MAIN_YES_DELETE;
        public TextMeshProUGUI MAIN_NO_KEEP_ACCOUNT;
        public TextMeshProUGUI MAIN_CREDITS_URL;
        public TextMeshProUGUI AUTH_TERMS_AND_CONDITIONS;
        public TextMeshProUGUI AUTH_FORGOT;
        public TextMeshProUGUI AUTH_FORGOT2;
        public TextMeshProUGUI AUTH_FORGOT3;
        public TextMeshProUGUI AUTH_FORGOT4;
        public TextMeshProUGUI AUTH_FORGOT5;
        public TextMeshProUGUI AUTH_FORGOT6;
        public TextMeshProUGUI AUTH_FORGOT7;
        public TextMeshProUGUI AUTH_FORGOT8;
        public TextMeshProUGUI AUTH_FORGOT9;
        public TextMeshProUGUI AUTH_FORGOT10;
        public TextMeshProUGUI COMMON_LABEL_CONTINUE;
        public TextMeshProUGUI COMMON_LABEL_CONTINUE2;
        public TextMeshProUGUI COMMON_LABEL_CONTINUE3;
        public TextMeshProUGUI COMMON_LABEL_CONTINUE4;
        public TextMeshProUGUI COMMON_LABEL_CONTINUE5;
        public TextMeshProUGUI COMMON_LABEL_CONTINUE6;
        public TextMeshProUGUI COMMON_LABEL_CONTINUE7;
        public TextMeshProUGUI AUTH_EMAIL;
        public TextMeshProUGUI AUTH_EMAIL2;
        public TextMeshProUGUI AUTH_EMAIL3;
        public TextMeshProUGUI AUTH_EMAIL4;
        public TextMeshProUGUI AUTH_EMAIL5;
        public TextMeshProUGUI AUTH_EMAIL6;
        public TextMeshProUGUI AUTH_NEW_PASSWORD;
        public TextMeshProUGUI AUTH_CONFIRM_NEW_PASSWORD;
        public TextMeshProUGUI AUTH_SELECT_LANGUAGE;
        public TextMeshProUGUI AUTH_ENTER_YOUR_PASSWORD;
        public TextMeshProUGUI AUTH_ENTER_YOUR_PASSWORD2;
        public TextMeshProUGUI AUTH_CHECK_YOUR_EMAIL;
        public TextMeshProUGUI AUTH_SENT_RECOVER_EMAIL_TO;
        public TextMeshProUGUI AUTH_SENT_RECOVER_EMAIL_TO2;
        public TextMeshProUGUI AUTH_CLICK_ON_THE_LINK;
        public TextMeshProUGUI AUTH_EMAIL_DIDNT_ARRIVE;
        public TextMeshProUGUI AUTH_EMAIL_DIDNT_ARRIVE2;
        public TextMeshProUGUI AUTH_RESEND;
        public TextMeshProUGUI AUTH_RESEND2;
        public TextMeshProUGUI BURG_TERMS;
        public TextMeshProUGUI BURG_CODEOFCONDUCT;
        public TextMeshProUGUI BURG_PRIVACY;
        public TextMeshProUGUI GUEST_ARE_YOU_SURE;
        public TextMeshProUGUI GUEST_CLOSE_GUEST_ACCOUNT;
        public TextMeshProUGUI GUEST_CLOSE_ANYWAY;
        public TextMeshProUGUI GUEST_REGISTER_TO_SAVE;
        public TextMeshProUGUI QUICKPLAY_LOGOUT;
        public TextMeshProUGUI QUICKPLAY_LOGOUT_GUEST;
        public ILanguage language;

        private IEnumerator SetLanguage()
        {
            yield return null;
            language.SetTextByKey(MAIN_ABOUT, LangKeys.MAIN_ABOUT);
            language.SetTextByKey(REGISTER_TEXT, LangKeys.NAUTH_REGISTER);
            language.SetTextByKey(MAIN_ABOUT1, LangKeys.MAIN_ABOUT);
            language.SetTextByKey(MAIN_CHANGE_EMAIL, LangKeys.MAIN_CHANGE_EMAIL);
            language.SetTextByKey(MAIN_CHANGE_EMAIL2, LangKeys.MAIN_CHANGE_EMAIL);
            language.SetTextByKey(MAIN_CHANGE_EMAIL3, LangKeys.MAIN_CHANGE_EMAIL);
            language.SetTextByKey(MAIN_CHANGE_LANGUAGE, LangKeys.MAIN_CHANGE_LANGUAGE);
            language.SetTextByKey(MAIN_CHANGE_PASSWORD, LangKeys.MAIN_CHANGE_PASSWORD);
            language.SetTextByKey(MAIN_CHANGE_PASSWORD2, LangKeys.MAIN_CHANGE_PASSWORD);
            language.SetTextByKey(MAIN_CHANGE_PASSWORD3, LangKeys.MAIN_CHANGE_PASSWORD);
            language.SetTextByKey(QUICKPLAY_LOGOUT, LangKeys.QUICKPLAY_LOGOUT);
            language.SetTextByKey(QUICKPLAY_LOGOUT_GUEST, LangKeys.QUICKPLAY_LOGOUT);



            //MAIN_COPYRIGHT_INFO.text = language.GetTextByKey(LangKeys.MAIN_COPYRIGHT_INFO) + "";


            language.SetTextByKey(MAIN_DELETE_ACCOUNT, LangKeys.MAIN_DELETE_ACCOUNT);
            language.SetTextByKey(MAIN_EDIT_ACCOUNT, LangKeys.MAIN_EDIT_ACCOUNT);
            language.SetTextByKey(MAIN_EDIT_ACCOUNT1, LangKeys.MAIN_EDIT_ACCOUNT);
            language.SetTextByKey(MAIN_ENTER_CURRENT_PASSWORD, LangKeys.MAIN_ENTER_CURRENT_PASSWORD);
            language.SetTextByKey(MAIN_ENTER_CURRENT_PASSWORD1, LangKeys.MAIN_ENTER_CURRENT_PASSWORD);
            language.SetTextByKey(MAIN_ENTER_YOUR_NEW_EMAIL, LangKeys.MAIN_ENTER_YOUR_NEW_EMAIL);
            language.SetTextByKey(MAIN_ENTER_YOUR_NEW_EMAIL1, LangKeys.MAIN_ENTER_YOUR_NEW_EMAIL);
            language.SetTextByKey(MAIN_HELP, LangKeys.MAIN_HELP);
            language.SetTextByKey(MAIN_HELP2, LangKeys.MAIN_HELP);
            language.SetTextByKey(MAIN_LOG_OUT, LangKeys.MAIN_LOG_OUT);
            language.SetTextByKey(MAIN_NEW_EMAIL, LangKeys.MAIN_NEW_EMAIL);
            language.SetTextByKey(MAIN_OLD_PASSWORD, LangKeys.MAIN_OLD_PASSWORD);


            language.SetTextByKey(MAIN_SUPPORT, LangKeys.MAIN_SUPPORT);

            language.SetTextByKey(MAIN_YOUVE_SUCCESSFULLY_CHANGED_YOUR_PASSWORD, LangKeys.MAIN_YOUVE_SUCCESSFULLY_CHANGED_YOUR_PASSWORD);
            language.SetTextByKey(MAIN_YOUR_PASSWORD, LangKeys.MAIN_YOUR_PASSWORD);
            language.SetTextByKey(MAIN_YOUR_PASSWORD2, LangKeys.MAIN_YOUR_PASSWORD);
            language.SetTextByKey(MAIN_ENTER_YOUR_NEW_PASSWORD, LangKeys.MAIN_ENTER_YOUR_NEW_PASSWORD);
            language.SetTextByKey(MAIN_ENTER_YOUR_NEW_PASSWORD2, LangKeys.MAIN_ENTER_YOUR_NEW_PASSWORD);
            language.SetTextByKey(MAIN_ENTER_YOUR_NEW_PASSWORD3, LangKeys.MAIN_ENTER_YOUR_NEW_PASSWORD);
            language.SetTextByKey(MAIN_CLICK_ON_LINK_TO_CONFIRM, LangKeys.MAIN_CLICK_ON_LINK_TO_CONFIRM);
            language.SetTextByKey(MAIN_YOUVE_SUCCESSFULLY_CHANGED_YOUR_EMAIL, LangKeys.MAIN_YOUVE_SUCCESSFULLY_CHANGED_YOUR_EMAIL);
            language.SetTextByKey(MAIN_GO_TO_PLAY, LangKeys.MAIN_GO_TO_PLAY);
            language.SetTextByKey(MAIN_GO_TO_PLAY2, LangKeys.MAIN_GO_TO_PLAY);
            language.SetTextByKey(MAIN_ARE_YOU_SURE_DELETE_ACCOUNT, LangKeys.MAIN_ARE_YOU_SURE_DELETE_ACCOUNT);
            language.SetTextByKey(MAIN_DELETE_ACCOUNT_WARNING, LangKeys.MAIN_DELETE_ACCOUNT_WARNING);
            language.SetTextByKey(MAIN_YES_DELETE, LangKeys.MAIN_YES_DELETE);
            language.SetTextByKey(MAIN_NO_KEEP_ACCOUNT, LangKeys.MAIN_NO_KEEP_ACCOUNT);
            language.SetTextByKey(MAIN_CREDITS_URL, LangKeys.MAIN_CREDITS_URL);
            language.SetTextByKey(AUTH_TERMS_AND_CONDITIONS, LangKeys.AUTH_TERMS_AND_CONDITIONS);
            language.SetTextByKey(AUTH_FORGOT, LangKeys.AUTH_FORGOT);
            language.SetTextByKey(AUTH_FORGOT2, LangKeys.AUTH_FORGOT);
            language.SetTextByKey(AUTH_FORGOT3, LangKeys.AUTH_FORGOT);
            language.SetTextByKey(AUTH_FORGOT4, LangKeys.AUTH_FORGOT);
            language.SetTextByKey(AUTH_FORGOT5, LangKeys.AUTH_FORGOT);
            language.SetTextByKey(AUTH_FORGOT6, LangKeys.AUTH_FORGOT);
            language.SetTextByKey(AUTH_FORGOT7, LangKeys.AUTH_FORGOT);
            language.SetTextByKey(AUTH_FORGOT8, LangKeys.AUTH_FORGOT);
            language.SetTextByKey(AUTH_FORGOT9, LangKeys.AUTH_FORGOT);
            language.SetTextByKey(AUTH_FORGOT10, LangKeys.AUTH_FORGOT);
            language.SetTextByKey(COMMON_LABEL_CONTINUE, LangKeys.COMMON_LABEL_CONTINUE);
            language.SetTextByKey(COMMON_LABEL_CONTINUE2, LangKeys.COMMON_LABEL_CONTINUE);
            language.SetTextByKey(COMMON_LABEL_CONTINUE3, LangKeys.COMMON_LABEL_CONTINUE);
            language.SetTextByKey(COMMON_LABEL_CONTINUE4, LangKeys.COMMON_LABEL_CONTINUE);
            language.SetTextByKey(COMMON_LABEL_CONTINUE5, LangKeys.COMMON_LABEL_CONTINUE);
            language.SetTextByKey(COMMON_LABEL_CONTINUE6, LangKeys.COMMON_LABEL_CONTINUE);
            language.SetTextByKey(COMMON_LABEL_CONTINUE7, LangKeys.COMMON_LABEL_CONTINUE);
            language.SetTextByKey(AUTH_EMAIL, LangKeys.AUTH_EMAIL);
            language.SetTextByKey(AUTH_EMAIL2, LangKeys.AUTH_EMAIL);
            language.SetTextByKey(AUTH_EMAIL3, LangKeys.AUTH_EMAIL);
            language.SetTextByKey(AUTH_EMAIL4, LangKeys.AUTH_EMAIL);
            language.SetTextByKey(AUTH_EMAIL5, LangKeys.AUTH_EMAIL);
            language.SetTextByKey(AUTH_EMAIL6, LangKeys.AUTH_EMAIL);
            language.SetTextByKey(AUTH_NEW_PASSWORD, LangKeys.AUTH_NEW_PASSWORD);
            language.SetTextByKey(AUTH_CONFIRM_NEW_PASSWORD, LangKeys.AUTH_CONFIRM_NEW_PASSWORD);
            language.SetTextByKey(AUTH_SELECT_LANGUAGE, LangKeys.AUTH_SELECT_LANGUAGE);
            language.SetTextByKey(AUTH_ENTER_YOUR_PASSWORD, LangKeys.AUTH_ENTER_YOUR_PASSWORD);
            language.SetTextByKey(AUTH_ENTER_YOUR_PASSWORD2, LangKeys.AUTH_ENTER_YOUR_PASSWORD);
            language.SetTextByKey(AUTH_CHECK_YOUR_EMAIL, LangKeys.AUTH_CHECK_YOUR_EMAIL);
            language.SetTextByKey(AUTH_SENT_RECOVER_EMAIL_TO, LangKeys.AUTH_SENT_RECOVER_EMAIL_TO);
            language.SetTextByKey(AUTH_SENT_RECOVER_EMAIL_TO2, LangKeys.AUTH_SENT_RECOVER_EMAIL_TO);
            language.SetTextByKey(AUTH_CLICK_ON_THE_LINK, LangKeys.AUTH_CLICK_ON_THE_LINK);
            language.SetTextByKey(AUTH_EMAIL_DIDNT_ARRIVE, LangKeys.AUTH_EMAIL_DIDNT_ARRIVE);
            language.SetTextByKey(AUTH_EMAIL_DIDNT_ARRIVE2, LangKeys.AUTH_EMAIL_DIDNT_ARRIVE);
            language.SetTextByKey(AUTH_RESEND, LangKeys.AUTH_RESEND);
            language.SetTextByKey(AUTH_RESEND2, LangKeys.AUTH_RESEND);
            language.SetTextByKey(BURG_TERMS, LangKeys.AUTH_TERMS_AND_CONDITIONS);
            language.SetTextByKey(BURG_CODEOFCONDUCT, LangKeys.HAMBURGER_CODE_OF_CONDUCT);
            language.SetTextByKey(BURG_PRIVACY, LangKeys.HAMBURGER_PRIVACY);
            termsAndConditionsUrl = language.GetTextByKey(LangKeys.AUTH_TERMS_AND_CONDITIONS_LINK);
            codeOfConductUrl = language.GetTextByKey(LangKeys.AUTH_CODE_OF_CONDUCT);

            language.SetTextByKey(GUEST_ARE_YOU_SURE, LangKeys.GUEST_ARE_YOU_SURE);
            language.SetTextByKey(GUEST_CLOSE_GUEST_ACCOUNT, LangKeys.GUEST_CLOSE_GUEST_ACCOUNT);
            language.SetTextByKey(GUEST_CLOSE_ANYWAY, LangKeys.GUEST_CLOSE_ANYWAY);
            language.SetTextByKey(GUEST_REGISTER_TO_SAVE, LangKeys.GUEST_REGISTER_TO_SAVE);

        }
        private void Awake()
        {
            if (language == null)
                language = Injection.Get<ILanguage>();
            this.CreatePresenter<BurgerPresenter, IBurguerView>();
            AM = FindObjectOfType<Audio.AudioModule>();
            gameData = Injection.Get<IGameData>();
            GuestBehaviour(gameData.IsGuest());
        }

        void GuestBehaviour(bool isGuest)
        {
            editAccountButton.gameObject.SetActive(!isGuest);
            logOutButton.gameObject.SetActive(!isGuest);
            registerButton.gameObject.SetActive(isGuest);
            logoutGuest.gameObject.SetActive(isGuest);
        }

        public void LinkProvider()
        {
            CurrentRoom.Instance.LinkProvider();
        }

        void OnEnable()
        {
            onEnabled.OnNext(Unit.Default);
            MainMenuManager.OnExitSection += ExitView;
            HelpMenuManager.OnExitSection += ExitView;
            EditAccountManager.OnExitSection += ExitView;
            if (string.IsNullOrEmpty(gameData.GetUserInformation().Email.ToString()))
            {//this is so UI looks ok when you are missing some options because you dont have email
                ThinLineLanguage.localScale = new Vector3(0.7f,1,1);
                EditAccountContent.localScale = new Vector3(1.44f, 1.44f, 1.44f); 
                ThinLineDeleteAccount.localScale = new Vector3(0.7f, 1, 1); 
                ChangePassword.SetActive(false);
                ChangeEmail.SetActive(false);
            }
            else
            {
                ChangePassword.SetActive(true);
                ChangeEmail.SetActive(true);
            }
        }

        void OnDisable()
        {
            MainMenuManager.OnExitSection -= ExitView;
            HelpMenuManager.OnExitSection -= ExitView;
            EditAccountManager.OnExitSection -= ExitView;
            onDisabled.OnNext(Unit.Default);
        }

        public void ShowSection(BurgerSection section)
        {
            HideSections();
            screenBlocker.gameObject.SetActive(true);

            switch (section)
            {
                case BurgerSection.Menu:
                    menuSection.SetActive(true);
                    break;
                case BurgerSection.Help:
                    helpSection.SetActive(true);
                    break;
                case BurgerSection.EditAccount:
                    editSection.SetActive(true);
                    break;
            }
        }

        public void HideSections()
        {
            screenBlocker.gameObject.SetActive(false);

            foreach (var section in sections)
            {
                section.SetActive(false);
            }
        }

        public void ExitView()
        {
            HideSections();
            gameObject.SetActive(false);
        }


        public void ToggleSound()
        {
            if (language == null)
                language = Injection.Get<ILanguage>();
            AM.ToggleSound();
            AudioOnGraphic.SetActive(!AudioOnGraphic.activeSelf);
            AudioOffGraphic.SetActive(!AudioOnGraphic.activeSelf);

            SoundText.text = AM.IsAudioMuted() ? language.GetTextByKey(LangKeys.MAIN_SOUND_OFF) : language.GetTextByKey(LangKeys.MAIN_SOUND_ON);
        }

        private void SetSoundGraphics()
        {
            if (language == null)
                language = Injection.Get<ILanguage>();
            AudioOnGraphic.SetActive(!AM.IsAudioMuted());
            AudioOffGraphic.SetActive(AM.IsAudioMuted());
            SoundText.text = AM.IsAudioMuted() ? language.GetTextByKey(LangKeys.MAIN_SOUND_OFF) : language.GetTextByKey(LangKeys.MAIN_SOUND_ON);
        }

        void Start()
        {
            StartCoroutine(SetLanguage());
            GuestBehaviour(gameData.IsGuest());
            SetSoundGraphics();
        }

        public void VisitExternalSectionUrl(HelpSections sectionsKey)
        {
            switch (sectionsKey)
            {
                case HelpSections.Support:
#if UNITY_EDITOR
                    Application.OpenURL(friendbaseFaqUrl);
#endif
#if UNITY_ANDROID || UNITY_IOS
                    OpenWebView(friendbaseFaqUrl);
#endif
                    break;
                case HelpSections.TermsAndConditions:
#if UNITY_EDITOR
                    Application.OpenURL(termsAndConditionsUrl);
#endif
#if UNITY_ANDROID || UNITY_IOS
                    OpenWebView(termsAndConditionsUrl);
#endif
                    break;
                case HelpSections.CodeOfConduct:
#if UNITY_EDITOR
                    Application.OpenURL(codeOfConductUrl);
#endif
#if UNITY_ANDROID || UNITY_IOS
                    OpenWebView(codeOfConductUrl);
#endif
                    break;
                case HelpSections.PrivacyPolicy:
#if UNITY_EDITOR
                    Application.OpenURL(privacyPolicyUrl);
#endif
#if UNITY_ANDROID || UNITY_IOS
                    OpenWebView(privacyPolicyUrl);
#endif
                    break;
            }
        }

        public void CloseWebView()
        {
            webView.close();
            webViewContainer.SetActive(false);
        }

        public void OpenWebView(string Url)
        {
            webViewContainer.SetActive(true);
            webView.setUrl(Url);
        }
    }
}
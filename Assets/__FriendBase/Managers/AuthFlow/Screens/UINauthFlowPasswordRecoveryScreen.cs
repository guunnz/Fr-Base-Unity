using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Architecture.Injector.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINauthFlowPasswordRecoveryScreen : AbstractAuthFlowScreen
{
    public override NauthFlowScreenType ScreenType => NauthFlowScreenType.PASSWORD_RECOVERY;

    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private UINauthFlowGenericInputText inputMail;
    [SerializeField] private Button btnAction;
    [SerializeField] private TextMeshProUGUI txtBtnAction;
    [SerializeField] private TextMeshProUGUI txtDoNotReceiveMail;
    [SerializeField] private TextMeshProUGUI txtBubbleMsg;
    [SerializeField] private GameObject bubbleContainer;

    protected override void Start()
    {
        base.Start();
        language.SetTextByKey(txtTitle, LangKeys.NAUTH_PASSWORD_RECOVERY);
    }

    public override void OnOpen()
    {
        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenRecoverPassword);

        txtDoNotReceiveMail.gameObject.SetActive(false);
        language.SetTextByKey(txtBtnAction, LangKeys.NAUTH_SEND);

        string emailTxt = language.GetTextByKey(LangKeys.NAUTH_EMAIL);
        string enterEmailTxt = language.GetTextByKey(LangKeys.NAUTH_ENTER_MAIL);

        inputMail.SetUp(UINauthFlowGenericInputText.InputType.Mail, emailTxt, enterEmailTxt);
        language.SetTextByKey(txtBubbleMsg, LangKeys.NAUTH_WE_WILL_SEND_YOU_AN_EMAIL);
    }

    public async void OnBtnActionPressDown()
    {
        string mail = inputMail.GetText();

        if (!IsValidEmail(mail))
        {
            inputMail.SetBoxFieldOrangeStroke();
            string msg = language.GetTextByKey(LangKeys.NAUTH_EMAIL_IS_INCORRECT);
            inputMail.ShowBubbleMedium(msg, UINauthFlowBoobleController.IconType.ALERT);
            return;
        }

        inputMail.SetBoxFieldDefault();
        HideButtonAction();
        analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenRecoverPasswordPress);
        bool result = await Injection.Get<IAuthFlowEndpoint>().SendEmailResetPassword(mail);
        ShowButtonAction(result);
    }

    void HideButtonAction()
    {
        bubbleContainer.SetActive(false);
        btnAction.gameObject.SetActive(false);
        txtDoNotReceiveMail.gameObject.SetActive(false);
        inputMail.HideBubbles();
    }

    void ShowButtonAction(bool result)
    {
        if (result)
        {
            analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenRecoverPasswordSucceed);

            txtDoNotReceiveMail.gameObject.SetActive(true);
            language.SetTextByKey(txtBubbleMsg, LangKeys.NAUTH_WE_SENT_YOU_AN_EMAIL_TO_CREATE_PASSWORD);
            language.SetTextByKey(txtDoNotReceiveMail, LangKeys.NAUTH_DID_NOT_RECEIVE_IT);
            inputMail.SetBoxFieldYellowBackground();
            bubbleContainer.SetActive(true);
            btnAction.gameObject.SetActive(true);
            language.SetTextByKey(txtBtnAction, LangKeys.NAUTH_RESEND);
        }
        else
        {
            analyticsSender.SendAnalytics(AnalyticsEvent.AuthFlowScreenRecoverPasswordError);

            btnAction.gameObject.SetActive(true);
            language.SetTextByKey(txtBtnAction, LangKeys.NAUTH_SEND);
            inputMail.SetBoxFieldOrangeStroke();

            string noAccountTxt = language.GetTextByKey(LangKeys.NAUTH_THERE_IS_NO_ACCOUNT_REGISTERED);
            inputMail.ShowBubbleMedium(noAccountTxt, UINauthFlowBoobleController.IconType.ALERT);
        }
    }

    public void OnBtnBackDown()
    {
        authFlowManager.GoScreen(NauthFlowScreenType.LOGIN_WITH_MAIL);
    }
}

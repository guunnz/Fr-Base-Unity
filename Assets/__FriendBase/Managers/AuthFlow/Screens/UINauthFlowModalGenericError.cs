using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINauthFlowModalGenericError : AbstractUIPanel
{
    public enum MessageType { DIFFERENT_PROVIDER };

    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtDescription;
    [SerializeField] private TextMeshProUGUI txtBtnOk;

    public override void Open()
    {
        base.Open();
        ShowDefaultMessage();
    }

    public void Open(MessageType messageType)
    {
        base.Open();
        if (messageType == MessageType.DIFFERENT_PROVIDER)
        {
            language.SetTextByKey(txtTitle, LangKeys.COMMON_LABEL_THERE_WAS_AN_UNEXPECTED_ERROR);
            language.SetTextByKey(txtDescription, LangKeys.NAUTH_MAIL_REGISTERED_WITH_OTHER_PROVIDER);
            language.SetTextByKey(txtBtnOk, LangKeys.COMMON_LABEL_OK);
        }
        else
        {
            ShowDefaultMessage();
        }
    }

    void ShowDefaultMessage()
    {
        language.SetTextByKey(txtTitle, LangKeys.COMMON_LABEL_THERE_WAS_AN_UNEXPECTED_ERROR);
        language.SetTextByKey(txtDescription, LangKeys.TRY_AGAIN_ERROR);
        language.SetTextByKey(txtBtnOk, LangKeys.COMMON_LABEL_OK);
    }
}

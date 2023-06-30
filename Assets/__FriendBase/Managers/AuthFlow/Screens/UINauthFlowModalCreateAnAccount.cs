using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINauthFlowModalCreateAnAccount : AbstractUIPanel
{
    [SerializeField] private TextMeshProUGUI txtTitle;
    [SerializeField] private TextMeshProUGUI txtDescription;
    [SerializeField] private TextMeshProUGUI txtBtnGoBack;
    [SerializeField] private TextMeshProUGUI txtBtnCreateAccount;

    public delegate void GoToCreateAccount();
    public event GoToCreateAccount OnGoToCreateAccount;

    protected override void Start()
    {
        base.Start();
    }

    public override void OnOpen()
    {
        language.SetTextByKey(txtTitle, LangKeys.NAUTH_YOU_NEED_TO_CREATE_ACCOUNT);
        language.SetTextByKey(txtDescription, LangKeys.NAUTH_ACCOUNT_DOES_NOT_EXIST);
        language.SetTextByKey(txtBtnGoBack, LangKeys.NAUTH_GO_BACK );
        language.SetTextByKey(txtBtnCreateAccount, LangKeys.NAUTH_LETS_CREATE_ONE);
    }

    public void PressGoToCreateAccount()
    {
        if (OnGoToCreateAccount!=null)
        {
            OnGoToCreateAccount();
        }
        Close();
    }
}

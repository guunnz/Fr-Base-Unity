using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIMsgPanelBanned : AbstractUIPanel
{
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtDesc;
    [SerializeField] protected TextMeshProUGUI txtBtnAccept;

    private Action callbackOnAccept;

    public void Open(string title, string desc, string btnAccept, Action callback)
    {
        language.SetText(txtTitle, title);
        language.SetText(txtDesc, desc);
        language.SetText(txtBtnAccept, btnAccept);
        callbackOnAccept = callback;
        base.Open();
    }

    public void OpenWithBannedDescription(Action callback)
    {
        string btnAccept = language.GetTextByKey(LangKeys.COMMON_LABEL_OK);
        string title = "Account banned";
        string desc = "Your account has been permanently suspended for violations of the Friendbase Code of Conduct.";

        Open(title, desc, btnAccept, callback);
    }

    public void OpenWithSuspendedDescription(TimeSpan timespan, Action callback)
    {
        string btnAccept = language.GetTextByKey(LangKeys.COMMON_LABEL_OK);
        string title = "Your account is suspended";
        string desc = "It looks like you broke the Friendbase Code of Conduct by using foul language. \nYou will not be able to go to public places or play minigames for {0} days. If you have any questions please contact us at support@friendbase.com";
        desc = string.Format(desc, timespan.Days);
        Open(title, desc, btnAccept, callback);
    }
}

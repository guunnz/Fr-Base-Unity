using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIFirstTimeModal : AbstractUIPanel
{
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtDesc;
    [SerializeField] protected TextMeshProUGUI txtBtn;

    public override void OnOpen()
    {
        language.SetTextByKey(txtTitle, LangKeys.AUTH_ACCOUNT_WAS_CREATED);
        language.SetTextByKey(txtDesc, LangKeys.AUTH_NOW_LETS_MAKE_AVATAR);
        language.SetTextByKey(txtBtn, LangKeys.COMMON_LABEL_OK);
    }
}

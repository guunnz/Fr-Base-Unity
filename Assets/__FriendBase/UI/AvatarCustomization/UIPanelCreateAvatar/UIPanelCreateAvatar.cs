using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPanelCreateAvatar : AbstractUIPanel
{
    [SerializeField] protected TextMeshProUGUI txtBtnDone;

    public override void OnOpen()
    {
        language.SetTextByKey(txtBtnDone, LangKeys.STORE_DONE);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIDialogPanel : AbstractUIPanel
{
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtDesc;
    [SerializeField] protected TextMeshProUGUI txtBtnAccept;
    [SerializeField] protected TextMeshProUGUI txtBtnDiscard;

    private Action callbackOnAccept;
    private Action callbackDiscard;

    public void Open(string title, string desc, string btnAccept, string btnDiscard,  Action callback,  Action callbackDiscard = null, bool gemPurchase = true)
    {
        if (!gemPurchase)
        {
            txtBtnAccept.transform.parent.gameObject.SetActive(false);
            txtBtnDiscard.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            txtBtnAccept.transform.parent.gameObject.SetActive(true);
            txtBtnDiscard.transform.parent.gameObject.SetActive(true);
        }
        language.SetText(txtTitle, title);
        language.SetText(txtDesc, desc);
        language.SetText(txtBtnAccept, btnAccept);
        language.SetText(txtBtnDiscard, btnDiscard);


        callbackOnAccept = callback;
        this.callbackDiscard = callbackDiscard;
        base.Open();
    }

    public void OnBtnAccept()
    {
        Close();
        if (callbackOnAccept != null)
        {
            callbackOnAccept();
        }
    }

    public void OnBtnDiscard()
    {
        Close();
        if (callbackDiscard != null)
        {
            callbackDiscard();
        }
    }
}

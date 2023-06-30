using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using LocalizationSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINauthFlowCheckboxTerms : MonoBehaviour
{
    [SerializeField] private Image imgIconInScene;
    [SerializeField] private Sprite imgIconEmpty;
    [SerializeField] private Sprite imgIconChecked;
    [SerializeField] private Sprite imgIconWrong;
    [SerializeField] private GameObject boobleAlert;
    [SerializeField] private TextMeshProUGUI txtBoobleAlert;

    public bool FlagTermsAndConditions { get; private set; }

    void Start()
    {
        Injection.Get<ILanguage>().SetTextByKey(txtBoobleAlert, LangKeys.NAUTH_CHECK_FINISH);
    }

    public void Open()
    {
        FlagTermsAndConditions = false;
        RefreshIconTermsAndConditions();
        boobleAlert.SetActive(false);
    }

    public void SetWrong()
    {
        imgIconInScene.sprite = imgIconWrong;
        boobleAlert.SetActive(true);
    }

    private void RefreshIconTermsAndConditions()
    {
        boobleAlert.SetActive(false);
        if (FlagTermsAndConditions)
        {
            imgIconInScene.sprite = imgIconChecked;
        }
        else
        {
            imgIconInScene.sprite = imgIconEmpty;
        }
    }

    public void OnPressTermsAndConditions()
    {
        FlagTermsAndConditions = !FlagTermsAndConditions;
        RefreshIconTermsAndConditions();
    }
}
